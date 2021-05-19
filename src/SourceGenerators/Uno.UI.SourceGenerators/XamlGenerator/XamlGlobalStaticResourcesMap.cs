﻿#nullable enable

using Uno.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static Uno.UI.Xaml.XamlFilePathHelper;

namespace Uno.UI.SourceGenerators.XamlGenerator
{
	internal class XamlGlobalStaticResourcesMap
	{
		private readonly Dictionary<string, List<StaticResourceDefinition>> _map = new Dictionary<string, List<StaticResourceDefinition>>();
		private readonly Dictionary<string, XamlFileDefinition> _rdMap = new Dictionary<string, XamlFileDefinition>(StringComparer.CurrentCultureIgnoreCase);

		public XamlGlobalStaticResourcesMap()
		{
		}

		internal StaticResourceDefinition? FindResource(string resourceKey)
		{
			var list = GetListForKey(resourceKey);

			return list.OrderBy(k => k.Precedence).FirstOrDefault();
		}

		internal void Add(string staticResourceKey, string ns, ResourcePrecedence precedence)
		{
			var list = GetListForKey(staticResourceKey);

			list.Add(new StaticResourceDefinition(staticResourceKey, ns, precedence));
		}

		/// <summary>
		/// Gets the names of all GlobalStaticResources properties associated with a top-level ResourceDictionary.
		/// </summary>
		/// <param name="initialFiles">File names of the ResourceDictionaries whose properties should be returned first.</param>
		/// <param name="ignoredFiles">Files which shouldn't be included in the default system resources</param>
		/// <remarks>This is used when building Uno.UI itself to create a master dictionary of system resources.</remarks>
		internal IEnumerable<string> GetAllDictionaryProperties(string[] initialFiles)
		{
			var initialProperties = initialFiles.Select(f =>
					_rdMap.First(kvp =>
						kvp.Key.EndsWith(f, StringComparison.InvariantCultureIgnoreCase)
					)
				)
				.ToArray();

			return initialProperties.Concat(
					_rdMap.Except(initialProperties)
				)
				.Select(kvp => ConvertIdToResourceDictionaryProperty(kvp.Value.UniqueID));
		}

		/// <summary>
		/// Gets the name of a GlobalStaticResources property associated with a ResourceDictionary.Source designation. Throws an exception if none is found.
		/// </summary>
		/// <param name="originDictionary">The file containing the XAML</param>
		/// <param name="source">The ResourceDictionary.Source value</param>
		internal string? FindTargetPropertyForMergedDictionarySource(XamlFileDefinition originDictionary, string source)
		{
			var targetSource = GetSourceLink(originDictionary);
			var absoluteSource = ResolveAbsoluteSource(targetSource, source);

			if (_rdMap.TryGetValue(absoluteSource, out var xamlFileDefinition))
			{
				// TODO: check if it actually corresponds to a ResourceDictionary
				return ConvertIdToResourceDictionaryProperty(xamlFileDefinition.UniqueID);
			}

			return null;
		}

		/// <summary>
		/// Get the appropriate source link for a given XAML file.
		/// </summary>
		internal string GetSourceLink(XamlFileDefinition xamlFileDefinition)
		{
			return _rdMap.FirstOrDefault(kvp => kvp.Value == xamlFileDefinition).Key; //TODO: this is O(n), is it an actual perf issue?
		}

		private string ConvertIdToResourceDictionaryProperty(string id) => "{0}_ResourceDictionary".InvariantCultureFormat(id);

		/// <summary>
		/// Build a map of source links to corresponding XAML files, used for ResourceDictionary.Source resolution.
		/// </summary>
		internal void BuildResourceDictionaryMap(XamlFileDefinition[] files, string[] links)
		{
			for (int i = 0; i < files.Length; i++)
			{
				_rdMap[links[i].Replace('\\', '/')] = files[i];
			}
		}

		private List<StaticResourceDefinition> GetListForKey(string staticResourceKey)
		{
			return _map.FindOrCreate(staticResourceKey, () => new List<StaticResourceDefinition>());
		}

		public enum ResourcePrecedence : int
		{
			Local = 0,
			Library,
			System,
		}

		public class StaticResourceDefinition
		{
			public StaticResourceDefinition(string staticResourceKey, string ns, ResourcePrecedence precedence)
			{
				Name = staticResourceKey;
				Namespace = ns;
				Precedence = precedence;
			}

			public string Name { get; }

			public string Namespace { get; }

			public ResourcePrecedence Precedence { get; }
		}
	}
}
