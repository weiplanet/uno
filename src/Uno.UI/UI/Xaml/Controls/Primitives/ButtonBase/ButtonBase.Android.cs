﻿using System;
using Android.Views;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Logging;
using Uno.UI;
using Windows.UI.Xaml.Input;
using Android.Runtime;
using Java.Interop;

namespace Windows.UI.Xaml.Controls.Primitives
{
	public partial class ButtonBase : ContentControl
	{
		private readonly SerialDisposable _touchSubscription = new SerialDisposable();
		private readonly SerialDisposable _isEnabledSubscription = new SerialDisposable();

		partial void PartialInitializeProperties()
		{
			// need the Tapped event to be registered for "Click" to work properly
			Tapped += (snd, evt) => { };
		}

		private protected override void OnLoaded()
		{
			base.OnLoaded();

			Focusable = true;
			FocusableInTouchMode = true;

			RegisterEvents();

			OnCanExecuteChanged();
		}

		private protected override void OnUnloaded()
		{
			base.OnUnloaded();
			_isEnabledSubscription.Disposable = null;
		}

		partial void OnIsEnabledChangedPartial(bool oldValue, bool newValue)
		{
			Clickable = newValue;
		}

		partial void RegisterEvents()
		{
			_touchSubscription.Disposable = null;

			View uiControl = GetUIControl();

			var nativeButton = uiControl as Android.Widget.Button;
			if (nativeButton is Android.Widget.Button)
			{
				_isEnabledSubscription.Disposable =
					DependencyObjectExtensions.RegisterDisposablePropertyChangedCallback(
						this,
						IsEnabledProperty,
						(s, e) => uiControl.Enabled = IsEnabled
					);

				uiControl.Enabled = IsEnabled;
			}
			else if (uiControl != null)
			{
			}
			else
			{
				if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
				{
					this.Log().WarnFormat("ControlTemplateRoot is not available, {0} will not be clickable", this.GetType());
				}
			}
		}

		/// <summary>
		/// Gets the native UI Control, if any.
		/// </summary>
		private View GetUIControl()
		{
			return
				// Check for non-templated ContentControl root (ContentPresenter bypass)
				ContentTemplateRoot

				// Then check for complex templated controls (where the native button is not at the root)
				?? (TemplatedRoot as ViewGroup)
					?.FindFirstChild<ContentPresenter>()
					?.ContentTemplateRoot

				// Finally check for templated ContentControl root
				?? TemplatedRoot as View
				;
		}
	}
}
