﻿#nullable enable
using System;
using Uno.Extensions;
using Uno.Foundation;
using Uno.Logging;
using Windows.Foundation;
using System.Globalization;
using Uno.Foundation.Extensibility;
using Uno.Disposables;
using Windows.ApplicationModel;

namespace Windows.UI.ViewManagement
{
	partial class ApplicationView : IApplicationViewEvents
	{
		private readonly IApplicationViewExtension _applicationViewExtension;

		public ApplicationView()
		{
			if (!ApiExtensibility.CreateInstance(this, out _applicationViewExtension))
			{
				throw new InvalidOperationException($"Unable to find IApplicationViewExtension extension");
			}
		}

		public string Title
		{
			get => _applicationViewExtension.Title;
			set => _applicationViewExtension.Title = value;
		}

		public bool TryEnterFullScreenMode() => _applicationViewExtension.TryEnterFullScreenMode();

		public void ExitFullScreenMode() => _applicationViewExtension.ExitFullScreenMode();
	}

	internal interface IApplicationViewExtension
	{
		string Title { get; set; }

		bool TryEnterFullScreenMode();

		void ExitFullScreenMode();
	}

	internal interface IApplicationViewEvents
	{

	}
}
