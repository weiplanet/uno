#if !NETFX_CORE
using System;

using Uno.Extensions;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
using Uno.UI.Extensions;

#if XAMARIN_ANDROID
using View = Android.Views.View;
using Font = Android.Graphics.Typeface;
using Android.Graphics;
#elif XAMARIN_IOS_UNIFIED
using View = UIKit.UIView;
using Color = UIKit.UIColor;
using Font = UIKit.UIFont;
#endif

namespace Uno.UI.DataBinding
{
	internal static partial class BindingPropertyHelper
	{
		/// <summary>
		/// Converts the input to the outputType using a fast conversion, for known system types.
		/// </summary>
		/// <param name="outputType">The target type</param>
		/// <param name="input">The input value to use</param>
		/// <param name="output">The input value converted to the <paramref name="outputType"/>.</param>
		/// <returns>True if the conversion succeeded, otherwise false.</returns>
		/// <remarks>
		/// This is a fast path conversion that avoids going through the TypeConverter
		/// infrastructure for known system types.
		/// </remarks>
		private static bool FastConvert(Type outputType, object input, out object output)
		{
			output = null;

			if (
				input is string stringInput
				&& FastStringConvert(outputType, stringInput, ref output)
			)
			{
				return true;
			}

			if (FastNumberConvert(outputType, input, ref output))
			{
				return true;
			}

			return input switch
			{
				Enum _ => FastEnumConvert(outputType, input, ref output),
				bool boolInput => FastBooleanConvert(outputType, boolInput, ref output),
				Windows.UI.Color color => FastColorConvert(outputType, color, ref output),
				SolidColorBrush solidColorBrush => FastSolidColorBrushConvert(outputType, solidColorBrush, ref output),
				ColorOffset colorOffsetInput => FastColorOffsetConvert(outputType, colorOffsetInput, ref output),
				Thickness thicknessInput => FastThicknessConvert(outputType, thicknessInput, ref output),
				_ => false
			};
		}

		private static bool FastBooleanConvert(Type outputType, bool boolInput, ref object output)
		{
			if (outputType == typeof(Visibility))
			{
				output = boolInput ? Visibility.Visible : Visibility.Collapsed;
				return true;
			}

			return false;
		}

		private static bool FastEnumConvert(Type outputType, object input, ref object output)
		{
			if (outputType == typeof(string))
			{
				output = input.ToString();
				return true;
			}

			return false;
		}

		private static bool FastColorOffsetConvert(Type outputType, ColorOffset input, ref object output)
		{
			if (outputType == typeof(Windows.UI.Color))
			{
				output = (Windows.UI.Color)input;
				return true;
			}

			return false;
		}

		private static bool FastColorConvert(Type outputType, Windows.UI.Color color, ref object output)
		{
			if (outputType == typeof(SolidColorBrush))
			{
				output = new SolidColorBrush(color);
				return true;
			}

			return false;
		}

		private static bool FastSolidColorBrushConvert(Type outputType, SolidColorBrush solidColorBrush,
			ref object output)
		{
			if (outputType == typeof(Windows.UI.Color) || outputType == typeof(Windows.UI.Color?))
			{
				output = solidColorBrush.Color;
				return true;
			}

			return false;
		}

		private static bool FastThicknessConvert(Type outputType, Thickness thickness, ref object output)
		{
			if (outputType == typeof(double))
			{
				if (thickness.IsUniform())
				{
					output = thickness.Left;
					return true;
				}

				// TODO: test what Windows does in non-uniform case
			}

			return false;
		}

		private static bool FastNumberConvert(Type outputType, object input, ref object output)
		{
			if (input is double doubleInput)
			{
				if(outputType == typeof(float))
				{
					output = (float)doubleInput;
					return true;
				}
				if (outputType == typeof(TimeSpan))
				{
					output = TimeSpan.FromSeconds(doubleInput);
					return true;
				}
				if (outputType == typeof(GridLength))
				{
					output = GridLengthHelper.FromPixels(doubleInput);
					return true;
				}
			}

			if (input is int intInput)
			{
				if (outputType == typeof(float))
				{
					output = (float)intInput;
					return true;
				}
				if (outputType == typeof(TimeSpan))
				{
					output = TimeSpan.FromSeconds(intInput);
					return true;
				}
				if (outputType == typeof(GridLength))
				{
					output = GridLengthHelper.FromPixels(intInput);
					return true;
				}
			}

			return false;
		}

