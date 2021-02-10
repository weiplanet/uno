﻿#nullable enable

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Windows.Foundation;
using Windows.UI;

namespace Windows.UI.Composition
{
	public static class SkiaExtensions
	{
		public static SKRect ToSKRect(this Rect rect)
			=> new SKRect((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);

		public static Rect ToRect(this SKRect rect)
			=> new Rect(x: (float)rect.Left, y: (float)rect.Top, width:(float)rect.Width, height: (float)rect.Height);

		public static Size ToSize(this SKSize size)
			=> new Size(size.Width, size.Height);

		public static SKColor ToSKColor(this Color color)
			=> new SKColor(red: color.R, green: color.G, blue: color.B, alpha: color.A);

		public static SKColor ToSKColor(this Color color, double alphaMultiplier)
			=> new SKColor(red: color.R, green: color.G, blue: color.B, alpha: (byte)(color.A * alphaMultiplier));

		public static SKMatrix44 ToSKMatrix44(this Matrix4x4 m)
		{
			var ret = new SKMatrix44();

			ret[0, 0] = m.M11;
			ret[1, 0] = m.M12;
			ret[2, 0] = m.M13;
			ret[3, 0] = m.M14;

			ret[0, 1] = m.M21;
			ret[1, 1] = m.M22;
			ret[2, 1] = m.M23;
			ret[3, 1] = m.M24;

			ret[0, 2] = m.M31;
			ret[1, 2] = m.M32;
			ret[2, 2] = m.M33;
			ret[3, 2] = m.M34;

			ret[0, 3] = m.M41;
			ret[1, 3] = m.M42;
			ret[2, 3] = m.M43;
			ret[3, 3] = m.M44;

			return ret;
		}

		public static SKMatrix44 ToSKMatrix44(this Matrix3x2 m)
		{
			var ret = new SKMatrix44();

			ret[0, 0] = m.M11;
			ret[1, 0] = m.M12;
			ret[2, 0] = 0;
			ret[3, 0] = 0;

			ret[0, 1] = m.M21;
			ret[1, 1] = m.M22;
			ret[2, 1] = 0;
			ret[3, 1] = 0;

			ret[0, 2] = m.M31;
			ret[1, 2] = m.M32;
			ret[2, 2] = 1;
			ret[3, 2] = 0;

			ret[0, 3] = 0;
			ret[1, 3] = 0;
			ret[2, 3] = 0;
			ret[3, 3] = 1;

			return ret;
		}

		public static SKMatrix ToSKMatrix(this Matrix3x2 m)
		{
			var ret = SKMatrix.Identity;

			ret.ScaleX = m.M11;
			ret.SkewY = m.M12;
			ret.SkewX = m.M21;
			ret.ScaleY = m.M22;
			ret.TransX = m.M31;
			ret.TransY = m.M32;

			return ret;
		}
	}
}
