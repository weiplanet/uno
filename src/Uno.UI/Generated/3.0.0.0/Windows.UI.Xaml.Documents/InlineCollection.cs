#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.UI.Xaml.Documents
{
	#if false || false || false || false || false || false || false
	[global::Uno.NotImplemented]
	#endif
	public  partial class InlineCollection : global::System.Collections.Generic.IList<global::Windows.UI.Xaml.Documents.Inline>,global::System.Collections.Generic.IEnumerable<global::Windows.UI.Xaml.Documents.Inline>
	{
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  uint Size
		{
			get
			{
				throw new global::System.NotImplementedException("The member uint InlineCollection.Size is not implemented in Uno.");
			}
		}
		#endif
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.GetAt(uint)
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.Size.get
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.GetView()
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.IndexOf(Windows.UI.Xaml.Documents.Inline, out uint)
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.SetAt(uint, Windows.UI.Xaml.Documents.Inline)
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.InsertAt(uint, Windows.UI.Xaml.Documents.Inline)
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.RemoveAt(uint)
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.Append(Windows.UI.Xaml.Documents.Inline)
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.RemoveAtEnd()
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.Clear()
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.GetMany(uint, Windows.UI.Xaml.Documents.Inline[])
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.ReplaceAll(Windows.UI.Xaml.Documents.Inline[])
		// Forced skipping of method Windows.UI.Xaml.Documents.InlineCollection.First()
		// Processing: System.Collections.Generic.IList<Windows.UI.Xaml.Documents.Inline>
		// Skipping already implement System.Collections.Generic.IList<Windows.UI.Xaml.Documents.Inline>.this[int]
		// Processing: System.Collections.Generic.ICollection<Windows.UI.Xaml.Documents.Inline>
		// Skipping already implement System.Collections.Generic.ICollection<Windows.UI.Xaml.Documents.Inline>.Count
		// Skipping already implement System.Collections.Generic.ICollection<Windows.UI.Xaml.Documents.Inline>.IsReadOnly
		// Processing: System.Collections.Generic.IEnumerable<Windows.UI.Xaml.Documents.Inline>
		// Processing: System.Collections.IEnumerable
	}
}
