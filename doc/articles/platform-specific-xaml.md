# Platform-specific XAML markup in Uno

Uno allows you to reuse views and business logic across platforms. Sometimes though you may want to write different code per platform, either because you need to access platform-specific native APIs and 3rd-party libraries, or because you want your app to look and behave differently depending on the platform. 

This guide covers multiple approaches to managing per-platform markup in XAML. See [this guide for managing per-platform C#](platform-specific-csharp.md).

## Project structure

There are two ways to restrict code or XAML markup to be used only on a specific platform:
 * Use conditionals within a shared file
 * Place the code in a file which is only included in the desired platform head.
 
 The structure of an Uno app created with the default [Visual Studio template](https://marketplace.visualstudio.com/items?itemName=nventivecorp.uno-platform-addin) is [explained in more detail here](uno-app-solution-structure.md). The key point to understand is that files in a shared project referenced from a platform head **are treated in exactly the same way** as files included directly under the head, and are compiled together into a single assembly.

## XAML conditional prefixes

The Uno platform uses pre-defined prefixes to include or exclude parts of XAML markup depending on the platform. These prefixes can be applied to XAML objects or to individual object properties.

Conditional prefixes you wish to use in XAML file must be defined at the top of the file, like other XAML prefixes. They can be then applied to any object or property within the body of the file.

For prefixes which will be excluded on Windows (e.g. `android`, `ios`), the actual namespace is arbitrary, since the Uno parser ignores it. The prefix should be put in the `mc:Ignorable` list. For prefixes which will be included on Windows (e.g. `win`, `not_android`) the namespace should be `http://schemas.microsoft.com/winfx/2006/xaml/presentation` and the prefix should not be put in the `mc:Ignorable` list.

### Examples

##### Example 1

Using the following XAML:

```xaml
<Page x:Class="HelloWorld.MainPage"
	  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	  xmlns:android="http://uno.ui/android"
	  xmlns:ios="http://uno.ui/ios"
	  xmlns:wasm="http://uno.ui/wasm"
	  xmlns:win="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	  xmlns:not_android="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	  xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
	  xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
	  mc:Ignorable="d android ios wasm">

	<StackPanel Margin="20,70,0,0">
		<TextBlock Text="This text will be large on Windows, and pink on WASM"
				   win:FontSize="24"
				   wasm:Foreground="DeepPink"
				   TextWrapping="Wrap"/>
		<TextBlock android:Text="This version will be used on Android"
				   not_android:Text="This version will be used on every other platform" />
		<ios:TextBlock Text="This TextBlock will only be created on iOS" />
	</StackPanel>
</Page>
```

Results in:

![Visual output](Assets/platform-specific-xaml.png)

In this example note how the properties `FontSize` and `Foreground` are selectively used based on platform. The `TextBlock` property `Text` also has two different values based on whether or not the app is running in Android. Finally, an entire `TextBlock` is added if the app is running in iOS. This shows:
 
 1. How certain properties can be used based on the platform
 2. How the values of certain properties can be changed based on the platform
 3. How entire controls can be added or removed for certain platforms

#### Example 2

Platform-specific XAML also allows you to exclude a parent element and all its children. This is especially useful for cases where children are already part of a namespace but need to be excluded on certain platforms.

Consider the following XAML which is using the Windows Community Toolkit's [Blur](https://docs.microsoft.com/en-us/windows/communitytoolkit/animations/blur) animation. While this runs for UWP, it is not currently supported in the Uno Platform and needs to be conditionally disabled. It isn't possible to add something like `<win:interactivity:Interaction.Behaviors>` to disable the behavior itself. Instead, the entire `Grid` is disabled on any platforms except Windows and child elements will be disabled along with it.

```xaml
<Page x:Class="HelloWorld.MainPage"
	  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	  xmlns:behaviors="using:Microsoft.Toolkit.Uwp.UI.Animations.Behaviors"
	  xmlns:toolkit="using:Microsoft.Toolkit.Uwp.UI.Controls"
	  xmlns:interactivity="using:Microsoft.Xaml.Interactivity"
	  xmlns:win="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	  xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
	  xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
	  mc:Ignorable="d android ios wasm">

	<Grid>
		<win:Grid Background="Gray">
			<interactivity:Interaction.Behaviors>
				<behaviors:Blur Value="7" Duration="0" Delay="0" AutomaticallyStart="True" />
			</interactivity:Interaction.Behaviors>
		</win:Grid>
		<Grid>
			<!-- Other page content -->
		</Grid>
	</Grid>
</Page>
```

### Available prefixes

The pre-defined prefixes are listed below:

| Prefix        | Included platforms                 | Excluded platforms                 | Namespace                                                   | Put in `mc:Ignorable`? |
|---------------|------------------------------------|------------------------------------|-------------------------------------------------------------|------------------------|
| `win`         | Windows                            | Android, iOS, web, macOS, Skia     | `http://schemas.microsoft.com/winfx/2006/xaml/presentation` | no                     |
| `xamarin`     | Android, iOS, web, macOS, Skia     | Windows                            | `http://uno.ui/xamarin`                                      | yes                    |
| `not_win`     | Android, iOS, web, macOS, Skia     | Windows                            | `http://uno.ui/not_win`                                      | yes                    |
| `android`     | Android                            | Windows, iOS, web, macOS, Skia     | `http://uno.ui/android`                                      | yes                    |
| `ios`         | iOS                                | Windows, Android, web, macOS, Skia | `http://uno.ui/ios`                                          | yes                    |
| `wasm`        | web                                | Windows, Android, iOS, macOS, Skia | `http://uno.ui/wasm`                                         | yes                    |
| `macos`       | macOS                              | Windows, Android, iOS, web, Skia   | `http://uno.ui/macos`                                        | yes                    |
| `skia`        | Skia                               | Windows, Android, iOS, web, macOs  | `http://schemas.microsoft.com/winfx/2006/xaml/presentation` | no                     |
| `not_android` | Windows, iOS, web, macOS, Skia     | Android                            | `http://schemas.microsoft.com/winfx/2006/xaml/presentation` | no                     |
| `not_ios`     | Windows, Android, web, macOS, Skia | iOS                                | `http://schemas.microsoft.com/winfx/2006/xaml/presentation` | no                     |
| `not_wasm`    | Windows, Android, iOS, macOS, Skia | web                                | `http://schemas.microsoft.com/winfx/2006/xaml/presentation` | no                     |
| `not_macos`   | Windows, Android, iOS, web, Skia   | macOS                              | `http://schemas.microsoft.com/winfx/2006/xaml/presentation` | no                     |
| `not_skia`    | Windows, Android, iOS, web, macOS  | Skia                               | `http://schemas.microsoft.com/winfx/2006/xaml/presentation` | no                     |

More visually, platform support for the pre-defined prefixes is shown in the below table:

| Prefix        |  Win  | Droid |  iOS  |  Web  | macOS | Skia  | 
|---------------|-------|-------|-------|-------|-------|-------|
| `win`         | ✔ | ✖ | ✖ | ✖ | ✖ | ✖ |
| `android`     | ✖ | ✔ | ✖ | ✖ | ✖ | ✖ |
| `ios`         | ✖ | ✖ | ✔ | ✖ | ✖ | ✖ |
| `wasm`        | ✖ | ✖ | ✖ | ✔ | ✖ | ✖ |
| `macos`       | ✖ | ✖ | ✖ | ✖ | ✔ | ✖ |
| `skia`        | ✖ | ✖ | ✖ | ✖ | ✖ | ✔ |
| `xamarin`     | ✖ | ✔ | ✔ | ✔ | ✔ | ✔ |
| `not_win`     | ✖ | ✔ | ✔ | ✔ | ✔ | ✔ |
| `not_android` | ✔ | ✖ | ✔ | ✔ | ✔ | ✔ |
| `not_ios`     | ✔ | ✔ | ✖ | ✔ | ✔ | ✔ |
| `not_wasm`    | ✔ | ✔ | ✔ | ✖ | ✔ | ✔ |
| `not_macos`   | ✔ | ✔ | ✔ | ✔ | ✖ | ✔ |
| `not_skia`    | ✔ | ✔ | ✔ | ✔ | ✔ | ✖ |

Where:
 * 'Win' represents Windows, and
 * 'Droid' represents Android

### XAML prefixes in cross-targeted libraries
For Uno 3.0 and above, Xaml prefixes behave differently in class libraries than when used directly in application code. Specifically, it isn't possible to distinguish Skia and Wasm in a library, since both platforms use the .NET Standard 2.0 target. The `wasm` and `skia` prefixes will always evaluate to false inside of a library.

The prefix `netstdref` is available and will include the objects or properties in both Skia and Wasm build. A prefix `not_nestdref` can also be used to exclude them. Since Skia and Wasm are similar, it is often not necessary to make the distinction. 

In cases where it is needed (fonts are one example) then the XAML files must be placed directly in the platform specific project or a shared project.

| Prefix          | Namespace                                                   | Put in `mc:Ignorable`? |
|-----------------|-------------------------------------------------------------|------------------------|
| `netstdref`     | `http://uno.ui/netstdref`                                   | yes                    |
| `not_netstdref` | `http://uno.ui/not_netstdref`                               | yes                    |
