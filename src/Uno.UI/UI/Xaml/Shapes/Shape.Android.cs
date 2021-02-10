﻿using Android.Graphics;
using Windows.UI.Xaml.Controls;
using System;
using Uno.Logging;
using Uno.Extensions;
using System.Drawing;
using Uno.UI;
using Windows.UI.Xaml.Media;
using System.Linq;

namespace Windows.UI.Xaml.Shapes
{
	public partial class Shape
	{
		protected bool HasStroke
		{
			get { return Stroke != null && ActualStrokeThickness > 0; }
		}

		internal double PhysicalStrokeThickness
		{
			get { return ViewHelper.LogicalToPhysicalPixels((double)ActualStrokeThickness); }
		}

		protected Windows.Foundation.Rect GetDrawArea(Android.Graphics.Canvas canvas)
		{
			var drawSize = default(Windows.Foundation.Size);

			var suggestedWidth = canvas.Width;
			var suggestedHeight = canvas.Height;

			switch (Stretch)
			{
				case Stretch.Fill:
					drawSize.Width = suggestedWidth;
					drawSize.Height = suggestedHeight;
					break;
				case Stretch.None:
					drawSize.Width = double.IsNaN(Width) || Width == 0 ? 0 : suggestedWidth;
					drawSize.Height = double.IsNaN(Height) || Height == 0 ? 0 : suggestedHeight;
					break;
				case Stretch.Uniform:
					drawSize.Width = Math.Min(suggestedWidth, suggestedHeight);
					drawSize.Height = Math.Min(suggestedWidth, suggestedHeight);
					break;
				case Stretch.UniformToFill:
					drawSize.Width = Math.Max(suggestedWidth, suggestedHeight);
					drawSize.Height = Math.Max(suggestedWidth, suggestedHeight);
					break;
			}

			var drawArea = HasStroke
				? GetAdjustedRectangle(drawSize, PhysicalStrokeThickness)
				: new Windows.Foundation.Rect(Windows.Foundation.Point.Zero, drawSize);

			return drawArea;
		}

		protected void DrawFill(Android.Graphics.Canvas canvas, Windows.Foundation.Rect fillArea, Android.Graphics.Path fillPath)
		{
			if (!fillArea.HasZeroArea())
			{
				var imageBrushFill = Fill as ImageBrush;
				if (imageBrushFill != null)
				{
					imageBrushFill.ScheduleRefreshIfNeeded(fillArea, Invalidate);
					imageBrushFill.DrawBackground(canvas, fillArea, fillPath);
				}
				else
				{
					var fill = Fill ?? SolidColorBrushHelper.Transparent;
					var fillPaint = fill.GetFillPaint(fillArea);
					canvas.DrawPath(fillPath, fillPaint);
				}
			}
		}

		protected void DrawStroke(Android.Graphics.Canvas canvas, Windows.Foundation.Rect strokeArea, Action<Android.Graphics.Canvas, Windows.Foundation.Rect, Paint> drawingMethod)
		{
			if (HasStroke)
			{
				var strokeThickness = PhysicalStrokeThickness;

				using (var strokePaint = new Paint(Stroke.GetStrokePaint(strokeArea)))
				{
					SetStrokeDashEffect(strokePaint);

					if (strokeArea.HasZeroArea())
					{
						//Draw the stroke as a fill because the shape has no area
						strokePaint.SetStyle(Paint.Style.Fill);
						canvas.DrawCircle((float)(strokeThickness / 2), (float)(strokeThickness / 2), (float)(strokeThickness / 2), strokePaint);
					}
					else
					{
						strokePaint.StrokeWidth = (float)strokeThickness;
						drawingMethod(canvas, strokeArea, strokePaint);
					}
				}
			}
		}

		protected void SetStrokeDashEffect(Paint strokePaint)
		{
			var strokeDashArray = StrokeDashArray;

			if (strokeDashArray != null && strokeDashArray.Count > 0)
			{
				// If only value specified in the dash array, copy and add it
				if (strokeDashArray.Count == 1)
				{
					strokeDashArray.Add(strokeDashArray[0]);
				}

				// Make sure the dash array has a positive number of items, Android cannot have an odd number
				// of items in the array (in such a case we skip the dash effect and log the error)
				//		https://developer.android.com/reference/android/graphics/DashPathEffect.html
				//		**  The intervals array must contain an even number of entries (>=2), with
				//			the even indices specifying the "on" intervals, and the odd indices
				//			specifying the "off" intervals.  **
				if (strokeDashArray.Count % 2 == 0)
				{
					var pattern = strokeDashArray.Select(d => (float)d).ToArray();
					strokePaint.SetPathEffect(new DashPathEffect(pattern, 0));
				}
				else
				{
					this.Log().ErrorIfEnabled(() => "StrokeDashArray containing an odd number of values is not supported on Android.");
				}
			}
		}

		/// <summary>
		/// Convert the drawSize to a drawArea adjusted to prevent the shape's stroke from exceeding its bounds (half the strokeThickness is drawn outside the drawArea)
		/// </summary>
		/// <param name="drawSize"></param>
		/// <param name="strokeThickness"></param>
		/// <returns></returns>
		private static Windows.Foundation.Rect GetAdjustedRectangle(Windows.Foundation.Size drawSize, double strokeThickness)
		{
			if (drawSize == default(Windows.Foundation.Size) || double.IsNaN(drawSize.Width) || double.IsNaN(drawSize.Height))
			{
				return Windows.Foundation.Rect.Empty;
			}

			return new Windows.Foundation.Rect
			(
				x: (float)(strokeThickness / 2), 
				y: (float)(strokeThickness / 2),
				width: (float)(drawSize.Width - strokeThickness), 
				height: (float)(drawSize.Height - strokeThickness)
			);
		}

		partial void OnFillUpdatedPartial()
		{
			this.Invalidate();
		}

		partial void OnStrokeUpdatedPartial()
		{
			this.Invalidate();
		}
		partial void OnStretchUpdatedPartial()
		{
			this.Invalidate();
		}
		partial void OnStrokeThicknessUpdatedPartial()
		{
			this.Invalidate();
		}
	}
}
