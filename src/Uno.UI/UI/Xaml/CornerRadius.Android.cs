﻿using Android.Graphics;
using Uno.UI;

namespace Windows.UI.Xaml
{
	partial struct CornerRadius
	{
		internal Path GetOutlinePath(RectF rect)
		{
			var radii = GetRadii();

			var path = new Path();
			path.AddRoundRect(rect, radii, Path.Direction.Cw);

			return path;
		}

		internal float[] GetRadii()
		{
			return new float[]
			{
				ViewHelper.LogicalToPhysicalPixels(TopLeft),
				ViewHelper.LogicalToPhysicalPixels(TopLeft),
				ViewHelper.LogicalToPhysicalPixels(TopRight),
				ViewHelper.LogicalToPhysicalPixels(TopRight),
				ViewHelper.LogicalToPhysicalPixels(BottomRight),
				ViewHelper.LogicalToPhysicalPixels(BottomRight),
				ViewHelper.LogicalToPhysicalPixels(BottomLeft),
				ViewHelper.LogicalToPhysicalPixels(BottomLeft)
			};
		}
	}
}
