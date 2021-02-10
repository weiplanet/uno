﻿# Developing with Uno.UI

## Pre-requisites

* [**Visual Studio 2019 16.3 or later**](https://visualstudio.microsoft.com/), with :
	* Xamarin component, with the iOS Remote Simulator installed
	* A working Mac with Visual Studio for Mac, XCode 8.2 or later installed
	* The google Android x86 emulators

Visual Studio for Mac is supported, but the editing capabilities are currently limited.

## Create a new project

Follow the instructions in the [Getting Started guide](get-started.md) to use the Uno app template to create a new app in Visual Studio. See the [Uno.Quickstart](https://github.com/unoplatform/uno.QuickStart) repository for a simple example of a 'Hello World' application.

## General guidelines for developing with Uno.UI

* **Always develop for Windows UWP first**. Uno.UI uses the same namespaces and 
conventions as UWP's XAML, which allows for a better use of Visual Studio
debugging tools such as Xaml Edit and Continue, and C# Edit and Continue. The compilation is also much faster, making the development inner loop more efficient.
* When creating platform specific XAML, **wrap it inside of
a user control**, or a control style. This makes for cleaner code when the control is used.
* When writing platform-specific code, **prefer to create [partial classes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods) and avoid conditional 
code that uses #if pre-compiler directives**. If the implementation becomes too large, consider
abstracting the implementation using a common interface.
* A platform-specific code file **should have an appropriate suffix** (.android.cs, .ios.cs, .uwp.cs, .xamarin.cs, ...)
* Uno.UI is not a perfect implementation of UWP's XAML, which means that there will be compatibility issues. When you 
encounter one, a few approaches can be taken:
 1. **Always [report the issue](https://github.com/unoplatform/uno/issues) to the Uno.UI maintainers**. This may be a known issue, for which there may be known 
    workarounds or guidance on how to handle the issue.
 1. **Try to find a UWP-compatible workaround**, possibly non-breaking, meaning that the added Xaml produces the
    same behavior for all platforms, even if it does not conform to the expected UWP behavior.
 1. **Make the Xaml code conditional to Uno.UI**, using [xml namespaces](platform-specific-xaml.md). Note that using 
    this technique exposes the app's code to breaking changes.

## Bootstrapping Uno.UI

### Enable console logging

Uno provides a simple logging infrastructure through the Uno.Logging namespace.

The LogManager is the entry point to get the loggers, and Uno.UI provides a default Console
logger that may output content in Android's LogCat, or iOS' syslog.

Here's how to enable it :

```csharp
	Uno.Logging.LogManager.LoggerSelector = Uno.Logging.ConsoleLogger.Instance.GetLog;
	Uno.Logging.ConsoleLogger.Instance.LogLevel = Uno.Logging.LogLevel.Warn;

	Uno.Logging.ConsoleLogger.Instance.Filter = (level, name, message, exception) =>
	{
        // Provides a list of loggers that should be displayed to the console
		var passList = new[] {
			"Windows.UI.Xaml.VisualStateManager",
			"Windows.UI.Xaml.VisualStateGroup",
			"Windows.UI.Xaml.Controls.RadioButton",
		};

		return passList.Any(n => name.StartsWith(n))
			? Uno.Logging.ConsoleLogger.FilterResult.Pass : Uno.Logging.ConsoleLogger.FilterResult.Ignore;
	};
```

Note that enabling debug logging may significantly slow down the application. It is strongly 
suggested to keep the logging level to **LogLevel.Warn** for routine development.

## Troubleshooting Uno.UI Xaml pages 

When building, the Uno.UI code generator produces .cs files that are
located in the **MyApp.[iOS|Android]\Obj\[Platform]** folder.

You may see those files in Visual Studio by selecting this project in the Solution Explorer, then click
the **Show all Files** icon at the top.

If you notice an issue, or an error in the commented code of the generated file, you may need to alter your Xaml file.

## Configure the manifest for the WebAssembly head
In your WASM head, create a folder named `WasmScripts`, with a file containing the Javascript code below
(e.g. `AppManifest.js`) and the `Embedded resource` build action.

The manifest file should contain the following:

```javascript
var UnoAppManifest = {

    splashScreenImage: "Assets/AppSplashScreen.scale-200.png",
    splashScreenColor: "#3750D1",
    displayName: "My Sample App"

}
```

The properties are :
* **splashScreenImage**: defines the image that will be centered on the window during the application's loading time.
* **splashScreenColor**: defines the background color of the splash screen.
* **displayName**: defines the default name of the application in the browser's window title.

## Supporting multiple platforms in Xaml files

See [here for a detailed guide to authoring platform-specific XAML code](platform-specific-xaml.md).

## Uno.UI Layout Behavior

The layout behavior is the notion of applying margins, paddings and alignments for a control inside its parent. In UWP/WPF, the code responsible for this behavior is located in the FrameworkElement class. This means that any control can alter its own rendering position inside its parent, and place its content accordingly, regardless of the parent type.

The Uno.UI layout engine on Android and iOS is applied by a parent to its children. This means that if a control has an alignment or a margin set, if it is not child of a FrameworkElement (i.e. it's the child of a non-Uno view), those properties will be ignored, and the control will stretch within its parent's available space.

This behavior is a direct consequence of the ability to mix native and Uno.UI controls.

## Dependency Properties

Uno.UI allows the sharing of [Dependency Property](https://docs.microsoft.com/en-us/windows/uwp/xaml-platform/dependency-properties-overview) declaration and code between Windows and Xamarin based platforms.

Declaring a dependency property in Uno UI requires a class to implement the interface `DependencyObject`, to gain access to the GetValue and SetValue methods.

Here is an example of such a declaration:

```csharp
    public class ControlTemplateTest : Control  
    {  
       public string MyCustomContent  
       {  
          get { return (string)GetValue(MyCustomContentProperty); }  
          set { SetValue(MyCustomContentProperty, value); }  
       }
    
       public static readonly DependencyProperty MyCustomContentProperty =  
             DependencyProperty.Register("MyCustomContent", typeof(string), typeof(ControlTemplateTest), new PropertyMetadata(null));  
    }
```

In Visual Studio, this code can be created using the `propdp`
[code snippet](https://msdn.microsoft.com/en-us/library/ms165392.aspx).

## Creating a Control Template  

Uno.UI provides the ability to create control templates, which provide another way of creating multi-platform user interfaces.

For the following control class:  

```csharp
    public class MyUserControl : Control  
    {  
        public MyUserControl()
        {
            InitializeComponent();
        }
        
        public string MyCustomContent  
        {  
            get { return (string)GetValue(MyCustomContentProperty); }  
            set { SetValue(MyCustomContentProperty, value); }  
        }
    
        public static readonly DependencyProperty MyCustomContentProperty =  
             DependencyProperty.Register("MyCustomContent", typeof(string), typeof(MyUserControl), new PropertyMetadata(null));  
    }
```

Which exposes a `MyCustomContent` property, it is then possible to apply a template that uses template bindings:

```xml
    <UserControl
        x:Class="GenericApp.Views.Pages.Main.MyUserControl"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="using:GenericApp.Views.Pages.Main"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d"
        d:DesignHeight="300"
        d:DesignWidth="400">
     <UserControl.Template>
      <ControlTemplate>
       <StackPanel>
        <TextBlock Text="My user control" />
        <TextBlock Text="{TemplateBinding MyCustomContent}" />
       </StackPanel>
      </ControlTemplate>
     </UserControl.Template>
    
    </UserControl>
```

Where the bindings applied to the control on the MyHeader property will be propagated to the template binding automatically.

## Using Static Resources

Uno.UI currently supports Static Resources using a two-level scope resolution.

*   If a static resource is declared in a Xaml file for which the root element **is not** a ResourceDictionary, the static resources will only be available in this file.
*   If a static resource is declared in a Xaml file for which the root element **is** a ResourceDictionary, the static resource will be available for all Xaml files of the application.

The local scope takes precedence over the global scope.

Uno.UI generates a file named GlobalStaticResources, which contains:

*   Static members for all the available resources.
*   A method called FindResource which takes a resource name to find.

Uno.UI also generates a nested class named StaticResources in all non-ResourceDictionary Xaml files, using the same static members and FindResource method.

## Using Styles

Uno.UI supports the [authoring of styles](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.style.aspx).

For many controls in Uno, two prepackaged styles are provided:
* NativeDefault[Control] which is customized to match the UI guidelines of the target platform.
* XamlDefault[Control] which is the default style of controls on Windows.

An application can set native styles as the default for supported controls by setting the static flag `Uno.UI.FeatureConfiguration.Style.UseUWPDefaultStyles = false;` somewhere in app code (eg, the `App.xaml.cs` constructor). It's also possible to configure only certain controls to default to the native style, in the following manner: `Uno.UI.FeatureConfiguration.Style.UseUWPDefaultStylesOverride[typeof(Frame)] = false;`

Third-party libraries can define native variants of default styles for custom controls, using the `xamarin:IsNativeStyle="True"` tag in Xaml. These will be used if the consuming application is configured to use native styles.

On WASM, the NativeDefault[Control] styles are currently only aliases to the XamlDefault[Control], for code compatibility with other platforms.

## Localization

Localization is done through the `resw` files in the current project. Resources are then referenced using `x:Uid`.

See the UWP documentation on [localizing strings in your UI](https://docs.microsoft.com/en-us/windows/uwp/app-resources/localize-strings-ui-manifest).

Resources may be placed in the default scope files `Resources.resw`, or in a custom named file. Custom named file content
can be used with the `x:Uid="/myResources/MyResource"` format, see [how to factor strings into multiple resource files](https://docs.microsoft.com/en-us/windows/uwp/app-resources/localize-strings-ui-manifest#factoring-strings-into-multiple-resources-files).

Note that the default language can be defined using the `DefaultLanguage` msbuild property, using an IETF Language Tag (e.g. `en` or `fr-FR`).

## Supported Uno.UI Controls

### Grid

The Grid control is implemented as a best effort in terms of compatibility with UWP.

For more information, see the [Grid class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.grid.aspx).

### Border

See the [Border control](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.border.aspx).

### Button

The Button control is implemented by default using a ControlTemplate that contains a bindable native button that binds that the Content property as a string, and propagates the CanExecute of a databound command.

For more information, see the [Button class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.button.aspx).

### CheckBox

The CheckBox control is implemented by default using a ControlTemplate that contains a bindable native CheckBox that binds the Content property as a string, IsChecked as a boolean, and propagates the CanExecute of a databound command.

For more information, see the [CheckBox class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.checkbox.aspx).

### HyperLinkButton

For more information, see the [HyperLinkButton](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.hyperlinkbutton.aspx) class.

### RadioButton

The RadioButton control is implemented by default using a ControlTemplate that contains a bindable native CheckBox that binds the Content property as a string, IsChecked as a boolean, and propagates the CanExecute of a databound command.

For more information, see the [RadioButton class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.radiobutton.aspx).

### ComboBox

The ComboBox control is implemented by default using a ControlTemplate that contains a bindable native CheckBox that binds that the Content property as a string, IsChecked as a boolean, and propagates the CanExecute of a databound command.

For more information, see the [ComboBox class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.combobox.aspx).

### ContentControl

For more information, see the [ContentControl class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.contentcontrol.aspx).

### ContentPresenter

For more information, see the [ContentPresenter class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.contentpresenter.aspx).

### Control

For more information, see [Control class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.control.aspx).

### GridView

For more information, see [GridView class](https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.gridview).

### Image

For more information, see [Image class](https://msdn.microsoft.com/en-us/library/window/apps/windows.ui.xaml.controls.image.aspx).


### ImageSource

Image handling works 'out of the box' on iOS and WebAssembly without further configuration required.

On Android, to handle the loading of images from a remote url, the Image control has to be provided a 
ImageSource.DefaultImageLoader such as the [Android Universal Image Loader](https://github.com/nostra13/Android-Universal-Image-Loader).

This package is installed by default when using the [Uno Cross-Platform solution templates](https://marketplace.visualstudio.com/items?itemName=nventivecorp.uno-platform-addin). If not using the solution template, you can install the [nventive.UniversalImageLoader](https://www.nuget.org/packages/nventive.UniversalImageLoader/) NuGet package and call the following code from your application's App constructor:

```csharp
private void ConfigureUniversalImageLoader()
{
	// Create global configuration and initialize ImageLoader with this config
	ImageLoaderConfiguration config = new ImageLoaderConfiguration
		.Builder(Context)
		.Build();

	ImageLoader.Instance.Init(config);

	ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
}
```

On iOS, bundle images can be selected using "bundle://" (e.g. bundle:///SplashScreen). When selecting the bundle resource, do not include the zoom factor, nor the file extension.

The `ms-appx:///` format is also supported for local asset resolution.

### ItemsControl

For more information, see [ItemsControl class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.itemscontrol.aspx).

### ListView

For more information, see [ListView class](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.listview.aspx).

### Page

For more information, see the [Page](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.page.aspx) class.

### Panel

For more information, see the [Panel](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.panel.aspx) class.

When implementing a custom `Panel` or `FrameworkElement` via `MeasureOverride` and `ArrangeOverride`, measuring 
and arranging the children must be done through `MeasureElement` and `ArrangeElement`. This is different from UWP where one should 
call `element.Measure` and `element.Arrange`.

### PasswordBox

For more information, see the [PasswordBox](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.passwordbox.aspx) class.

### StackPanel

For more information, see the [StackPanel](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.stackpanel.aspx) class.

### TextBlock

The Uno.UI TextBlock supports the Text property as well as the
 [Inlines](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.textblock.inlines.aspx) property
 using [Runs](https://msdn.microsoft.com/en-us/library/windows/apps/xaml/windows.ui.xaml.documents.run.aspx).

For more information, see the [TextBlock](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.textblock.aspx) class.

### Custom Fonts

For more information, see the [Custom fonts documentation](features/custom-fonts.md).

### TextBox

For more information, see the [TextBox](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.textbox.aspx) class.

### UserControl

For more information, see the [UserControl](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.usercontrol.aspx) class.

### WebView

For more information, see the [WebView](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.aspx) class.

### WrapPanel

Shows a panel's items in a wrapping structure. For more information, see the [WrapPanel class](https://msdn.microsoft.com/en-us/library/cc295081.aspx).

### Controlling Orientation

The application's orientation can be controlled using the [DisplayInformation](https://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.display.displayinformation.aspx) class, more specifically, the static [AutoRotationPreferences](https://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.display.displayinformation.autorotationpreferences) property.

#### Controlling Orientation on iOS 

In order for the DisplayInformation's AutoRotationPreferences to work properly, you need to ensure that all potential orientations are supported within the iOS application's `info.plist` file.

**Warning for iOS 9+ Development**
As of iOS 9, the system does not allow iPad applications to dictate their orientation if they support [Multitasking / Split View](https://developer.apple.com/library/prerelease/content/documentation/WindowsViews/Conceptual/AdoptingMultitaskingOniPad/). In order to control orientation through the DisplayInformation class, you will need to opt-out of Multitasking / Split View by ensuring that you have defined the following in your `info.plist`:

```xml
<key>UIRequiresFullScreen</key>
<true/>
```

### Controlling the Keyboard on iOS

If a view needs to keep the keyboard opened when tapping on it, use the `Uno.UI.Controls.Window.SetNeedsKeyboard` attached property.

## Creating/Using Android Activities
At the root of every Android Uno app, lies a `BaseActivity` class that extends from `Android.Support.V7.App.AppCompatActivity` which is part of the [Android v7 AppCompat Support Library](https://developer.android.com/topic/libraries/support-library/features.html#v7-appcompat). If you ever need to create a new Activity within your app or within Uno you must be sure to extend `BaseActivity` and, if you need to apply a Theme to the activity, ensure that the Theme you set is a `Theme.AppCompat` theme (or descendant).

### Using the ILLinker for WebAssembly

For more information about using the IL Linker for WebAssembly, read this [article](features/using-il-linker-webassembly.md).
