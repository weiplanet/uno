﻿using Uno.Extensions;
using Uno.Logging;
using Uno.UI.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Uno.UI.DataBinding;
using Uno.Disposables;
using Windows.UI.Xaml.Data;
using System.Runtime.CompilerServices;
using System.Drawing;
using Uno.UI;
using Windows.UI.Xaml.Media;

using View = Windows.UI.Xaml.UIElement;

namespace Windows.UI.Xaml.Controls
{
	public partial class Panel : IEnumerable
	{
		private readonly SerialDisposable _borderBrushChanged = new SerialDisposable();

		public Panel()
		{
			Initialize();
		}

		partial void Initialize();

		public void UpdateBorder()
		{
			SetBorder(BorderThickness, BorderBrush);
		}

		protected virtual void OnChildrenChanged()
		{
			UpdateBorder();
		}

		partial void OnPaddingChangedPartial(Thickness oldValue, Thickness newValue)
		{
			UpdateBorder();
		}

		partial void OnBorderBrushChangedPartial(Brush oldValue, Brush newValue)
		{
			_borderBrushChanged.Disposable = null;
			if (newValue?.SupportsAssignAndObserveBrush ?? false)
			{
				_borderBrushChanged.Disposable = Brush.AssignAndObserveBrush(newValue, _ => UpdateBorder());
			}

			UpdateBorder();
		}

		partial void OnBorderThicknessChangedPartial(Thickness oldValue, Thickness newValue)
		{
			UpdateBorder();
		}

		partial void OnCornerRadiusChangedPartial(CornerRadius oldValue, CornerRadius newValue)
		{
			SetCornerRadius(newValue);
		}

		/// <summary>        
		/// Support for the C# collection initializer style.
		/// Allows items to be added like this 
		/// new Panel 
		/// {
		///    new Border()
		/// }
		/// </summary>
		/// <param name="view"></param>
		public void Add(View view)
		{
			Children.Add(view);
		}

		public new IEnumerator GetEnumerator()
		{
			return this.GetChildren().GetEnumerator();
		}

		protected override void OnBackgroundChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnBackgroundChanged(e);
			UpdateHitTest();
		}

		bool ICustomClippingElement.AllowClippingToLayoutSlot => true;
		bool ICustomClippingElement.ForceClippingToLayoutSlot => CornerRadius != CornerRadius.None;
	}
}
