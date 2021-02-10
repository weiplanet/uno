# ElevatedView
In many design languages (like [_Material Design_](https://material.io/design)), there's a notion of 'elevation' where a portion of the UI should be presented as being _elevated_ over the rest of the content.

In this case, UWP's native elevation API can't work on all platforms because of technical limitations. To address this problem, Uno provides a control called `ElevatedView`, able to produce a similar elevated effect on all platforms (UWP, Android, iOS, WebAssembly, and macOS).

This control is very useful to create cards with both rounded corners and an elevated effect - which could otherwise be challenging to produce on some platforms.

## How to use the `ElevatedView`

First you need to add the `toolkit` namespace in your XAML file:

```
xmlns:toolkit="using:Uno.UI.Toolkit"
```

After that, use the `ElevatedView` to host the content you need to be elevated:
``` xml
<StackPanel Orientation="Horizontal" Spacing="20">

	<Button>Non-Elevated Button</Button>

	<toolkit:ElevatedView Elevation="10">
		<Button>Elevated Button</Button>
	</toolkit:ElevatedView>

</StackPanel>
```

Will produce the following result:

![ElevatedView sample](../Assets/features/elevatedview/elevatedview-sample.png)

> **ATTENTION FOR UWP**: When there is an error seeing the `<toolkit:ElevatedView>` on UWP, the common mistake is to forget to include the `Uno.UI` package for all platforms, including UWP. On UWP, the only component that the `Uno.UI` package adds is the Toolkit.

## Settings

You can set the following properties:

* `Elevation`: numeric number representing the level of the elevation effect. Typical values are between 5 and 30. The default is `0` - no elevation.
* `ShadowColor`: By default the shadow will be `Black`, but you can set any other value. You can reduce the shadow effect by using the alpha channel [except Android]. On Android, the shadow color can only be changed since Android Pie (API 28+). The default is `Black` with alpha channel at 25%.
* `Background`: The default is `Transparent`. Setting `null` will remove the shadow effect.
* `CornerRadius`: Use it to create rounded corner effects. The shadow will follow them.

## Particularities

* Make sure to _give room_ for the shadow in the layout (eg by setting a `Margin` on the `ElevatedView`).  Some platforms like macOS may clip the shadow otherwise. For the same reason, avoid wrapping the `<toolkit:ElevatedView>` directly in a `<ScrollViewer> ` because it's designed to clip its content.