		private static bool FastStringConvert(Type outputType, string input, ref object output)
		{
			if (FastStringToVisibilityConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToHorizontalAlignmentConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToVerticalAlignmentConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToBrushConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToDoubleConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToSingleConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToThicknessConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToGridLengthConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToOrientationConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToColorConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToTextAlignmentConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToImageSource(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToFontWeightConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToFontFamilyConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToIntegerConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToPointF(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToPoint(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToMatrix(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToDuration(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToRepeatBehavior(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToKeyTime(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToKeySpline(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToPointCollection(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToPath(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToInputScope(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToToolTip(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToIconElement(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToUriConvert(outputType, input, ref output))
			{
				return true;
			}

			if (FastStringToTypeConvert(outputType, input, ref output))
			{
				return true;
			}

			// Fallback for Enums. Leave it at the end.
			if (outputType.IsEnum)
			{
				output = Enum.Parse(outputType, input, true);
				return true;
			}

			return false;
		}

		private static bool FastStringToIconElement(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Windows.UI.Xaml.Controls.IconElement))
			{
				output = (Windows.UI.Xaml.Controls.IconElement)input;
				return true;
			}

			return false;
		}

		private static bool FastStringToInputScope(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Windows.UI.Xaml.Input.InputScope))
			{
				object nameValue = null;
				if (FastEnumConvert(typeof(Windows.UI.Xaml.Input.InputScopeNameValue), input, ref nameValue))
				{
					output = new Windows.UI.Xaml.Input.InputScope
					{
						Names = {
							new Windows.UI.Xaml.Input.InputScopeName
							{
								NameValue = (Windows.UI.Xaml.Input.InputScopeNameValue)nameValue
							}
						}
					};

					return true;
				}
			}

			return false;
		}

		private static bool FastStringToToolTip(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Windows.UI.Xaml.Controls.ToolTip))
			{
				output = new Windows.UI.Xaml.Controls.ToolTip {Content = input};
				return true;
			}

			return false;
		}

		private static bool FastStringToKeyTime(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(KeyTime))
			{
				output = KeyTime.FromTimeSpan(TimeSpan.Parse(input, CultureInfo.InvariantCulture));
				return true;
			}

			return false;
		}


		private static bool FastStringToKeySpline(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(KeySpline))
			{
				output = KeySpline.FromString(input);
				return true;
			}

			return false;
		}

		private static bool FastStringToDuration(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Duration))
			{
				if (input == nameof(Duration.Forever))
				{
					output = Duration.Forever;
				}
				else if (input == nameof(Duration.Automatic))
				{
					output = Duration.Automatic;
				}
				else
				{
					output = new Duration(TimeSpan.Parse(input, CultureInfo.InvariantCulture));
				}

				return true;
			}

			return false;
		}

		private static bool FastStringToRepeatBehavior(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(RepeatBehavior)
				&& input == nameof(RepeatBehavior.Forever))
			{
				output = RepeatBehavior.Forever;

				return true;
			}

			return false;
		}

		private static bool FastStringToPointCollection(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(PointCollection))
			{
				output = (PointCollection)input;

				return true;
			}

			return false;
		}

		private static bool FastStringToPath(Type outputType, string input, ref object output)
		{
#if __WASM__
			if (outputType == typeof(Geometry))
			{
				output = (Geometry)input;

				return true;
			}
#endif

			return false;
		}

		private static bool FastStringToFontFamilyConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(FontFamily))
			{
				output = new FontFamily(input);

				return true;
			}

			return false;
		}

