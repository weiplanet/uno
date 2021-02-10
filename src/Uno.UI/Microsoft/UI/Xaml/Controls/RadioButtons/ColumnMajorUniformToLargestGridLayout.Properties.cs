﻿using Windows.UI.Xaml;

namespace Microsoft.UI.Xaml.Controls.Primitives
{
	public partial class ColumnMajorUniformToLargestGridLayout
	{
		public double ColumnSpacing
		{
			get => (double)GetValue(ColumnSpacingProperty);
			set => SetValue(ColumnSpacingProperty, value);
		}

		public static readonly DependencyProperty ColumnSpacingProperty =
			DependencyProperty.Register(nameof(ColumnSpacing), typeof(double), typeof(ColumnMajorUniformToLargestGridLayout), new PropertyMetadata(default(double), OnColumnSpacingPropertyChanged));

		public int MaxColumns
		{
			get => (int)GetValue(MaxColumnsProperty);
			set => SetValue(MaxColumnsProperty, value);
		}

		public static readonly DependencyProperty MaxColumnsProperty =
			DependencyProperty.Register(nameof(MaxColumns), typeof(int), typeof(ColumnMajorUniformToLargestGridLayout), new PropertyMetadata(default(int), OnMaxColumnsPropertyChanged));

		public double RowSpacing
		{
			get => (double)GetValue(RowSpacingProperty);
			set => SetValue(RowSpacingProperty, value);
		}

		public static readonly DependencyProperty RowSpacingProperty =
			DependencyProperty.Register(nameof(RowSpacing), typeof(double), typeof(ColumnMajorUniformToLargestGridLayout), new PropertyMetadata(default(double), OnRowSpacingPropertyChanged));

		private static void OnColumnSpacingPropertyChanged(
			DependencyObject sender,
			DependencyPropertyChangedEventArgs args)
		{
			var owner = (ColumnMajorUniformToLargestGridLayout)sender;
			owner.OnColumnSpacingPropertyChanged(args);
		}

		private static void OnMaxColumnsPropertyChanged(
			DependencyObject sender,
			DependencyPropertyChangedEventArgs args)
		{
			var owner = (ColumnMajorUniformToLargestGridLayout)sender;

			var value = (int)args.NewValue;
			var coercedValue = value;
			owner.ValidateGreaterThanZero(coercedValue);
			if (value != coercedValue)
			{
				sender.SetValue(args.Property, coercedValue);
				return;
			}

			owner.OnMaxColumnsPropertyChanged(args);
		}

		private static void OnRowSpacingPropertyChanged(
			DependencyObject sender,
			DependencyPropertyChangedEventArgs args)
		{
			var owner = (ColumnMajorUniformToLargestGridLayout)sender;
			owner.OnRowSpacingPropertyChanged(args);
		}
	}
}
