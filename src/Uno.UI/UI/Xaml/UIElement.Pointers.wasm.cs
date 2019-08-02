using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using Uno;
using Uno.Extensions;
using Uno.Foundation;
using Uno.Logging;
using Uno.UI.Xaml.Input;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.System;
using Uno.Collections;
using Uno.UI;
using System.Numerics;
using Windows.UI.Input;
using Uno.UI.Xaml;

namespace Windows.UI.Xaml
{
	public partial class UIElement : DependencyObject
	{
		// Ref:
		// https://www.w3.org/TR/pointerevents/
		// https://developer.mozilla.org/en-US/docs/Web/API/PointerEvent

		private static readonly Dictionary<RoutedEvent, (string domEventName, EventArgsParser argsParser, RoutedEventHandlerWithHandled handler)> _pointerHandlers
			= new Dictionary<RoutedEvent, (string, EventArgsParser, RoutedEventHandlerWithHandled)>
			{
				{PointerEnteredEvent, ("pointerover", PayloadToEnteredPointerArgs, (snd, args) => ((UIElement)snd).OnNativePointerEnter((PointerRoutedEventArgs)args))},
				{PointerPressedEvent, ("pointerdown", PayloadToPressedPointerArgs, (snd, args) => ((UIElement)snd).OnNativePointerDown((PointerRoutedEventArgs)args))},
				{PointerMovedEvent, ("pointermove", PayloadToMovedPointerArgs, (snd, args) => ((UIElement)snd).OnNativePointerMove((PointerRoutedEventArgs)args))},
				{PointerReleasedEvent, ("pointerup", PayloadToReleasedPointerArgs, (snd, args) => ((UIElement)snd).OnNativePointerUp((PointerRoutedEventArgs)args))},
				{PointerExitedEvent, ("pointerout", PayloadToExitedPointerArgs, (snd, args) => ((UIElement)snd).OnNativePointerExited((PointerRoutedEventArgs)args))},
				{PointerCanceledEvent, ("pointercancel", PayloadToCancelledPointerArgs, (snd, args) => ((UIElement)snd).OnNativePointerCancel((PointerRoutedEventArgs)args, isSwallowedBySystem: true))}, //https://www.w3.org/TR/pointerevents/#the-pointercancel-event
			};

		partial void OnGestureRecognizerInitialized(GestureRecognizer recognizer)
		{
			// When a gesture recognizer is initialized, we subscribe to pointer events in order to feed it.
			// Note: We subscribe to * all * pointer events in order to maintain a logical internal state of pointers over / press / capture

			foreach (var pointerEvent in _pointerHandlers.Keys)
			{
				AddPointerHandler(pointerEvent, 1, default, default);
			}
		}

		partial void AddPointerHandler(RoutedEvent routedEvent, int handlersCount, object handler, bool handledEventsToo)
		{
			if (handlersCount != 1
				// We do not remove event handlers for now, so do not rely only on the handlersCount and keep track of registered events
				|| _registeredRoutedEvents.HasFlag(routedEvent.Flag))
			{
				return;
			}
			_registeredRoutedEvents |= routedEvent.Flag;

			if (!_pointerHandlers.TryGetValue(routedEvent, out var evt))
			{
				Application.Current.RaiseRecoverableUnhandledException(new NotImplementedException($"Pointer event {routedEvent.Name} is not supported on this platform"));
				return;
			}

			RegisterEventHandler(
				evt.domEventName,
				handler: evt.handler,
				onCapturePhase: false,
				canBubbleNatively: true,
				eventFilter: HtmlEventFilter.Default,
				eventExtractor: HtmlEventExtractor.PointerEventExtractor,
				payloadConverter: evt.argsParser
			);
		}

