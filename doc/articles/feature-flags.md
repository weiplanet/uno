# Configuring Uno's behavior globally

Uno provides a set of feature flags that can be set early in an app's startup to control its behavior. Some of these flags are for backward compatibility, some of them provide fine-grained customizability of a particular feature, and some of them allow to toggle between more 'WinUI-like' and more 'native-like' behavior in a particular context.

# Legacy Clipping
Historically, Uno has been relying on the default platform's behavior for clipping, which is quite different from UWP compositing behavior.

By default, this mode is enabled for the time being, as it is quite disrupting.

Use `Uno.UI.FeatureConfiguration.UIElement.UseLegacyClipping` to control this mode.
Additionally, `Uno.UI.FeatureConfiguration.UIElement.ShowClippingBounds` can be used to show the clipping boundaries to determine if the behavior of the clipping is appropriate.

# UWP Styles default

By default, Uno favors the default UWP XAML styles over the native styles for Button, Slider, ComboBox, etc...

This can be changed using `Uno.UI.FeatureConfiguration.Style.UseUWPDefaultStyles`.

## Disabling accessibility text scaling

By default, Uno automatically enables accessibility text scaling on iOS and Android devices however to have more control an option has been added to disable text scaling. 

Use `Uno.UI.FeatureConfiguration.Font.IgnoreTextScaleFactor` to control this. 
