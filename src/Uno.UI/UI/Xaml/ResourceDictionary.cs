using System;
using System.Collections.Generic;
using System.Threading;
using Uno.UI;
using Uno.Extensions;
using System.ComponentModel;
using Uno.UI.Xaml;

namespace Windows.UI.Xaml
{
	public partial class ResourceDictionary : DependencyObject, IDependencyObjectParse, IDictionary<object, object>
	{
		private readonly Dictionary<object, object> _values = new Dictionary<object, object>();

		/// <summary>
		/// If true, there may be lazily-set values in the dictionary that need to be initialized.
		/// </summary>
		private bool _hasUnmaterializedItems = false;

		public ResourceDictionary()
		{
		}

		private Uri _source;
		public Uri Source
		{
			get => _source;
			set
			{
				if (!IsParsing) // If we're parsing, the Source is being set as a 'FYI', don't try to resolve it
				{
					var sourceDictionary = ResourceResolver.RetrieveDictionaryForSource(value);

					CopyFrom(sourceDictionary);
				}

				_source = value;
			}
		}

		public IList<ResourceDictionary> MergedDictionaries { get; } = new List<ResourceDictionary>();

		private ResourceDictionary _themeDictionaries;
		public IDictionary<object, object> ThemeDictionaries { get => _themeDictionaries = _themeDictionaries ?? new ResourceDictionary(); }

		/// <summary>
		/// Is this a ResourceDictionary created from system resources, ie within the Uno.UI assembly?
		/// </summary>
		internal bool IsSystemDictionary { get; set; }

		internal object Lookup(object key)
		{
			if (!TryGetValue(key, out var value))
			{
				return null;
			}

			return value;
		}

		/// <remarks>This method does not exist in C# UWP API
		/// and can be removed as breaking change later.</remarks>
		public bool HasKey(object key) => ContainsKey(key);

		/// <remarks>This method does not exist in C# UWP API
		/// and can be removed as breaking change later.</remarks>
		public bool Insert(object key, object value)
		{
			Set(key, value, throwIfPresent: false);
			return true;
		}

		public bool Remove(object key) => _values.Remove(key);

		public bool Remove(KeyValuePair<object, object> key) => _values.Remove(key.Key);

		public void Clear() => _values.Clear();

		public void Add(object key, object value) => Set(key, value, throwIfPresent: true);

		public bool ContainsKey(object key) => ContainsKey(key, shouldCheckSystem: true);
		public bool ContainsKey(object key, bool shouldCheckSystem) => _values.ContainsKey(key) || ContainsKeyMerged(key) || ContainsKeyTheme(key)
			|| (shouldCheckSystem && !IsSystemDictionary && ResourceResolver.ContainsKeySystem(key));

		public bool TryGetValue(object key, out object value) => TryGetValue(key, out value, shouldCheckSystem: true);
		public bool TryGetValue(object key, out object value, bool shouldCheckSystem)
		{
			if (_values.TryGetValue(key, out value))
			{
				TryMaterializeLazy(key, ref value);
				TryResolveAlias(ref value);
				return true;
			}

			if (GetFromMerged(key, out value))
			{
				return true;
			}

			if (GetFromTheme(key, out value))
			{
				return true;
			}

			if (shouldCheckSystem && !IsSystemDictionary) // We don't fall back on system resources from within a system-defined dictionary, to avoid an infinite recurse
			{
				return ResourceResolver.TrySystemResourceRetrieval(key, out value);
			}

			return false;
		}

		public object this[object key]
		{
			get
			{
				object value;
				TryGetValue(key, out value);

				return value;
			}
			set => Set(key, value, throwIfPresent: false);
		}

		private void Set(object key, object value, bool throwIfPresent)
		{
			if (throwIfPresent && _values.ContainsKey(key))
			{
				throw new ArgumentException("An entry with the same key already exists.");
			}

			if(value is WeakResourceInitializer lazyResourceInitializer)
			{
				value = lazyResourceInitializer.Initializer;
			}

			if (value is ResourceInitializer resourceInitializer)
			{
				_hasUnmaterializedItems = true;
				_values[key] = new LazyInitializer(ResourceResolver.CurrentScope, resourceInitializer);
			}
			else
			{
				_values[key] = value;
			}
		}

