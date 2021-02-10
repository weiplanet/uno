using Windows.UI;
using Windows.UI.Xaml;

namespace Microsoft.UI.Xaml.Controls
{
	public partial class ColorPicker
	{
		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public static DependencyProperty ColorProperty { get ; } =
			DependencyProperty.Register(
				nameof(Color),
				typeof(Color),
				typeof(ColorPicker),
				new PropertyMetadata(
					Color.FromArgb(255, 255, 255, 255),
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public ColorSpectrumComponents ColorSpectrumComponents
		{
			get => (ColorSpectrumComponents)GetValue(ColorSpectrumComponentsProperty);
			set => SetValue(ColorSpectrumComponentsProperty, value);
		}

		public static DependencyProperty ColorSpectrumComponentsProperty { get ; } =
			DependencyProperty.Register(
				nameof(ColorSpectrumComponents),
				typeof(ColorSpectrumComponents),
				typeof(ColorPicker),
				new PropertyMetadata(
					ColorSpectrumComponents.HueSaturation,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public ColorSpectrumShape ColorSpectrumShape
		{
			get => (ColorSpectrumShape)GetValue(ColorSpectrumShapeProperty);
			set => SetValue(ColorSpectrumShapeProperty, value);
		}

		public static DependencyProperty ColorSpectrumShapeProperty { get ; } =
			DependencyProperty.Register(
				nameof(ColorSpectrumShape),
				typeof(ColorSpectrumShape),
				typeof(ColorPicker),
				new PropertyMetadata(
					ColorSpectrumShape.Box,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsAlphaEnabled
		{
			get => (bool)GetValue(IsAlphaEnabledProperty);
			set => SetValue(IsAlphaEnabledProperty, value);
		}

		public static DependencyProperty IsAlphaEnabledProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsAlphaEnabled),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					false,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsAlphaSliderVisible
		{
			get => (bool)GetValue(IsAlphaSliderVisibleProperty);
			set => SetValue(IsAlphaSliderVisibleProperty, value);
		}

		public static DependencyProperty IsAlphaSliderVisibleProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsAlphaSliderVisible),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					true,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsAlphaTextInputVisible
		{
			get => (bool)GetValue(IsAlphaTextInputVisibleProperty);
			set => SetValue(IsAlphaTextInputVisibleProperty, value);
		}

		public static DependencyProperty IsAlphaTextInputVisibleProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsAlphaTextInputVisible),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					true,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsColorChannelTextInputVisible
		{
			get => (bool)GetValue(IsColorChannelTextInputVisibleProperty);
			set => SetValue(IsColorChannelTextInputVisibleProperty, value);
		}

		public static DependencyProperty IsColorChannelTextInputVisibleProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsColorChannelTextInputVisible),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					true,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsColorPreviewVisible
		{
			get => (bool)GetValue(IsColorPreviewVisibleProperty);
			set => SetValue(IsColorPreviewVisibleProperty, value);
		}

		public static DependencyProperty IsColorPreviewVisibleProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsColorPreviewVisible),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					true,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsColorSliderVisible
		{
			get => (bool)GetValue(IsColorSliderVisibleProperty);
			set => SetValue(IsColorSliderVisibleProperty, value);
		}

		public static DependencyProperty IsColorSliderVisibleProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsColorSliderVisible),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					true,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsColorSpectrumVisible
		{
			get => (bool)GetValue(IsColorSpectrumVisibleProperty);
			set => SetValue(IsColorSpectrumVisibleProperty, value);
		}

		public static DependencyProperty IsColorSpectrumVisibleProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsColorSpectrumVisible),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					true,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsHexInputVisible
		{
			get => (bool)GetValue(IsHexInputVisibleProperty);
			set => SetValue(IsHexInputVisibleProperty, value);
		}

		public static DependencyProperty IsHexInputVisibleProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsHexInputVisible),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					true,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public bool IsMoreButtonVisible
		{
			get => (bool)GetValue(IsMoreButtonVisibleProperty);
			set => SetValue(IsMoreButtonVisibleProperty, value);
		}

		public static DependencyProperty IsMoreButtonVisibleProperty { get ; } =
			DependencyProperty.Register(
				nameof(IsMoreButtonVisible),
				typeof(bool),
				typeof(ColorPicker),
				new PropertyMetadata(
					false,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public int MaxHue
		{
			get => (int)GetValue(MaxHueProperty);
			set => SetValue(MaxHueProperty, value);
		}

		public static DependencyProperty MaxHueProperty { get ; } =
			DependencyProperty.Register(
				nameof(MaxHue),
				typeof(int),
				typeof(ColorPicker),
				new PropertyMetadata(
					359,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public int MaxSaturation
		{
			get => (int)GetValue(MaxSaturationProperty);
			set => SetValue(MaxSaturationProperty, value);
		}

		public static DependencyProperty MaxSaturationProperty { get ; } =
			DependencyProperty.Register(
				nameof(MaxSaturation),
				typeof(int),
				typeof(ColorPicker),
				new PropertyMetadata(
					100,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public int MaxValue
		{
			get => (int)GetValue(MaxValueProperty);
			set => SetValue(MaxValueProperty, value);
		}

		public static DependencyProperty MaxValueProperty { get ; } =
			DependencyProperty.Register(
				nameof(MaxValue),
				typeof(int),
				typeof(ColorPicker),
				new PropertyMetadata(
					100,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public int MinHue
		{
			get => (int)GetValue(MinHueProperty);
			set => SetValue(MinHueProperty, value);
		}

		public static DependencyProperty MinHueProperty { get ; } =
			DependencyProperty.Register(
				nameof(MinHue),
				typeof(int),
				typeof(ColorPicker),
				new PropertyMetadata(
					0,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public int MinSaturation
		{
			get => (int)GetValue(MinSaturationProperty);
			set => SetValue(MinSaturationProperty, value);
		}

		public static DependencyProperty MinSaturationProperty { get ; } =
			DependencyProperty.Register(
				nameof(MinSaturation),
				typeof(int),
				typeof(ColorPicker),
				new PropertyMetadata(
					0,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public int MinValue
		{
			get => (int)GetValue(MinValueProperty);
			set => SetValue(MinValueProperty, value);
		}

		public static DependencyProperty MinValueProperty { get ; } =
			DependencyProperty.Register(
				nameof(MinValue),
				typeof(int),
				typeof(ColorPicker),
				new PropertyMetadata(
					0,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public Color? PreviousColor
		{
			get => (Color?)GetValue(PreviousColorProperty);
			set => SetValue(PreviousColorProperty, value);
		}

		public static DependencyProperty PreviousColorProperty { get ; } =
			DependencyProperty.Register(
				nameof(PreviousColor),
				typeof(Color?),
				typeof(ColorPicker),
				new PropertyMetadata(
					null,
					(s, e) => (s as ColorPicker)?.OnPropertyChanged(e)));

		public event Windows.Foundation.TypedEventHandler<ColorPicker, ColorChangedEventArgs> ColorChanged;
	}
}
