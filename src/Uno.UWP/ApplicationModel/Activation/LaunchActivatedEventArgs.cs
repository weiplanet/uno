using System;
using Uno;
using Windows.Foundation;
using Windows.Foundation.Metadata;

namespace Windows.ApplicationModel.Activation
{
	public sealed partial class LaunchActivatedEventArgs : IActivatedEventArgs
	{
		internal LaunchActivatedEventArgs()
		{

		}

		internal LaunchActivatedEventArgs(ActivationKind kind, string arguments)
		{
			Arguments = arguments;
			Kind = kind;
		}

#if !__ANDROID__ && !__IOS__
		[NotImplemented]
#endif
		public ActivationKind Kind
		{
			get;
		}

		/// <summary>
		/// Defaults to NotRunning, may not be accurate in all cases for all platforms.
		/// </summary>
		public ApplicationExecutionState PreviousExecutionState { get; }

		[NotImplemented]
		public SplashScreen SplashScreen
		{
			get;
		}

		[NotImplemented]
		public int CurrentlyShownApplicationViewId
		{
			get;
		}

		public string Arguments
		{
			get;
		}

		[NotImplemented]
		public string TileId
		{
			get;
		}

		public bool PrelaunchActivated => false; // No platform other than UWP supports prelaunch yet.
	}
}