		/// <summary>
		/// If retrieved element is a <see cref="LazyInitializer"/> stub, materialize the actual object and replace the stub.
		/// </summary>
		private void TryMaterializeLazy(object key, ref object value)
		{
			if (value is LazyInitializer lazyInitializer)
			{
				object newValue = null;
#if !HAS_EXPENSIVE_TRYFINALLY
				try
#endif
				{
					_values.Remove(key); // Temporarily remove the key to make this method safely reentrant, if it's a framework- or application-level theme dictionary
					ResourceResolver.PushNewScope(lazyInitializer.CurrentScope);
					newValue = lazyInitializer.Initializer();
				}
#if !HAS_EXPENSIVE_TRYFINALLY
				finally
#endif
				{
					value = newValue;
					_values[key] = newValue; // If Initializer threw an exception this will push null, to avoid running buggy initialization again and again (and avoid surfacing initializer to consumer code)
					ResourceResolver.PopScope();
				}
			}
		}

		/// <summary>
		/// If <paramref name="value"/> is a <see cref="StaticResourceAliasRedirect"/>, replace it with the target of ResourceKey, or null if no matching resource is found.
		/// </summary>
		/// <returns>True if <paramref name="value"/> is a <see cref="StaticResourceAliasRedirect"/>, false otherwise</returns>
		private bool TryResolveAlias(ref object value)
		{
			if (value is StaticResourceAliasRedirect alias)
			{
				ResourceResolver.ResolveResourceStatic(alias.ResourceKey, out var resourceKeyTarget, alias.ParseContext);
				value = resourceKeyTarget;
				return true;
			}

			return false;
		}

		private bool GetFromMerged(object key, out object value)
		{
			// Check last dictionary first - //https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/resourcedictionary-and-xaml-resource-references#merged-resource-dictionaries
			for (int i = MergedDictionaries.Count - 1; i >= 0; i--)
			{
				if (MergedDictionaries[i].TryGetValue(key, out value, shouldCheckSystem: false))
				{
					return true;
				}
			}

			value = null;

			return false;
		}

		private bool ContainsKeyMerged(object key)
		{
			for (int i = MergedDictionaries.Count - 1; i >= 0; i--)
			{
				if (MergedDictionaries[i].ContainsKey(key, shouldCheckSystem: false))
				{
					return true;
				}
			}

			return false;
		}

		private ResourceDictionary GetThemeDictionary() => GetThemeDictionary(Themes.Active) ?? GetThemeDictionary(Themes.Default);

		private ResourceDictionary GetThemeDictionary(string theme)
		{
			object dict = null;
			if (_themeDictionaries?.TryGetValue(theme, out dict, shouldCheckSystem: false) ?? false)
			{
				return dict as ResourceDictionary;
			}

			return null;
		}

		private bool GetFromTheme(object key, out object value)
		{
			var dict = GetThemeDictionary();

			if (dict != null && dict.TryGetValue(key, out value, shouldCheckSystem: false))
			{
				return true;
			}

			return GetFromThemeMerged(key, out value);
		}

		private bool GetFromThemeMerged(object key, out object value)
		{
			for (int i = MergedDictionaries.Count - 1; i >= 0; i--)
			{
				if (MergedDictionaries[i].GetFromTheme(key, out value))
				{
					return true;
				}
			}

			value = null;

			return false;
		}


		private bool ContainsKeyTheme(object key)
		{
			return GetThemeDictionary()?.ContainsKey(key, shouldCheckSystem: false) ?? ContainsKeyThemeMerged(key);
		}