		private static PointerRoutedEventArgs PayloadToEnteredPointerArgs(object snd, string payload) => PayloadToPointerArgs(snd, payload, isInContact: false);
		private static PointerRoutedEventArgs PayloadToPressedPointerArgs(object snd, string payload) => PayloadToPointerArgs(snd, payload, isInContact: true, pressed: true);
		private static PointerRoutedEventArgs PayloadToMovedPointerArgs(object snd, string payload) => PayloadToPointerArgs(snd, payload, isInContact: true);
		private static PointerRoutedEventArgs PayloadToReleasedPointerArgs(object snd, string payload) => PayloadToPointerArgs(snd, payload, isInContact: true, pressed: false);
		private static PointerRoutedEventArgs PayloadToExitedPointerArgs(object snd, string payload) => PayloadToPointerArgs(snd, payload, isInContact: false);
		private static PointerRoutedEventArgs PayloadToCancelledPointerArgs(object snd, string payload) => PayloadToPointerArgs(snd, payload, isInContact: false, pressed: false);

		private static PointerRoutedEventArgs PayloadToPointerArgs(object snd, string payload, bool isInContact, bool? pressed = null)
		{
			var parts = payload?.Split(';');
			if (parts?.Length != 7)
			{
				return null;
			}

			var pointerId = uint.Parse(parts[0], CultureInfo.InvariantCulture);
			var x = double.Parse(parts[1], CultureInfo.InvariantCulture);
			var y = double.Parse(parts[2], CultureInfo.InvariantCulture);
			var ctrl = parts[3] == "1";
			var shift = parts[4] == "1";
			var button = int.Parse(parts[5], CultureInfo.InvariantCulture); // -1: none, 0:main, 1:middle, 2:other (commonly main=left, other=right)
			var typeStr = parts[6];

			var position = new Point(x, y);
			var pointerType = ConvertPointerTypeString(typeStr);
			var key =
				button == 0 ? VirtualKey.LeftButton
				: button == 1 ? VirtualKey.MiddleButton
				: button == 2 ? VirtualKey.RightButton
				: VirtualKey.None; // includes -1 == none
			var keyModifiers = VirtualKeyModifiers.None;
			if (ctrl) keyModifiers |= VirtualKeyModifiers.Control;
			if (shift) keyModifiers |= VirtualKeyModifiers.Shift;
			var update = PointerUpdateKind.Other;
			if (pressed.HasValue)
			{
				if (pressed.Value)
				{
					update = key == VirtualKey.LeftButton ? PointerUpdateKind.LeftButtonPressed
						: key == VirtualKey.MiddleButton ? PointerUpdateKind.MiddleButtonPressed
						: key == VirtualKey.RightButton ? PointerUpdateKind.RightButtonPressed
						: PointerUpdateKind.Other;
				}
				else
				{
					update = key == VirtualKey.LeftButton ? PointerUpdateKind.LeftButtonReleased
						: key == VirtualKey.MiddleButton ? PointerUpdateKind.MiddleButtonReleased
						: key == VirtualKey.RightButton ? PointerUpdateKind.RightButtonReleased
						: PointerUpdateKind.Other;
				}
			}

			return new PointerRoutedEventArgs(
				pointerId,
				pointerType,
				position,
				isInContact,
				key,
				keyModifiers,
				update,
				(UIElement)snd);
		}

		private static PointerDeviceType ConvertPointerTypeString(string typeStr)
		{
			PointerDeviceType type;
			switch (typeStr.ToUpper())
			{
				case "MOUSE":
				default:
					type = PointerDeviceType.Mouse;
					break;
				case "PEN":
					type = PointerDeviceType.Pen;
					break;
				case "TOUCH":
					type = PointerDeviceType.Touch;
					break;
			}

			return type;
		}

		private void CapturePointerNative(Pointer pointer)
		{
			var command = "Uno.UI.WindowManager.current.setPointerCapture(" + HtmlId + ", " + pointer.PointerId + ");";
			WebAssemblyRuntime.InvokeJS(command);
		}

		private void ReleasePointerCaptureNative(Pointer pointer)
		{
			var command = "Uno.UI.WindowManager.current.releasePointerCapture(" + HtmlId + ", " + pointer.PointerId + ");";
			WebAssemblyRuntime.InvokeJS(command);
		}
	}
}
