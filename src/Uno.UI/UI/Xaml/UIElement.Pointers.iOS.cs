﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Foundation;
using UIKit;
using Uno.UI.Extensions;
using WebKit;

namespace Windows.UI.Xaml
{
	partial class UIElement
	{
		private class NativePointerId
		{
			private static readonly Dictionary<IntPtr, NativePointerId> _instances = new Dictionary<IntPtr, NativePointerId>();
			private static uint _nextAvailablePointerId;

			private readonly IntPtr _nativeId;
			private readonly HashSet<UIElement> _leases = new HashSet<UIElement>();

			public uint Value { get; }

			private NativePointerId(IntPtr nativeId)
			{
				_nativeId = nativeId;
				Value = _nextAvailablePointerId++;
			}

			public static NativePointerId Get(UIElement element, UITouch touch)
			{
				if (!_instances.TryGetValue(touch.Handle, out var id))
				{
					_instances[touch.Handle] = id = new NativePointerId(touch.Handle);
				}

				id._leases.Add(element);

				return id;
			}

			public void Release(UIElement element)
			{
				if (_leases.Remove(element) && _leases.Count == 0)
				{
					if (_instances.Remove(_nativeId) && _instances.Count == 0)
					{
						// When all pointers are released, we reset the pointer ID to 0.
						// This is required to detect a DoubleTap where pointer ID must be the same.
						_nextAvailablePointerId = 0;
					}
				}
			}
		}

		private IEnumerable<TouchesManager> _parentsTouchesManager;
		private bool _isManipulating;

		partial void InitializePointersPartial()
		{
			MultipleTouchEnabled = true;
			RegisterLoadActions(PrepareParentTouchesManagers, ReleaseParentTouchesManager);
		}

		#region Native touch handling (i.e. source of the pointer / gesture events)
		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			if (IsPointersSuspended)
			{
				return; // Will also prevent subsequents events
			}

			/* Note: Here we have a mismatching behavior with UWP, if the events bubble natively we're going to get
					 (with Ctrl_02 is a child of Ctrl_01):
							Ctrl_02: Entered
									 Pressed
							Ctrl_01: Entered
									 Pressed

					While on UWP we will get:
							Ctrl_02: Entered
							Ctrl_01: Entered
							Ctrl_02: Pressed
							Ctrl_01: Pressed

					However, to fix this is would mean that we handle all events in managed code, but this would
					break lots of control (ScrollViewer) and ability to easily integrate an external component.
			*/

			try
			{
				NotifyParentTouchesManagersManipulationStarted();

				var isHandledOrBubblingInManaged = default(bool);
				foreach (UITouch touch in touches)
				{
					var id = NativePointerId.Get(this, touch);
					var args = new PointerRoutedEventArgs(id.Value, touch, evt, this);

					isHandledOrBubblingInManaged |= OnNativePointerEnter(args);
					isHandledOrBubblingInManaged |= OnNativePointerDown(args);
				}

				/*
				 * **** WARNING ****
				 *
				 * If we do not propagate the "TouchesBegan" to the parents, they won't receive the
				 * "TouchesMoved" nor the "TouchesEnded". 
				 *
				 * It means that if a control (like the Button) handles the "Pressed" (or the "Entered")
				 * parent won't receive any touch event. As it doesn't seems to be have any impact in application,
				 * it's accepted as a known limitation.
				 *
				 * There is 2 ways to fix this:
				 *  - Still propagate the native "TouchesBegan" and then in parents control filter out events
				 *	  that was already raised in managed
				 *  - Keep a track in local element that the "TouchesBegan" was handled in managed, and then raise
				 *    all subsequent "Move", "Released" and "Exited" in managed only.
				 */

				if (!isHandledOrBubblingInManaged)
				{
					// Continue native bubbling up of the event
					base.TouchesBegan(touches, evt);
				}
			}
			catch (Exception e)
			{
				Application.Current.RaiseRecoverableUnhandledException(e);
			}
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			try
			{
				var isHandledOrBubblingInManaged = default(bool);
				foreach (UITouch touch in touches)
				{
					var id = NativePointerId.Get(this, touch);
					var args = new PointerRoutedEventArgs(id.Value, touch, evt, this);
					var isPointerOver = touch.IsTouchInView(this);

					// As we don't have enter/exit equivalents on iOS, we have to update the IsOver on each move
					// Note: Entered / Exited are raised *before* the Move (Checked using the args timestamp)
					isHandledOrBubblingInManaged |= OnNativePointerMoveWithOverCheck(args, isPointerOver);
				}

				if (!isHandledOrBubblingInManaged)
				{
					// Continue native bubbling up of the event
					base.TouchesMoved(touches, evt);
				}
			}
			catch (Exception e)
			{
				Application.Current.RaiseRecoverableUnhandledException(e);
			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			try
			{
				var isHandledOrBubblingInManaged = default(bool);
				foreach (UITouch touch in touches)
				{
					var id = NativePointerId.Get(this, touch);
					var args = new PointerRoutedEventArgs(id.Value, touch, evt, this);

					isHandledOrBubblingInManaged |= OnNativePointerUp(args);
					isHandledOrBubblingInManaged |= OnNativePointerExited(args);

					id.Release(this);
				}

				if (!isHandledOrBubblingInManaged)
				{
					// Continue native bubbling up of the event
					base.TouchesEnded(touches, evt);
				}

				NotifyParentTouchesManagersManipulationEnded();
			}
			catch (Exception e)
			{
				Application.Current.RaiseRecoverableUnhandledException(e);
			}
		}

