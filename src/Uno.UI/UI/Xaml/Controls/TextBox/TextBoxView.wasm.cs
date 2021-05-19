﻿using System;
using System.Collections.Generic;
using System.Text;
using Uno.UI;
using Uno.Extensions;
using Windows.UI.Xaml.Media;
using Uno.Logging;
using Windows.Foundation;
using System.Globalization;
using Uno.Disposables;

namespace Windows.UI.Xaml.Controls
{
	internal partial class TextBoxView : FrameworkElement
	{
		private readonly TextBox _textBox;
		private readonly SerialDisposable _foregroundChanged = new SerialDisposable();

		public Brush Foreground
		{
			get => (Brush)GetValue(ForegroundProperty);
			set => SetValue(ForegroundProperty, value);
		}

		internal static DependencyProperty ForegroundProperty { get; } =
			DependencyProperty.Register(
				name: "Foreground",
				propertyType: typeof(Brush),
				ownerType: typeof(TextBoxView),
				typeMetadata: new FrameworkPropertyMetadata(
					defaultValue: null,
					options: FrameworkPropertyMetadataOptions.Inherits,
					propertyChangedCallback: (s, e) => (s as TextBoxView)?.OnForegroundChanged(e)));

		private void OnForegroundChanged(DependencyPropertyChangedEventArgs e)
		{
			_foregroundChanged.Disposable = null;
			if (e.NewValue is SolidColorBrush scb)
			{
				_foregroundChanged.Disposable = Brush.AssignAndObserveBrush(scb, _ => this.SetForeground(e.NewValue));
			}

			this.SetForeground(e.NewValue);
		}

		public TextBoxView(TextBox textBox, bool isMultiline)
			: base(isMultiline ? "textarea" : "input")
		{
			IsMultiline = isMultiline;
			_textBox = textBox;
			SetTextNative(_textBox.Text);

			if (FeatureConfiguration.TextBox.HideCaret)
			{
				SetStyle(
					("caret-color", "transparent !important")
				);
			}

			SetAttribute("tabindex", "0");
		}

		private event EventHandler HtmlInput
		{
			add => RegisterEventHandler("input", value, GenericEventHandlers.RaiseEventHandler);
			remove => UnregisterEventHandler("input", value, GenericEventHandlers.RaiseEventHandler);
		}

		internal bool IsMultiline { get; }

		private protected override void OnLoaded()
		{
			base.OnLoaded();

			HtmlInput += OnInput;

			SetTextNative(_textBox.Text);
		}

		private protected override void OnUnloaded()
		{
			base.OnUnloaded();

			HtmlInput -= OnInput;
		}

		private void OnInput(object sender, EventArgs eventArgs)
		{
			var text = GetProperty("value");

			var updatedText = _textBox.ProcessTextInput(text);

			if (updatedText != text)
			{
				SetTextNative(updatedText);
			}

			InvalidateMeasure();
		}

		internal void SetTextNative(string text)
		{
			SetProperty("value", text);

			InvalidateMeasure();
		}

		protected override Size MeasureOverride(Size availableSize) => MeasureView(availableSize);

		internal void SetIsPassword(bool isPassword)
		{
			if (IsMultiline)
			{
				throw new NotSupportedException("A PasswordBox cannot have multiple lines.");
			}
			SetAttribute("type", isPassword ? "password" : "text");
		}

		internal void SetEnabled(bool newValue) => SetProperty("disabled", newValue ? "false" : "true");

		internal void SetIsReadOnly(bool isReadOnly)
		{
			if (isReadOnly)
			{
				SetAttribute("readonly", "readonly");
			}
			else
			{
				RemoveAttribute("readonly");
			}
		}

		public int SelectionStart
		{
			get => int.TryParse(GetProperty("selectionStart"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : 0;
			set => SetProperty("selectionStart", value.ToString());
		}

		public int SelectionEnd
		{
			get => int.TryParse(GetProperty("selectionEnd"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : 0;
			set => SetProperty("selectionEnd", value.ToString());
		}

		internal override bool IsViewHit() => true;
	}
}
