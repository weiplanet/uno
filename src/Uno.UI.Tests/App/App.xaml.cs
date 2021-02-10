﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.UI.Tests.App.Views;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Resources;

#if HAS_UNO_WINUI
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
#else
using LaunchActivatedEventArgs = Windows.ApplicationModel.Activation.LaunchActivatedEventArgs;
#endif

namespace UnitTestsApp
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	sealed partial class App : Application
	{
		public Grid HostView { get; private set; }

		public App()
		{
			CustomXamlResourceLoader.Current = new MyResourceLoader();
			this.InitializeComponent();
		}

		protected
#if !NETFX_CORE
			internal
#endif
			override void OnLaunched(LaunchActivatedEventArgs args)
		{
			if (HostView == null)
			{
				HostView = new Grid() { Name = "HostView" };

				Window.Current.Content = HostView;

				Window.Current.Activate();
			}

			OnLaunchedPartial();
		}

		partial void OnLaunchedPartial();

		/// <summary>
		/// Ensure that application exists, for unit tests. 
		/// </summary>
		/// <returns>The 'running' application.</returns>
		public static App EnsureApplication()
		{
			if (Current == null)
			{
				var application = new App();
#if !NETFX_CORE
				application.InitializationCompleted();
#endif
				application.OnLaunched(null);
			}

			var app = Current as App;
			app.HostView.Children.Clear();

#if !NETFX_CORE
			//Clear custom theme
			Uno.UI.ApplicationHelper.RequestedCustomTheme = null;
#endif

			return app;
		}
	}
}
