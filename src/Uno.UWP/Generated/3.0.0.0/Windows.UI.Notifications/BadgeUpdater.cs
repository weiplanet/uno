#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.UI.Notifications
{
	#if __ANDROID__ || false || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || false
	[global::Uno.NotImplemented]
	#endif
	public  partial class BadgeUpdater 
	{
		#if __ANDROID__ || false || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || false
		[global::Uno.NotImplemented("__ANDROID__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__")]
		public  void Update( global::Windows.UI.Notifications.BadgeNotification notification)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Notifications.BadgeUpdater", "void BadgeUpdater.Update(BadgeNotification notification)");
		}
		#endif
		#if __ANDROID__ || false || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || false
		[global::Uno.NotImplemented("__ANDROID__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__")]
		public  void Clear()
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Notifications.BadgeUpdater", "void BadgeUpdater.Clear()");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void StartPeriodicUpdate( global::System.Uri badgeContent,  global::Windows.UI.Notifications.PeriodicUpdateRecurrence requestedInterval)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Notifications.BadgeUpdater", "void BadgeUpdater.StartPeriodicUpdate(Uri badgeContent, PeriodicUpdateRecurrence requestedInterval)");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void StartPeriodicUpdate( global::System.Uri badgeContent,  global::System.DateTimeOffset startTime,  global::Windows.UI.Notifications.PeriodicUpdateRecurrence requestedInterval)
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Notifications.BadgeUpdater", "void BadgeUpdater.StartPeriodicUpdate(Uri badgeContent, DateTimeOffset startTime, PeriodicUpdateRecurrence requestedInterval)");
		}
		#endif
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "NET461", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  void StopPeriodicUpdate()
		{
			global::Windows.Foundation.Metadata.ApiInformation.TryRaiseNotImplemented("Windows.UI.Notifications.BadgeUpdater", "void BadgeUpdater.StopPeriodicUpdate()");
		}
		#endif
	}
}
