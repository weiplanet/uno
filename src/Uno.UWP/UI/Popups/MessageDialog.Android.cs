﻿#if XAMARIN_ANDROID
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Uno.UI;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Windows.UI.Core;
using Windows.Foundation;
using System.Threading;

namespace Windows.UI.Popups
{
	public partial class MessageDialog
	{
		const int MaximumCommands = 3;

		private void ShowInner(CancellationToken ct, TaskCompletionSource<IUICommand> invokedCommand)
		{
			// Android recommends placing buttons in this order:
			//  1) positive (default accept)
			//  2) negative (default cancel)
			//  3) neutral
			// For the moment, we respect instead the order they were added in Commands,
			// just like under Windows.

			var dialog = Commands
				.Where(command => !(command is UICommandSeparator)) // Not supported on Android
				.DefaultIfEmpty(new UICommand("Close")) // TODO: Localize (PBI 28711)
				.Reverse()
				.Select((command, index) =>
					new
					{
						Command = command,
						ButtonType = GetDialogButtonType(index)
					})
				.Aggregate(
					new global::AndroidX.AppCompat.App.AlertDialog.Builder(ContextHelper.Current)
						.SetTitle(Title ?? "")
						.SetMessage(Content ?? "")
						.SetOnCancelListener(new DialogListener(this, invokedCommand))
						.Create(),
					(alertDialog, commandInfo) =>
					{
						alertDialog.SetButton(
							commandInfo.ButtonType,
							commandInfo.Command.Label,
							(_, __) =>
							{
								commandInfo.Command.Invoked?.Invoke(commandInfo.Command);
								invokedCommand.TrySetResult(commandInfo.Command);
							}
						);
						return alertDialog;
					}
				);

			using (ct.Register(() =>
			{
				// If the cancellation token itself gets cancelled, we cancel as well.
				invokedCommand.TrySetCanceled();
				dialog.Dismiss();
			}))
			{
				dialog.Show();
			}
		}

		partial void ValidateCommands()
		{
			// On Android, providing more than 3 commands will skip all but the first two and the last.
			// We intercept this bad situation right away.
			if (this.Commands.Count > MaximumCommands)
			{
				throw new ArgumentOutOfRangeException("Commands", $"This platform does not support more than {MaximumCommands} commands.");
			}
		}

		private int GetDialogButtonType(int commandIndex)
		{
			return (int)DialogButtonType.Positive - commandIndex;
		}

		private class DialogListener : Java.Lang.Object, IDialogInterfaceOnCancelListener, IDialogInterfaceOnDismissListener
		{
			private readonly MessageDialog _dialog;
			private readonly TaskCompletionSource<IUICommand> _source;

			public DialogListener(MessageDialog dialog, TaskCompletionSource<IUICommand> source)
			{
				_dialog = dialog;
				_source = source;
			}

			public void OnCancel(IDialogInterface dialog)
			{
				// Cancelling from user action should never throw, but instead return either the "cancel" command, or null.
				_source.TrySetResult(_dialog.Commands.ElementAtOrDefault((int)_dialog.CancelCommandIndex));
			}

			public void OnDismiss(IDialogInterface dialog)
			{
				// Nothing special here
			}
		}
	}
}
#endif
