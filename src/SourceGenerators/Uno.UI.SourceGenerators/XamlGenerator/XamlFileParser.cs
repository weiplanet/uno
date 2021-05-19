﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Uno.Extensions;
using Uno.Logging;
using System.IO;
using System.Reflection;
using Uno.UI.SourceGenerators.XamlGenerator.XamlRedirection;
using System.Text.RegularExpressions;
using Windows.Foundation.Metadata;
using Uno.UI.SourceGenerators.XamlGenerator.Utils;
using System.Diagnostics;
using Uno.Roslyn;

namespace Uno.UI.SourceGenerators.XamlGenerator
{
	internal class XamlFileParser
	{
		private readonly string[] _excludeXamlNamespaces;
		private readonly string[] _includeXamlNamespaces;
		private readonly RoslynMetadataHelper _metadataHelper;
		private int _depth = 0;

		public XamlFileParser(string[] excludeXamlNamespaces, string[] includeXamlNamespaces, RoslynMetadataHelper roslynMetadataHelper)
		{
			_excludeXamlNamespaces = excludeXamlNamespaces;
			_includeXamlNamespaces = includeXamlNamespaces;
			this._metadataHelper = roslynMetadataHelper;
		}

		public XamlFileDefinition[] ParseFiles(string[] xamlSourceFiles)
		{
			var files = new List<XamlFileDefinition>();

			return xamlSourceFiles
				.AsParallel()
				.Select(ParseFile)
				.Where(f => f != null)
				.ToArray()!;
		}

		private XamlFileDefinition? ParseFile(string file)
		{
			try
			{
				this.Log().InfoFormat("Pre-processing XAML file: {0}", file);

				var document = ApplyIgnorables(file);

				// Initialize the reader using an empty context, because when the tasl
				// is run under the BeforeCompile in VS IDE, the loaded assemblies are used 
				// to interpret the meaning of objects, which is not correct in Uno.UI context.
				var context = new XamlSchemaContext(Enumerable.Empty<Assembly>());

				// Force the line info, otherwise it will be enabled only when the debugger is attached.
				var settings = new XamlXmlReaderSettings() { ProvideLineInfo = true };

				using (var reader = new XamlXmlReader(document, context, settings))
				{
					if (reader.Read())
					{
						return Visit(reader, file);
					}
				}

				return null;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to parse file {file}", e);
			}
		}

		private XmlReader ApplyIgnorables(string file)
		{
			var adjusted = File.ReadAllText(file);

			var document = new XmlDocument();
			document.LoadXml(adjusted);

			var (ignorables, shouldCreateIgnorable) = FindIgnorables(document);
			var conditionals = FindConditionals(document);

			shouldCreateIgnorable |= conditionals.ExcludedConditionals.Count > 0;

			var hasxBind = adjusted.Contains("{x:Bind", StringComparison.Ordinal);

			if (ignorables == null && !shouldCreateIgnorable && !hasxBind)
			{
				// No need to modify file
				return XmlReader.Create(file);
			}

			var originalIgnorables = ignorables?.Value ?? "";

			var ignoredNs = originalIgnorables.Split(' ');

			var newIgnored = ignoredNs
				.Except(_includeXamlNamespaces)
				.Concat(_excludeXamlNamespaces.Where(n => document.DocumentElement?.GetNamespaceOfPrefix(n).HasValue() ?? false))
				.Concat(conditionals.ExcludedConditionals.Select(a => a.LocalName))
				.ToArray();
			var newIgnoredFlat = newIgnored.JoinBy(" ");

			if (ignorables != null)
			{
				ignorables.Value = newIgnoredFlat;

				this.Log().InfoFormat("Ignorable XAML namespaces: {0} for {1}", ignorables.Value, file);

				// change the namespaces using textreplace, to keep the formatting and have proper
				// line/position reporting.
				adjusted = adjusted
					.Replace(
						"Ignorable=\"{0}\"".InvariantCultureFormat(originalIgnorables),
						"Ignorable=\"{0}\"".InvariantCultureFormat(ignorables.Value)
					)
					.TrimEnd("\r\n");
			}
			else
			{
				// No existing Ignorable node, create one
				var targetLine = File.ReadLines(file, Encoding.UTF8).First(l => !l.Trim().StartsWith("<!") && !l.IsNullOrWhiteSpace());
				if (targetLine.EndsWith(">"))
				{
					targetLine = targetLine.TrimEnd(">");
				}

				var mcName = document.DocumentElement?
					.Attributes
					.Cast<XmlAttribute>()
					.FirstOrDefault(a => a.Prefix == "xmlns" && a.Value == "http://schemas.openxmlformats.org/markup-compatibility/2006")
					?.LocalName;

				var mcString = "";
				if (mcName == null)
				{
					mcName = "mc";
					mcString = " xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"";
				}

				var replacement = "{0}{1} {2}:Ignorable=\"{3}\"".InvariantCultureFormat(targetLine, mcString, mcName, newIgnoredFlat);
				adjusted = ReplaceFirst(
						adjusted,
						targetLine,
						replacement
					)
					.TrimEnd("\r\n");
			}

			// Replace the ignored namespaces with unique urns so that same urn that are placed in Ignored attribute
			// are ignored independently.
			foreach (var n in newIgnored)
			{
				adjusted = adjusted
					.Replace(
						"xmlns:{0}=\"{1}\"".InvariantCultureFormat(n, document.DocumentElement?.GetNamespaceOfPrefix(n)),
						"xmlns:{0}=\"{1}\"".InvariantCultureFormat(n, Guid.NewGuid())
					);
			}

			// Put all the included namespaces in the same default namespace, so that the properties get their
			// DeclaringType properly set.
			foreach (var n in _includeXamlNamespaces)
			{
				if (document.DocumentElement != null)
				{
					var originalPrefix = document.DocumentElement.GetNamespaceOfPrefix(n);

					if (!originalPrefix.StartsWith("using:"))
					{
						adjusted = adjusted
							.Replace(
								"xmlns:{0}=\"{1}\"".InvariantCultureFormat(n, document.DocumentElement.GetNamespaceOfPrefix(n)),
								"xmlns:{0}=\"{1}\"".InvariantCultureFormat(n, document.DocumentElement.GetNamespaceOfPrefix(""))
							);
					}
				}
			}

			foreach (var includedCond in conditionals.IncludedConditionals)
			{
				var valueSplit = includedCond.Value.Split('?');
				// Strip the conditional part, so the namespace can be parsed correctly by the Xaml reader
				adjusted = adjusted
					.Replace(
						includedCond.OuterXml,
						"{0}=\"{1}\"".InvariantCultureFormat(includedCond.Name, valueSplit[0])
					);
			}

			if (hasxBind)
			{
				// Apply replacements to avoid having issues with the XAML parser which does not
				// support quotes in positional markup extensions parameters.
				// Note that the UWP preprocessor does not need to apply those replacements as the
				// x:Bind expressions are being removed during the first phase and replaced by "connections".
				adjusted = XBindExpressionParser.RewriteDocumentPaths(adjusted);
			}

			return XmlReader.Create(new StringReader(adjusted));
		}

