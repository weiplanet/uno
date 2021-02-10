﻿#if NETFX_CORE
using System;
using System.Windows.Input;

using ICommand = System.Windows.Input.ICommand;
using EventHandler = System.EventHandler;

namespace Uno.UI.Common
{
	public class DelegateCommand : ICommand
	{
		private Action _action;
		private bool _canExecuteEnabled = true;

		public event EventHandler CanExecuteChanged;

		public DelegateCommand(Action action)
		{
			_action = action;
		}

		public bool CanExecute(object parameter) => CanExecuteEnabled;

		public void Execute(object parameter) => _action?.Invoke();

		private void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

		public bool CanExecuteEnabled
		{
			get => _canExecuteEnabled;
			set
			{
				_canExecuteEnabled = value;
				OnCanExecuteChanged();
			}
		}
	}
}
#endif