		private static bool FastStringToMatrix(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Windows.UI.Xaml.Media.Matrix))
			{
				var fields = input
					.Split(new[] { ',' })
					?.Select(v => double.Parse(v, CultureInfo.InvariantCulture))
					?.ToArray();

				output = new Windows.UI.Xaml.Media.Matrix(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5]);
				return true;
			}

			return false;
		}

		private static bool FastStringToPointF(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(System.Drawing.PointF))
			{
				var fields = input
					.Split(new[] { ',' })
					?.Select(v => float.Parse(v, CultureInfo.InvariantCulture))
					?.ToArray();

				if (fields?.Length == 2)
				{
					output = new System.Drawing.PointF(fields[0], fields[1]);
					return true;
				}

				if (fields?.Length == 1)
				{
					output = new System.Drawing.PointF(fields[0], fields[0]);
					return true;
				}
			}

			return false;
		}

		private static bool FastStringToPoint(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Windows.Foundation.Point))
			{
				var fields = input
					.Split(new[] { ',' })
					?.Select(v => double.Parse(v, CultureInfo.InvariantCulture))
					?.ToArray();

				if (fields?.Length == 2)
				{
					output = new Windows.Foundation.Point(fields[0], fields[1]);
					return true;
				}

				if (fields?.Length == 1)
				{
					output = new Windows.Foundation.Point(fields[0], fields[0]);
					return true;
				}
			}

			return false;
		}

		private static bool FastStringToBrushConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Brush))
			{
				output = SolidColorBrushHelper.Parse(input);
				return true;
			}

			return false;
		}

		private static bool FastStringToColorConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Windows.UI.Color))
			{
				output = Windows.UI.Colors.Parse(input);
				return true;
			}

			return false;
		}

		private static bool FastStringToImageSource(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Windows.UI.Xaml.Media.ImageSource))
			{
				output = (Windows.UI.Xaml.Media.ImageSource)input;
				return true;
			}

			return false;
		}

		private static bool FastStringToTextAlignmentConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(TextAlignment))
			{
				switch (input.ToLowerInvariant())
				{
					case "center":
						output = TextAlignment.Center;
						return true;

					case "left":
						output = TextAlignment.Left;
						return true;

					case "right":
						output = TextAlignment.Right;
						return true;

					case "justify":
						output = TextAlignment.Justify;
						return true;

					case "detectfromcontent":
						output = TextAlignment.DetectFromContent;
						return true;

					default:
						throw new InvalidOperationException($"The value {input} is not a valid {nameof(TextAlignment)}");
				}
			}

			return false;
		}

		private static bool FastStringToGridLengthConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(GridLength))
			{
				var gridLengths = GridLength.ParseGridLength(input);

				if (gridLengths.Length > 0)
				{
					output = gridLengths[0];
					return true;
				}
			}

			return false;
		}

		private static bool FastStringToDoubleConvert(Type outputType, string input, ref object output)
		{
			const NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

			if (outputType == typeof(double))
			{
				if (input == "") // empty string is NaN when bound in XAML
				{
					output = double.NaN;
					return true;
				}

				if (input.Length == 1)
				{
					// Fast path for one digit string-to-double.
					// Often use in VisualStateManager to set double values (like opacity) to zero or one...
					var c = input[0];
					if (c >= '0' && c <= '9')
					{
						output = (double)(c - '0');
						return true;
					}
				}

				var trimmed = input.Trim();

				if (trimmed == "0" || trimmed == "") // Fast path for zero / empty values (means zero in XAML)
				{
					output = 0d;
					return true;
				}

				trimmed = trimmed.ToLowerInvariant();

				if (trimmed == "nan" || trimmed == "auto")
				{
					output = double.NaN;
					return true;
				}

				if (trimmed == "-infinity")
				{
					output = double.NegativeInfinity;
					return true;
				}

				if (trimmed == "infinity")
				{
					output = double.PositiveInfinity;
					return true;
				}

				if (double.TryParse(trimmed, numberStyles, NumberFormatInfo.InvariantInfo, out var d))
				{
					output = d;
					return true;
				}
			}

			return false;
		}

		private static bool FastStringToSingleConvert(Type outputType, string input, ref object output)
		{
			const NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

			if (outputType == typeof(float))
			{
				if (input == "") // empty string is NaN when bound in XAML
				{
					output = float.NaN;
					return true;
				}

				if (input.Length == 1)
				{
					// Fast path for one digit string-to-float
					var c = input[0];
					if (c >= '0' && c <= '9')
					{
						output = (float)(c - '0');
						return true;
					}
				}

				var trimmed = input.Trim();

				if (trimmed == "0" || trimmed == "") // Fast path for zero / empty values (means zero in XAML)
				{
					output = 0f;
					return true;
				}

				trimmed = trimmed.ToLowerInvariant();

				if (trimmed == "nan") // "Auto" is for sizes, which are only of type double
				{
					output = float.NaN;
					return true;
				}

				if (trimmed == "-infinity")
				{
					output = float.NegativeInfinity;
					return true;
				}

				if (trimmed == "infinity")
				{
					output = float.PositiveInfinity;
					return true;
				}

				if (float.TryParse(trimmed, numberStyles, NumberFormatInfo.InvariantInfo, out var f))
				{
					output = f;
					return true;
				}
			}

			return false;
		}

		private static bool FastStringToIntegerConvert(Type outputType, string input, ref object output)
		{
			const NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

			if (outputType == typeof(int))
			{
				if (input.Length == 1)
				{
					// Fast path for one digit string-to-float
					var c = input[0];
					if (c >= '0' && c <= '9')
					{
						output = (int)(c - '0');
						return true;
					}
				}

				var trimmed = input.Trim();

				if (trimmed == "0" || trimmed == "") // Fast path for zero / empty values (means zero in XAML)
				{
					output = 0;
					return true;
				}

				if (int.TryParse(trimmed, numberStyles, NumberFormatInfo.InvariantInfo, out var i))
				{
					output = i;
					return true;
				}
			}

			return false;
		}

		private static bool FastStringToOrientationConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Windows.UI.Xaml.Controls.Orientation))
			{
				switch (input.ToLowerInvariant())
				{
					case "vertical":
						output = Windows.UI.Xaml.Controls.Orientation.Vertical;
						return true;

					case "horizontal":
						output = Windows.UI.Xaml.Controls.Orientation.Horizontal;
						return true;

					default:
						throw new InvalidOperationException($"The value {input} is not a valid {nameof(Windows.UI.Xaml.Controls.Orientation)}");
				}
			}

			return false;
		}

		private static bool FastStringToThicknessConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Thickness))
			{
				output = new ThicknessConverter().ConvertFrom(input);
				return true;
			}

			return false;
		}

		private static bool FastStringToVerticalAlignmentConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(VerticalAlignment))
			{
				switch (input.ToLowerInvariant())
				{
					case "center":
						output = VerticalAlignment.Center;
						return true;

					case "top":
						output = VerticalAlignment.Top;
						return true;

					case "bottom":
						output = VerticalAlignment.Bottom;
						return true;

					case "stretch":
						output = VerticalAlignment.Stretch;
						return true;

					default:
						throw new InvalidOperationException($"The value {input} is not a valid {nameof(VerticalAlignment)}");
				}
			}

			return false;
		}

		private static bool FastStringToHorizontalAlignmentConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(HorizontalAlignment))
			{
				switch (input.ToLowerInvariant())
				{
					case "center":
						output = HorizontalAlignment.Center;
						return true;

					case "left":
						output = HorizontalAlignment.Left;
						return true;

					case "right":
						output = HorizontalAlignment.Right;
						return true;

					case "stretch":
						output = HorizontalAlignment.Stretch;
						return true;

					default:
						throw new InvalidOperationException($"The value {input} is not a valid {nameof(HorizontalAlignment)}");
				}
			}

			return false;
		}

		private static bool FastStringToVisibilityConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Visibility))
			{
				switch (input.ToLowerInvariant())
				{
					case "visible":
						output = Visibility.Visible;
						return true;

					case "collapsed":
						output = Visibility.Collapsed;
						return true;

					default:
						throw new InvalidOperationException($"The value {input} is not a valid {nameof(Visibility)}");
				}
			}

			return false;
		}

		private static bool FastStringToFontWeightConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(FontWeight))
			{
				// Note that list is hard coded to avoid the cold path cost of reflection.

				switch (input.ToLowerInvariant())
				{
					case "thin":
						output = FontWeights.Thin;
						return true;
					case "extralight":
						output = FontWeights.ExtraLight;
						return true;
					case "ultralight":
						output = FontWeights.UltraLight;
						return true;
					case "semilight":
						output = FontWeights.SemiLight;
						return true;
					case "light":
						output = FontWeights.Light;
						return true;
					case "normal":
						output = FontWeights.Normal;
						return true;
					case "regular":
						output = FontWeights.Regular;
						return true;
					case "medium":
						output = FontWeights.Medium;
						return true;
					case "semibold":
						output = FontWeights.SemiBold;
						return true;
					case "demibold":
						output = FontWeights.DemiBold;
						return true;
					case "bold":
						output = FontWeights.Bold;
						return true;
					case "ultrabold":
						output = FontWeights.UltraBold;
						return true;
					case "extrabold":
						output = FontWeights.ExtraBold;
						return true;
					case "black":
						output = FontWeights.Black;
						return true;
					case "heavy":
						output = FontWeights.Heavy;
						return true;
					case "extrablack":
						output = FontWeights.ExtraBlack;
						return true;
					case "ultrablack":
						output = FontWeights.UltraBlack;
						return true;

					default:
						throw new InvalidOperationException($"The value {input} is not a valid {nameof(FontWeight)}");
				}
			}

			return false;
		}

		private static bool FastStringToUriConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Uri))
			{
				output = new Uri(input);
				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool FastStringToTypeConvert(Type outputType, string input, ref object output)
		{
			if (outputType == typeof(Type))
			{
				output = Type.GetType(input, throwOnError: true);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
#endif