		private bool ContainsKeyThemeMerged(object key)
		{
			for (int i = MergedDictionaries.Count - 1; i >= 0; i--)
			{
				if (MergedDictionaries[i].ContainsKeyTheme(key))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Copy another dictionary's contents, this is used when setting the <see cref="Source"/> property
		/// </summary>
		private void CopyFrom(ResourceDictionary source)
		{
			_values.Clear();
			MergedDictionaries.Clear();
			_themeDictionaries?.Clear();

			_values.AddRange(source);
			MergedDictionaries.AddRange(source.MergedDictionaries);
			if (source._themeDictionaries != null)
			{
				ThemeDictionaries.AddRange(source.ThemeDictionaries);
			}
		}

		public global::System.Collections.Generic.ICollection<object> Keys => _values.Keys;

		// TODO: this doesn't handle lazy initializers or aliases
		public global::System.Collections.Generic.ICollection<object> Values => _values.Values;

		public void Add(global::System.Collections.Generic.KeyValuePair<object, object> item) => Add(item.Key, item.Value);

		public bool Contains(global::System.Collections.Generic.KeyValuePair<object, object> item) => _values.ContainsKey(item.Key);

		[Uno.NotImplemented]
		public void CopyTo(global::System.Collections.Generic.KeyValuePair<object, object>[] array, int arrayIndex)
		{
			throw new global::System.NotSupportedException();
		}

		public int Count => _values.Count;

		public bool IsReadOnly => false;

		private bool _isParsing;
		/// <summary>
		/// True if the element is in the process of being parsed from Xaml.
		/// </summary>
		/// <remarks>This property shouldn't be set from user code. It's public to allow being set from generated code.</remarks>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsParsing
		{
			get => _isParsing;
			set
			{
				if (!value)
				{
					throw new InvalidOperationException($"{nameof(IsParsing)} should never be set from user code.");
				}

				_isParsing = value;
				if (_isParsing)
				{
					ResourceResolver.PushSourceToScope(this);
				}
			}
		}

		public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
		{
			TryMaterializeAll();

			foreach (var kvp in _values)
			{
				var aliased = kvp.Value;
				if (TryResolveAlias(ref aliased))
				{
					yield return new KeyValuePair<object, object>(kvp.Key, aliased);
				}
				else
				{
					yield return kvp;
				}
			}
		}

		global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

		/// <summary>
		/// Ensure all lazily-set values are materialized, prior to enumeration.
		/// </summary>
		private void TryMaterializeAll()
		{
			if (!_hasUnmaterializedItems)
			{
				return;
			}

			var unmaterialized = new List<KeyValuePair<object, object>>();

			foreach (var kvp in _values)
			{
				if (kvp.Value is LazyInitializer lazyInitializer)
				{
					unmaterialized.Add(kvp);
				}
			}

			foreach (var kvp in unmaterialized)
			{
				var value = kvp.Value;
				TryMaterializeLazy(kvp.Key, ref value);
			}

			_hasUnmaterializedItems = false;
		}

		public void CreationComplete()
		{
			if (!IsParsing)
			{
				throw new InvalidOperationException($"Called without matching {nameof(IsParsing)} call. This method should never be called from user code.");
			}

			_isParsing = false;
			ResourceResolver.PopSourceFromScope();
		}

		/// <summary>
		/// Update theme bindings on DependencyObjects in the dictionary.
		/// </summary>
		internal void UpdateThemeBindings()
		{
			foreach (var item in _values.Values)
			{
				if (item is IDependencyObjectStoreProvider provider && provider.Store.Parent == null)
				{
					provider.Store.UpdateResourceBindings(isThemeChangedUpdate: true, containingDictionary: this);
				}
			}

			foreach (var mergedDict in MergedDictionaries)
			{
				mergedDict.UpdateThemeBindings();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate object ResourceInitializer();

		/// <summary>
		/// Allows resources to be initialized on-demand using correct scope.
		/// </summary>
		private struct LazyInitializer
		{
			public XamlScope CurrentScope { get; }
			public ResourceInitializer Initializer { get; }

			public LazyInitializer(XamlScope currentScope, ResourceInitializer initializer)
			{
				CurrentScope = currentScope;
				Initializer = initializer;
			}
		}

		/// <summary>
		/// Allows resources set by a StaticResource alias to be resolved with the correct theme at time of resolution (eg in response to the
		/// app theme changing).
		/// </summary>
		private class StaticResourceAliasRedirect
		{
			public StaticResourceAliasRedirect(string resourceKey, XamlParseContext parseContext)
			{
				ResourceKey = resourceKey;
				ParseContext = parseContext;
			}

			public string ResourceKey { get; }
			public XamlParseContext ParseContext { get; }
		}

		internal static object GetStaticResourceAliasPassthrough(string resourceKey, XamlParseContext parseContext) => new StaticResourceAliasRedirect(resourceKey, parseContext);

		private static class Themes
		{
			public const string Default = "Default";
			public static string Active => Application.Current?.RequestedThemeForResources ?? "Light";
		}
	}
}
