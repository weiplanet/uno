﻿using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Wasm;
using Uno.Extensions;

namespace Windows.UI.Xaml.Shapes
{
	partial class Polygon
	{
		private readonly SvgElement _polygon = new SvgElement("polygon");

		partial void InitializePartial()
		{
			SvgChildren.Add(_polygon);

			InitCommonShapeProperties();
		}

		protected override SvgElement GetMainSvgElement()
		{
			return _polygon;
		}

		partial void OnPointsChanged()
		{
			var points = Points;
			if (points == null)
			{
				_polygon.RemoveAttribute("points");
			}
			else
			{
				_polygon.SetAttribute("points", points.ToCssString());
			}
		}
	}
}
