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

namespace Windows.UI.Xaml
{
	public partial class UIElement : DependencyObject
	{
		private PointerRoutedEventArgs PayloadToPressedPointerArgs(string payload) => PayloadToPointerArgs(payload, isInContact: true, pressed: true);
		private PointerRoutedEventArgs PayloadToMovedPointerArgs(string payload) => PayloadToPointerArgs(payload, isInContact: true);
		private PointerRoutedEventArgs PayloadToReleasedPointerArgs(string payload) => PayloadToPointerArgs(payload, isInContact: true, pressed: false);
		private PointerRoutedEventArgs PayloadToEnteredPointerArgs(string payload) => PayloadToPointerArgs(payload, isInContact: false);
		private PointerRoutedEventArgs PayloadToExitedPointerArgs(string payload) => PayloadToPointerArgs(payload, isInContact: false);

		private PointerRoutedEventArgs PayloadToPointerArgs(string payload, bool isInContact, bool? pressed = null)
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
				this);
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
