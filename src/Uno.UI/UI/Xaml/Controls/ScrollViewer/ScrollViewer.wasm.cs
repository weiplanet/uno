﻿#nullable enable
using System.Collections.Generic;
using Windows.Foundation;
using Microsoft.Extensions.Logging;
using Uno.Logging;
using Uno.UI.Xaml;
using Uno.UI;

namespace Windows.UI.Xaml.Controls
{
	partial class ScrollViewer : ICustomClippingElement
	{
		internal Size ScrollBarSize => (_presenter as ScrollContentPresenter)?.ScrollBarSize ?? default;

		private void UpdateZoomedContentAlignment()
		{
		}

		// Disable clipping for Scrollviewer (edge seems to disable scrolling if
		// the clipping is enabled to the size of the scrollviewer, even if overflow-y is auto)
		bool ICustomClippingElement.AllowClippingToLayoutSlot => true;
		bool ICustomClippingElement.ForceClippingToLayoutSlot => true;

		private bool ChangeViewNative(double? horizontalOffset, double? verticalOffset, float? zoomFactor, bool disableAnimation)
		{
			if (zoomFactor.HasValue)
			{
				_log.Warn("ZoomFactor not supported yet on WASM target.");
			}

			if (_presenter != null)
			{
				_presenter.ScrollTo(horizontalOffset, verticalOffset, disableAnimation);
				return true;
			}
			if (_log.IsEnabled(LogLevel.Warning))
			{
				_log.Warn("Cannot ChangeView as ScrollContentPresenter is not ready yet.");
			}
			return false;
		}

		partial void UpdatePartial(bool isIntermediate)
		{
			if (FeatureConfiguration.UIElement.AssignDOMXamlProperties)
			{
				UpdateDOMXamlProperty(nameof(HorizontalOffset), HorizontalOffset);
				UpdateDOMXamlProperty(nameof(VerticalOffset), VerticalOffset);
			}
		}

		/// <summary>
		/// Trim excess scroll, which can be present if the content size is reduced.
		/// </summary>
		partial void TrimOverscroll(Orientation orientation)
		{
			if (_presenter is ContentPresenter presenter && presenter.Content is FrameworkElement presenterContent)
			{
				var presenterViewportSize = GetActualExtent(presenter, orientation);
				var contentExtent = GetActualExtent(presenterContent, orientation);
				var offset = GetOffsetForOrientation(orientation);
				var viewportEnd = offset + presenterViewportSize;
				var overscroll = contentExtent - viewportEnd;
				if (offset > 0 && overscroll < -0.5)
				{
					ChangeViewForOrientation(orientation, overscroll);
				}
			}
		}

		private double GetOffsetForOrientation(Orientation orientation)
			=> orientation == Orientation.Horizontal ? HorizontalOffset : VerticalOffset;

		private void ChangeViewForOrientation(Orientation orientation, double scrollAdjustment)
		{
			if (orientation == Orientation.Vertical)
			{
				ChangeView(null, VerticalOffset + scrollAdjustment, null, disableAnimation: true);
			}
			else
			{
				ChangeView(HorizontalOffset + scrollAdjustment, null, null, disableAnimation: true);
			}
		}

		private static double GetActualExtent(FrameworkElement element, Orientation orientation)
			=> orientation == Orientation.Horizontal ? element.ActualWidth : element.ActualHeight;
	}
}
