﻿using Uno.Extensions;
using Uno.Logging;
using Uno.UI.DataBinding;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Uno.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Drawing;
using AppKit;
using Uno.UI;
using Foundation;
using CoreGraphics;
using Windows.Foundation;

namespace Windows.UI.Xaml.Controls
{
	partial class NativeScrollContentPresenter : NSScrollView, IHasSizeThatFits
	{
		private readonly WeakReference<ScrollViewer> _scrollViewer;

		public NativeScrollContentPresenter(ScrollViewer scroller) : this()
		{
			_scrollViewer = new WeakReference<ScrollViewer>(scroller);
		}

		public NativeScrollContentPresenter()
		{
			Notifications.ObserveDidLiveScroll(this, OnLiveScroll);

			DrawsBackground = false;
		}

		public nfloat ZoomScale 
		{
			get => Magnification;
			set => Magnification = value;
		}

		public NSEdgeInsets ContentInset { get; set; }

		public CGPoint UpperScrollLimit { get; }

		public float MinimumZoomScale { get; set; }

		public float MaximumZoomScale { get; set; }

		private bool ShowsVerticalScrollIndicator
		{
			get => HasVerticalScroller;
			set => HasVerticalScroller = value;
		}

		private bool ShowsHorizontalScrollIndicator
		{
			get => HasHorizontalScroller;
			set => HasHorizontalScroller = value;
		}
		public override bool NeedsLayout
		{
			get => base.NeedsLayout; set
			{
				base.NeedsLayout = value;

				if (value)
				{
					_requiresMeasure = true;

					if (Superview != null)
					{
						Superview.NeedsLayout = true;
					}
				}
			}
		}

		public void SetContentOffset(CGPoint contentOffset, bool animated)
		{
			// Support for ChangeView https://github.com/unoplatform/uno/issues/626
		}

		partial void OnContentChanged(NSView previousView, NSView newView)
		{
			previousView?.SetParent(null);

			DocumentView = newView;

			// This is not needed on iOS/Android because the native ScrollViewer
			// relies on the Children property, not on a `DocumentView` property.
			newView?.SetParent(TemplatedParent);
		}

		private void OnLiveScroll(object sender, NSNotificationEventArgs e)
		{
			var scroller = _scrollViewer.TryGetTarget(out var s) ? s : TemplatedParent as ScrollViewer;
			if (scroller is null)
			{
				return;
			}

			var offset = DocumentVisibleRect.Location;
			scroller.OnScrollInternal(offset.X, offset.Y, isIntermediate: false);
		}

		public Rect MakeVisible(UIElement visual, Rect rectangle) =>
			throw new NotImplementedException("The member Rect ScrollContentPresenter.MakeVisible(UIElement visual, Rect rectangle) is not implemented in Uno.");
	}
}