		private static string ReplaceFirst(string targetString, string oldValue, string newValue)
		{
			var index = targetString.IndexOf(oldValue, StringComparison.InvariantCulture);
			if (index < 0)
			{
				throw new InvalidOperationException();
			}
			return targetString.Substring(0, index) + newValue + targetString.Substring(index + oldValue.Length);
		}

		private (XmlNode? Ignorables, bool ShouldCreateIgnorable) FindIgnorables(XmlDocument document)
		{
			var ignorables = document.DocumentElement?.Attributes.GetNamedItem("Ignorable", "http://schemas.openxmlformats.org/markup-compatibility/2006") as XmlAttribute;

			var excludeNamespaces = _excludeXamlNamespaces
				.Select(n => new { Name = n, Namespace = document.DocumentElement?.GetNamespaceOfPrefix(n) })
				.Where(n => n.Namespace.HasValue());

			var shouldCreateIgnorable = false;

			foreach (var nspace in excludeNamespaces)
			{
				var excludeNodes = document
					.DocumentElement
					?.SelectNodes("//* | //@*")
					?.OfType<XmlNode>()
					.Where(e => e.Prefix == nspace.Name);

				if (ignorables == null && (excludeNodes?.Any() ?? false))
				{
					shouldCreateIgnorable = true;
				}
			}

			return (ignorables, shouldCreateIgnorable);
		}

		/// <summary>
		/// Returns those XAML namespace definitions for which a conditional is set, grouped by those for which the conditional returns true and
		/// should be included, and those for which it returns fales and should be excluded.
		/// </summary>
		private (List<XmlAttribute> IncludedConditionals, List<XmlAttribute> ExcludedConditionals) FindConditionals(XmlDocument document)
		{
			var included = new List<XmlAttribute>();
			var excluded = new List<XmlAttribute>();

			foreach (XmlAttribute attr in document.DocumentElement!.Attributes)
			{
				if (attr.Prefix != "xmlns")
				{
					// Not a namespace
					continue;
				}

				var valueSplit = attr.Value.Split('?');
				if (valueSplit.Length != 2)
				{
					// Not a (valid) conditional
					continue;
				}

				if (ShouldInclude() is bool shouldInclude)
				{
					if (shouldInclude)
					{
						included.Add(attr);
					}
					else
					{
						excluded.Add(attr);
					}
				}

				bool? ShouldInclude()
				{
					var elements = valueSplit[1].Split('(', ',', ')');

					var methodName = elements[0];

					switch (methodName)
					{
						case nameof(ApiInformation.IsApiContractPresent):
						case nameof(ApiInformation.IsApiContractNotPresent):
							if (elements.Length < 4 || !ushort.TryParse(elements[2].Trim(), out var majorVersion))
							{
								throw new InvalidOperationException($"Syntax error while parsing conditional namespace expression {attr.Value}");
							}

							return methodName == nameof(ApiInformation.IsApiContractPresent) ?
								ApiInformation.IsApiContractPresent(elements[1], majorVersion) :
								ApiInformation.IsApiContractNotPresent(elements[1], majorVersion);
						case nameof(ApiInformation.IsTypePresent):
						case nameof(ApiInformation.IsTypeNotPresent):
							if (elements.Length < 2)
							{
								throw new InvalidOperationException($"Syntax error while parsing conditional namespace expression {attr.Value}");
							}
							var expectedType = elements[1];
							return methodName == nameof(ApiInformation.IsTypePresent) ?
								ApiInformation.IsTypePresent(elements[1], _metadataHelper) :
								ApiInformation.IsTypeNotPresent(elements[1], _metadataHelper);
						default:
							return null;// TODO: support IsPropertyPresent
					}
				}
			}

			return (included, excluded);
		}

