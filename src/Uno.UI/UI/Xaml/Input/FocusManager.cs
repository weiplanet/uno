﻿using Windows.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Uno.Extensions;
using Uno.UI.Extensions;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Microsoft.Extensions.Logging;

namespace Windows.UI.Xaml.Input
{
	public sealed partial class FocusManager
	{
		private static readonly Lazy<ILogger> _log = new Lazy<ILogger>(() => typeof(FocusManager).Log());

		private static object _focusedElement;

		/// <summary>
		/// Get the currently focused element, if any
		/// </summary>
		/// <returns>null means nothing is focused.</returns>
		public static object GetFocusedElement() => _focusedElement;

		/// <summary>
		/// Attempts to change focus from the element with focus to the next focusable element in the specified direction.
		/// </summary>
		/// <param name="focusNavigationDirection">The direction to traverse.</param>
		/// <returns>true if focus moved; otherwise, false.</returns>
		/// <remarks>The tab order is the order in which a user moves from one control to another by pressing the Tab key (forward) or Shift+Tab (backward).
		/// This method uses tab order sequence and behavior to traverse all focusable elements in the UI.
		/// If the focus is on the first element in the tab order and FocusNavigationDirection.Previous is specified, focus moves to the last element.
		/// If the focus is on the last element in the tab order and FocusNavigationDirection.Next is specified, focus moves to the first element.
		/// Other directions are not supported on all platforms.
		/// </remarks>
		public static bool TryMoveFocus(FocusNavigationDirection focusNavigationDirection)
		{
			return InnerTryMoveFocus(focusNavigationDirection);
		}

		/// <summary>
		/// Gets the next focusable UIElement depending on focusnavigationdirection, or null if no focusable elements are available.
		/// </summary>
		/// <param name="focusNavigationDirection"></param>
		/// <returns>Next focusable view depending on FocusNavigationDirection</returns>
		public static UIElement FindNextFocusableElement(FocusNavigationDirection focusNavigationDirection)
		{
			return InnerFindNextFocusableElement(focusNavigationDirection) as UIElement;
		}

		/// <summary>
		/// Retrieves the last element that can receive focus based on the specified scope.
		/// </summary>
		/// <param name="searchScope">The root object from which to search. If null, the search scope is the current window.</param>
		/// <returns>The first focusable object.</returns>
		public static DependencyObject FindFirstFocusableElement(DependencyObject searchScope)
		{
			return InnerFindFirstFocusableElement(searchScope);
		}

		/// <summary>
		/// Retrieves the last element that can receive focus based on the specified scope.
		/// </summary>
		/// <param name="searchScope">The root object from which to search. If null, the search scope is the current window.</param>
		/// <returns>The first focusable object.</returns>
		public static DependencyObject FindLastFocusableElement(DependencyObject searchScope)
		{
			return InnerFindLastFocusableElement(searchScope);
		}

		internal static bool SetFocusedElement(DependencyObject newFocus, FocusNavigationDirection focusNavigationDirection, FocusState focusState)
		{
			var control = newFocus as Control; // For now only called for Control
			if (!control.IsFocusable)
			{
				control = control.FindFirstChild<Control>(c => c.IsFocusable);
			}

			if (control == null)
			{
				return false;
			}

			return UpdateFocus(control, focusNavigationDirection, focusState);
		}

		private static bool UpdateFocus(DependencyObject newFocus, FocusNavigationDirection focusNavigationDirection, FocusState focusState)
		{
			// TODO: check AllowFocusOnInteraction
			if (_log.Value.IsEnabled(LogLevel.Debug))
			{
				_log.Value.LogDebug($"{nameof(UpdateFocus)}()- oldFocus={_focusedElement}, newFocus={newFocus}, oldFocus.FocusState={(_focusedElement as Control)?.FocusState}, focusState={focusState}");
			}

			if (newFocus == _focusedElement)
			{
				var newFocusAsControl = newFocus as Control;
				if (newFocusAsControl != null && newFocusAsControl.FocusState != focusState)
				{
					// We do not raise GettingFocus here since the OldFocusedElement and NewFocusedElement
					// would be the same element.
					RaiseGotFocusEvent(_focusedElement);

					// Make sure the FocusState is up-to-date.
					(newFocus as Control)?.UpdateFocusState(focusState);
				}
				// No change in focus element - can skip the rest of this method.
				return true;
			}

			//TODO: RaiseAndProcessGettingAndLosingFocusEvents

			var oldFocusedElement = _focusedElement;

			(oldFocusedElement as Control)?.UpdateFocusState(FocusState.Unfocused); // Set previous unfocused

			// Update the focused control
			_focusedElement = newFocus;

			(newFocus as Control)?.UpdateFocusState(focusState);

			FocusNative(newFocus as Control);

			if (oldFocusedElement != null)
			{
				RaiseLostFocusEvent(oldFocusedElement);
			}

			if (_focusedElement != null)
			{
				RaiseGotFocusEvent(_focusedElement);
			}

			return true;
		}

		private static void RaiseLostFocusEvent(object oldFocus)
		{
			void OnLostFocus()
			{
				if (oldFocus is UIElement uiElement)
				{
					uiElement.RaiseEvent(UIElement.LostFocusEvent, new RoutedEventArgs(uiElement));
				}

				// we replay all "lost focus" events
				LostFocus?.Invoke(null, new FocusManagerLostFocusEventArgs { OldFocusedElement = oldFocus as DependencyObject });
			}

			CoreDispatcher.Main.RunAsync(CoreDispatcherPriority.Normal, OnLostFocus); // event is rescheduled, as on UWP
		}

		private static void RaiseGotFocusEvent(object newFocus)
		{
			void OnGotFocus()
			{
				if (newFocus is UIElement uiElement)
				{
					uiElement.RaiseEvent(UIElement.GotFocusEvent, new RoutedEventArgs(uiElement));
				}

				GotFocus?.Invoke(null, new FocusManagerGotFocusEventArgs { NewFocusedElement = newFocus as DependencyObject });
			}

			CoreDispatcher.Main.RunAsync(CoreDispatcherPriority.Normal, OnGotFocus); // event is rescheduled, as on UWP
		}

		public static event EventHandler<FocusManagerGotFocusEventArgs> GotFocus;
		public static event EventHandler<FocusManagerLostFocusEventArgs> LostFocus;

	}
}
