#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.UI.Input
{
	#if false || false || false || false || false || false || false
	[global::Uno.NotImplemented]
	#endif
	public  partial class HoldingEventArgs 
	{
		// Skipping already declared property HoldingState
		// Skipping already declared property PointerDeviceType
		// Skipping already declared property Position
		#if false
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  uint ContactCount
		{
			get
			{
				throw new global::System.NotImplementedException("The member uint HoldingEventArgs.ContactCount is not implemented in Uno.");
			}
		}
		#endif
		#if false
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  uint CurrentContactCount
		{
			get
			{
				throw new global::System.NotImplementedException("The member uint HoldingEventArgs.CurrentContactCount is not implemented in Uno.");
			}
		}
		#endif
		// Forced skipping of method Windows.UI.Input.HoldingEventArgs.PointerDeviceType.get
		// Forced skipping of method Windows.UI.Input.HoldingEventArgs.Position.get
		// Forced skipping of method Windows.UI.Input.HoldingEventArgs.HoldingState.get
		// Forced skipping of method Windows.UI.Input.HoldingEventArgs.ContactCount.get
		// Forced skipping of method Windows.UI.Input.HoldingEventArgs.CurrentContactCount.get
	}
}