		private XamlFileDefinition Visit(XamlXmlReader reader, string file)
		{
			WriteState(reader);

			var xamlFile = new XamlFileDefinition(file);

			do
			{
				switch (reader.NodeType)
				{
					case XamlNodeType.StartObject:
						_depth++;
						xamlFile.Objects.Add(VisitObject(reader, null));
						break;

					case XamlNodeType.NamespaceDeclaration:
						xamlFile.Namespaces.Add(reader.Namespace);
						break;

					default:
						throw new InvalidOperationException();
				}
			}
			while (reader.Read());

			return xamlFile;
		}

		private void WriteState(XamlXmlReader reader)
		{
			// Console.WriteLine(
			//	$"{new string(' ', Math.Max(0,_depth))}{reader.NodeType} {reader.Type} {reader.Member} {reader.Value}"
			// );
		}

		private XamlObjectDefinition VisitObject(XamlXmlReader reader, XamlObjectDefinition? owner)
		{
			var xamlObject = new XamlObjectDefinition(reader.Type, reader.LineNumber, reader.LinePosition, owner);

			Visit(reader, xamlObject);

			return xamlObject;
		}

		private void Visit(XamlXmlReader reader, XamlObjectDefinition xamlObject)
		{
			while (reader.Read())
			{
				WriteState(reader);

				switch (reader.NodeType)
				{
					case XamlNodeType.StartMember:
						_depth++;
						xamlObject.Members.Add(VisitMember(reader, xamlObject));
						break;

					case XamlNodeType.StartObject:
						_depth++;
						xamlObject.Objects.Add(VisitObject(reader, xamlObject));
						break;

					case XamlNodeType.Value:
						xamlObject.Value = reader.Value;
						break;

					case XamlNodeType.EndObject:
						_depth--;
						return;

					case XamlNodeType.EndMember:
						_depth--;
						break;

					default:
						throw new InvalidOperationException();
				}
			}
		}

		private XamlMemberDefinition VisitMember(XamlXmlReader reader, XamlObjectDefinition owner)
		{
			var member = new XamlMemberDefinition(reader.Member, reader.LineNumber, reader.LinePosition, owner);

			while (reader.Read())
			{
				WriteState(reader);

				switch (reader.NodeType)
				{
					case XamlNodeType.EndMember:
						_depth--;
						return member;

					case XamlNodeType.Value:
						if (IsLiteralInlineText(reader.Value, member, owner))
						{
							var run = ConvertLiteralInlineTextToRun(reader);
							member.Objects.Add(run);
						}
						else
						{
							if (member.Value is string s)
							{
								member.Value += ", " + reader.Value;
							}
							else
							{
								member.Value = reader.Value;
							}
						}
						break;

					case XamlNodeType.StartObject:
						_depth++;
						member.Objects.Add(VisitObject(reader, owner));
						break;

					case XamlNodeType.EndObject:
						_depth--;
						break;

					case XamlNodeType.NamespaceDeclaration:
						// Skip
						break;

					default:
						throw new InvalidOperationException("Unable to process {2} node at Line {0}, position {1}".InvariantCultureFormat(reader.LineNumber, reader.LinePosition, reader.NodeType));
				}
			}

			return member;
		}

		private bool IsLiteralInlineText(object value, XamlMemberDefinition member, XamlObjectDefinition xamlObject)
		{
			return value is string
				&& xamlObject.Type.Name == "TextBlock"
				&& (member.Member.Name == "_UnknownContent" || member.Member.Name == "Inlines");
		}

		private XamlObjectDefinition ConvertLiteralInlineTextToRun(XamlXmlReader reader)
		{
			var runType = new XamlType(
				XamlConstants.PresentationXamlXmlNamespace,
				"Run",
				new List<XamlType>(),
				new XamlSchemaContext()
			);

			var textMember = new XamlMember("Text", runType, false);

			return new XamlObjectDefinition(runType, reader.LineNumber, reader.LinePosition, null)
			{
				Members =
				{
					new XamlMemberDefinition(textMember, reader.LineNumber, reader.LinePosition)
					{
						Value = reader.Value
					}
				}
			};
		}
	}
}
