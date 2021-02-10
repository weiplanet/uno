using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using UIKit;
using Uno.Disposables;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Uno.Extensions;
using Windows.UI;

namespace Uno.UI.Controls
{
	internal partial class CommandBarRenderer : Renderer<CommandBar, UINavigationBar>
	{
		private static DependencyProperty BackButtonForegroundProperty = ToolkitHelper.GetProperty("Uno.UI.Toolkit.CommandBarExtensions", "BackButtonForeground");
		private static DependencyProperty BackButtonIconProperty = ToolkitHelper.GetProperty("Uno.UI.Toolkit.CommandBarExtensions", "BackButtonIcon");

		public CommandBarRenderer(CommandBar element) : base(element) { }

		protected override UINavigationBar CreateNativeInstance()
		{
			var navigationBar = new UINavigationBar();
			var navigationItem = Element.GetRenderer(() => new CommandBarNavigationItemRenderer(Element)).Native;
			navigationBar.PushNavigationItem(navigationItem, false);

			return navigationBar;
		}

		protected override IEnumerable<IDisposable> Initialize()
		{
			yield return Element.RegisterDisposableNestedPropertyChangedCallback(
				(s, e) => Invalidate(),
				new[] { CommandBar.VisibilityProperty },
				new[] { CommandBar.PrimaryCommandsProperty },
				new[] { CommandBar.ContentProperty },
				new[] { CommandBar.ForegroundProperty },
				new[] { CommandBar.ForegroundProperty, SolidColorBrush.ColorProperty },
				new[] { CommandBar.ForegroundProperty, SolidColorBrush.OpacityProperty },
				new[] { CommandBar.BackgroundProperty },
				new[] { CommandBar.BackgroundProperty, SolidColorBrush.ColorProperty },
				new[] { CommandBar.BackgroundProperty, SolidColorBrush.OpacityProperty },
				new[] { BackButtonForegroundProperty },
				new[] { BackButtonIconProperty }
			);

			if (Native is UnoNavigationBar unoNavigationBar)
			{
				unoNavigationBar.SizeChanged += Invalidate;

				yield return Disposable.Create(() =>
					unoNavigationBar.SizeChanged -= Invalidate
				);
			}
		}

		protected override void Render()
		{
			ApplyVisibility();

			// Foreground
			if (Brush.TryGetColorWithOpacity(Element.Foreground, out var foregroundColor))
			{
				Native.TitleTextAttributes = new UIStringAttributes
				{
					ForegroundColor = foregroundColor,
				};
			}
			else
			{
				Native.TitleTextAttributes = null;
			}

			// Background
			var backgroundColor = Brush.GetColorWithOpacity(Element.Background);
			switch (backgroundColor)
			{
				case { } opaqueColor when opaqueColor.A == byte.MaxValue:
					// Prefer BarTintColor because it supports smooth transitions
					Native.BarTintColor = opaqueColor;
					Native.Translucent = false; //Make fully opaque for consistency with SetBackgroundImage
					Native.SetBackgroundImage(null, UIBarMetrics.Default);
					Native.ShadowImage = null;
					break;
				case { } semiTransparentColor when semiTransparentColor.A > 0:
					Native.BarTintColor = null;
					// Use SetBackgroundImage as hack to support semi-transparent background
					Native.SetBackgroundImage(((UIColor)semiTransparentColor).ToUIImage(), UIBarMetrics.Default);
					Native.Translucent = true;
					Native.ShadowImage = null;
					break;
				case { } transparent when transparent.A == 0:
					Native.BarTintColor = null;
					Native.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
					// We make sure a transparent bar doesn't cast a shadow.
					Native.ShadowImage = new UIImage(); // Removes the default 1px line
					Native.Translucent = true;
					break;
				default: //Background is null
					Native.BarTintColor = null;
					Native.SetBackgroundImage(null, UIBarMetrics.Default); // Restores the default blurry background
					Native.ShadowImage = null; // Restores the default 1px line
					Native.Translucent = true;
					break;
			}

			// CommandBarExtensions.BackButtonForeground
			var backButtonForeground = Brush.GetColorWithOpacity(Element.GetValue(BackButtonForegroundProperty) as Brush);
			Native.TintColor = backButtonForeground;

			// CommandBarExtensions.BackButtonIcon
			var backButtonIcon = Element.GetValue(BackButtonIconProperty) is BitmapIcon bitmapIcon
				? UIImageHelper.FromUri(bitmapIcon.UriSource)?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal)
				: null;

			Native.BackIndicatorImage = backButtonIcon;
			Native.BackIndicatorTransitionMaskImage = backButtonIcon;
		}

		private void ApplyVisibility()
		{
			var newHidden = Element.Visibility == Visibility.Collapsed;
			var hasChanged = Native.Hidden != newHidden;
			Native.Hidden = newHidden;
			if (hasChanged)
			{
				// Re-layout UINavigationBar when visibility changes, this is important eg in the case that status bar was shown/hidden
				// while CommandBar was collapsed
				Native.SetNeedsLayout();
				Native.Superview?.SetNeedsLayout();
			}
		}
	}
}
