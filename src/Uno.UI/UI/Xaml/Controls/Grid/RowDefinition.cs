﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Uno.UI.Xaml;

namespace Windows.UI.Xaml.Controls
{
	[DebuggerDisplay("{DebugDisplay,nq}")]
	public partial class RowDefinition : DefinitionBase, DependencyObject
	{
		public RowDefinition()
		{
			InitializeBinder();
			IsAutoPropertyInheritanceEnabled = false;

			this.RegisterDisposablePropertyChangedCallback((i, p, args) =>
			{
				Changed?.Invoke(this, EventArgs.Empty);
				SetDefaultState();
			});
			SetDefaultState();
		}

		#region Height DependencyProperty

		private static GridLength GetHeightDefaultValue() => GridLengthHelper.OneStar;

		[GeneratedDependencyProperty]
		public static DependencyProperty HeightProperty { get; } = CreateHeightProperty();

		public GridLength Height
		{
			get => GetHeightValue();
			set => SetHeightValue(value);
		}

		#endregion

		public static implicit operator RowDefinition(string value)
		{
			return new RowDefinition { Height = GridLength.ParseGridLength(value).First() };
		}

		[GeneratedDependencyProperty(DefaultValue = 0d)]
		public static DependencyProperty MinHeightProperty { get; } = CreateMinHeightProperty();

		public double MinHeight
		{
			get => GetMinHeightValue();
			set => SetMinHeightValue(value);
		}

		private static GridLength GetMaxHeightDefaultValue() => GridLengthHelper.OneStar;

		[GeneratedDependencyProperty(DefaultValue = double.PositiveInfinity)]
		public static DependencyProperty MaxHeightProperty { get; } = CreateMaxHeightProperty();
		public double MaxHeight
		{
			get => GetMaxHeightValue();
			set => SetMaxHeightValue(value);
		}

		public double ActualHeight
		{
			get
			{
				//var parent = this.GetParent();
				//var result = (parent as Grid)?.GetActualHeight(this) ?? 0d;
				//return result;
				return _measureArrangeSize;
			}
		}

		//private string DebugDisplay => $"RowDefinition(Height={Height.ToDisplayString()};MinHeight={MinHeight};MaxHeight={MaxHeight};ActualHeight={ActualHeight}";
		private string DebugDisplay => $"RowDefinition(Height={Height.ToDisplayString()};MinHeight={MinHeight};MaxHeight={MaxHeight}";

		#region internal DefinitionBase

		private void SetDefaultState()
		{
			_effectiveMinSize = default;
			_measureArrangeSize = default;
			_sizeCache = default;
			_finalOffset = default;
			_effectiveUnitType = GridUnitType.Auto;
		}

		private double _effectiveMinSize;
		private double _measureArrangeSize;
		private double _sizeCache;
		private double _finalOffset;
		private GridUnitType _effectiveUnitType;

		double DefinitionBase.GetUserSizeValue() => Height.Value;

		GridUnitType DefinitionBase.GetUserSizeType() => Height.GridUnitType;

		double DefinitionBase.GetUserMaxSize() => MaxHeight;

		double DefinitionBase.GetUserMinSize() => MinHeight;

		double DefinitionBase.GetEffectiveMinSize() => _effectiveMinSize;

		void DefinitionBase.SetEffectiveMinSize(double value) => _effectiveMinSize = value;

		double DefinitionBase.GetMeasureArrangeSize() => _measureArrangeSize;

		void DefinitionBase.SetMeasureArrangeSize(double value) => _measureArrangeSize = value;

		double DefinitionBase.GetSizeCache() => _sizeCache;

		void DefinitionBase.SetSizeCache(double value) => _sizeCache = value;

		double DefinitionBase.GetFinalOffset() => _finalOffset;

		void DefinitionBase.SetFinalOffset(double value) => _finalOffset = value;

		GridUnitType DefinitionBase.GetEffectiveUnitType() => _effectiveUnitType;

		void DefinitionBase.SetEffectiveUnitType(GridUnitType type) => _effectiveUnitType = type;

		double DefinitionBase.GetPreferredSize()
		{
			return
				(_effectiveUnitType != GridUnitType.Auto
				 && _effectiveMinSize < _measureArrangeSize)
					? _measureArrangeSize
					: _effectiveMinSize;
		}

		void DefinitionBase.UpdateEffectiveMinSize(double newValue)
		{
			_effectiveMinSize = Math.Max(_effectiveMinSize, newValue);
		}

		private event EventHandler Changed;

		event EventHandler DefinitionBase.Changed
		{
			add => Changed += value;
			remove => Changed -= value;
		}

		#endregion
	}
}