		public override void TouchesCancelled(NSSet touches, UIEvent evt)
		{
			try
			{
				var isHandledOrBubblingInManaged = default(bool);
				foreach (UITouch touch in touches)
				{
					var id = NativePointerId.Get(this, touch);
					var args = new PointerRoutedEventArgs(id.Value, touch, evt, this);

					// Note: We should have raise either PointerCaptureLost or PointerCancelled here depending of the reason which
					//		 drives the system to bubble a lost. However we don't have this kind of information on iOS, and it's
					//		 usually due to the ScrollView which kicks in. So we always raise the CaptureLost which is the behavior
					//		 on UWP when scroll starts (even if no capture are actives at this time).

					isHandledOrBubblingInManaged |= OnNativePointerCancel(args, isSwallowedBySystem: true);

					id.Release(this);
				}

				if (!isHandledOrBubblingInManaged)
				{
					// Continue native bubbling up of the event
					base.TouchesCancelled(touches, evt);
				}

				NotifyParentTouchesManagersManipulationEnded();
			}
			catch (Exception e)
			{
				Application.Current.RaiseRecoverableUnhandledException(e);
			}
		}
		#endregion

		#region TouchesManager (Alter the parents native scroll view to make sure to receive all touches)
		partial void OnManipulationModeChanged(ManipulationModes _, ManipulationModes newMode)
			// As we have to walk the tree and this method may be invoked too early, we don't try to track the state between the old and the new mode
			=> PrepareParentTouchesManagers(newMode);

		// Loaded
		private void PrepareParentTouchesManagers() => PrepareParentTouchesManagers(ManipulationMode);
		private void PrepareParentTouchesManagers(ManipulationModes mode)
		{
			// 1. Make sure to end any pending manipulation
			ReleaseParentTouchesManager();

			// 2. If this control can  Walk the tree to detect all ScrollView and register our self as a manipulation listener
			if (mode == ManipulationModes.None)
			{
				_parentsTouchesManager = TouchesManager.GetAllParents(this).ToList();

				foreach (var manager in _parentsTouchesManager)
				{
					manager.RegisterChildListener();
				}
			}
		}

		// Unloaded
		private void ReleaseParentTouchesManager()
		{
			// 1. Make sure to end any pending manipulation
			NotifyParentTouchesManagersManipulationEnded();

			// 2. Un-register our self (so the SV can re-enable the delay)
			if (_parentsTouchesManager != null)
			{
				foreach (var manager in _parentsTouchesManager)
				{
					manager.UnRegisterChildListener();
				}

				_parentsTouchesManager = null; // prevent leak and disable manipulation started/ended reports
			}
		}

		private void NotifyParentTouchesManagersManipulationStarted()
		{
			if (!_isManipulating && (_parentsTouchesManager?.Any() ?? false))
			{
				_isManipulating = true;
				foreach (var manager in _parentsTouchesManager)
				{
					manager.ManipulationStarted();
				}
			}
		}

		private void NotifyParentTouchesManagersManipulationEnded()
		{
			if (_isManipulating && (_parentsTouchesManager?.Any() ?? false))
			{
				_isManipulating = false;
				foreach (var manager in _parentsTouchesManager)
				{
					manager.ManipulationEnded();
				}
			}
		}

		/// <summary>
		/// By default the UIScrollView will delay the touches to the content until it detects
		/// if the manipulation is a drag.And even there, if it detects that the manipulation
		///	* is a Drag, it will cancel the touches on content and handle them internally
		/// (i.e.Touches[Began|Moved|Ended] will no longer be invoked on SubViews).
		/// cf.https://developer.apple.com/documentation/uikit/uiscrollview
		///
		/// The "TouchesManager" give the ability to any child UIElement to alter this behavior
		///	if it needs to handle the gestures itself (e.g.the Thumb of a Slider / ToggleSwitch).
		/// 
		/// On the UIElement this is defined by the ManipulationMode
		/// </summary>
		internal abstract class TouchesManager
		{
			private static readonly ConditionalWeakTable<UIView, ScrollViewTouchesManager> _scrollViews = new ConditionalWeakTable<UIView, ScrollViewTouchesManager>();

			/// <summary>
			/// Gets the current <see cref="TouchesManager"/> for the given view, or create one if possible,
			/// or throw an exception if this type of view does not support touches manager.
			/// </summary>
			public static TouchesManager GetOrCreate(UIView view)
				=> TryGet(view, out var result)
					? result
					: throw new NotSupportedException($"View {view} does not supports enhanced touches management (this is supported only by scrollable content).");

			/// <summary>
			/// Tries to get the current <see cref="TouchesManager"/> for the given view
			/// </summary>
			public static bool TryGet(UIView view, out TouchesManager manager)
			{
				switch (view)
				{
					case NativeScrollContentPresenter presenter:
						manager = presenter.TouchesManager;
						return true;

					case UIScrollView scrollView:
						manager = _scrollViews.GetValue(scrollView, sv => new ScrollViewTouchesManager((UIScrollView)sv));
						return true;

					case ListViewBase listView:
						manager = listView.NativePanel.TouchesManager;
						return true;

					case UIWebView uiWebView:
						manager = _scrollViews.GetValue(uiWebView.ScrollView, sv => new ScrollViewTouchesManager((UIScrollView)sv));
						return true;

					case WKWebView wkWebView:
						manager = _scrollViews.GetValue(wkWebView.ScrollView, sv => new ScrollViewTouchesManager((UIScrollView)sv));
						return true;

					default:
						manager = default;
						return false;
				}
			}

			/// <summary>
			/// Gets all the <see cref="TouchesManager"/> of the parents hierarchy
			/// </summary>
			public static IEnumerable<TouchesManager> GetAllParents(UIElement element)
			{
				foreach (var parent in GetAllParentViews(element))
				{
					if (TryGet(parent, out var manager))
					{
						yield return manager;
					}
				}
			}

			private static IEnumerable<UIView> GetAllParentViews(UIView current)
			{
				while (current != null)
				{
					// Navigate upward using the managed shadowed visual tree
					using (var parents = current.GetParents().GetEnumerator())
					{
						while (parents.MoveNext())
						{
							if (parents.Current is UIView view)
							{
								yield return current = view;
							}
						}
					}

					// When reaching a UIView, fallback to the native visual tree until the next DependencyObject
					do
					{
						yield return current = current.Superview;
					} while (current != null && !(current is DependencyObject));
				}
			}

			/// <summary>
			/// The number of children that are listening to touches events for manipulations
			/// </summary>
			public int Listeners { get; private set; }

			/// <summary>
			/// The number of children that are currently handling a manipulation
			/// </summary>
			public int ActiveListeners { get; private set; }

			/// <summary>
			/// Notify the owner of this touches manager that a child is listening to touches events for manipulations
			/// (so the owner should disable any delay for touches propagation)
			/// </summary>
			/// <remarks>The caller MUST also call <see cref="UnRegisterChildListener"/> once completed.</remarks>
			public void RegisterChildListener()
			{
				if (Listeners++ == 0)
				{
					SetCanDelay(false);
				}
			}

			/// <summary>
			/// Un-register a child listener
			/// </summary>
			public void UnRegisterChildListener()
			{
				if (--Listeners == 0)
				{
					SetCanDelay(true);
				}
			}

			/// <summary>
			/// Indicates that a child listener has started to track a manipulation
			/// (so the owner should not cancel the touches propagation)
			/// </summary>
			/// <remarks>The caller MUST also call <see cref="ManipulationEnded"/> once completed (or cancelled).</remarks>
			public void ManipulationStarted()
			{
				if (ActiveListeners++ == 0)
				{
					SetCanCancel(false);
				}
			}

			/// <summary>
			/// Indicates the end (success or failure) of a manipulation tracking
			/// </summary>
			public void ManipulationEnded()
			{
				if (--ActiveListeners == 0)
				{
					SetCanCancel(true);
				}
			}

			protected abstract void SetCanDelay(bool canDelay);

			protected abstract void SetCanCancel(bool canCancel);
		}

		private class ScrollViewTouchesManager : TouchesManager
		{
			private readonly UIScrollView _scrollView;

			public ScrollViewTouchesManager(UIScrollView scrollView)
			{
				_scrollView = scrollView;
			}

			/// <inheritdoc />
			protected override void SetCanDelay(bool canDelay)
				=> _scrollView.DelaysContentTouches = canDelay;

			/// <inheritdoc />
			protected override void SetCanCancel(bool canCancel)
				=> _scrollView.CanCancelContentTouches = canCancel;
		}
		#endregion

		#region Capture
		// Pointer capture is not needed on iOS, otherwise we could use ExclusiveTouch = true;
		// partial void CapturePointerNative(Pointer pointer);
		// partial void ReleasePointerNative(Pointer pointer);
		#endregion
	}
}
