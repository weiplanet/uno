# Uno features

## Development flow

- Uno's UWP Xaml
    - **Windows-first development**
    - **Faster compilation** for most of the development cycle, including UI code. iOS and Android can be tested for platform-specific features.
    - Ability to develop responsive layout in Windowed mode using Windows 10.
    - Allows for **edit and continue** support from VS2017+
    - Allows for **UI and Non-UI code edit and continue** support
- Conditional platform features access
    - All XAML controls have **access to native properties** through conditional XAML
    - Ability to **add native controls in XAML**
- Full UWP availability at compile time
  - Allows for the **compilation of open-source libraries** that depend only on UWP and multi-targeted nuget packages
    - **XamlBehaviors library** support
- MacOS Compilation

## Features list

### Animations

- Animations on any DependencyProperty and AttachedProperty
- Visual States (declared animations, advanced setters)
- Visual State Groups (multi-state controls)
- StateTriggers (Orientation-based responsive design)
- Custom StateTriggers (e.g network state-based states)
- XAML-defined animation storyboards
- Binding expressions to Attached properties (Storyboard setters)

### Styling

- Attached Property Style binding (advanced styling, control reuse)
- Control templating (without the need of renderers) (control reuse, white-labeling)
- TemplateBinding  (control reuse)

### Data Binding

- Value Precedence
- Inheritance
- Mode
- Trigger
- x:Bind *(without phases and expressions)*
- Converters
- Attached Properties binding
- RelativeSource.Self

### Design fidelity
- Text Inlines Binding
- Text Independent trimming and wrapping
- Text Character Spacing
- Text Spans (Bold, Italic, Underline)
- [Path & Shapes support](features/shapes-and-brushes.md)
- Updateable Shapes support
- Image Brush support in shapes
- `FontIcon` support
- Merged Dictionary support

### Responsive design

- Layout constraints [Min/Max][Width/Height]
- Binding SourceTriggers (TextBox immediate vs. focus lost interactions)
- DependencyProperty Inheritance (Color, text style propagation)

### Runtime Performance

- CoreDispatcher Priority support (Large UIs performance)
- `x:DeferLoadStrategy="Lazy"` and `x:Load="false"` support (responsive design performance)
- Image explicit size support (performance)
- Event tracing (sub-millisecond [ETL performance profiling](Assets/diagnostics.PNG))
- Internal logging
- Reflection-less bindings (complex UI performance)
- Binding suspension and restoration
- Expando Binding
- DynamicObject Binding
- DataTemplate and ControlTemplate reuse, pooling and prefetching

### ListView

- `Selector`
- Any `ItemsPanel` support
- `ItemsStackPanel`
- `ItemsWrapGrid`
- Group sticky headers
- Group collection tracking
- Variable item size
- Snap points (with ScrollIntoView support)
- `ISupportIncrementalLoading` support
- `ICollectionView` support with SelectedItem

### Command Bar

- UWP Command bar support
- Native Command bar support (Image, title, back, Opacity, ...)
- Global back button support

### Media

- `SolidColorBrush`
- `ImageBrush`
- `LinearGradientBrush`, with animations support.
- Local assets support with automatic conversion from UWP conventions

### Others

- Native property access from any XAML control
- Localization for any property via x:Uid, using scoped or unscoped `.resw` files (Size localization)
- FocusManager (advanced large forms navigation, dynamic UIs)
- Advanced WebView support (scripting, scrolling, string, custom agent)
- Automatic asset generation from UWP assets
- Native element embedding
- Panels (Grid, StackPanel with `Spacing`, RelativePanel, Canvas)
- Custom Panels
- Popups/Dialogs
- Work with the usual Windows tooling
- Animations
  - Easing functions
  - UWP Theme Transitions
  - Entrance animations
- XAML Behaviors
- AttachedProperty Binding
- AttachedProperty Styling
- Custom `MarkupExtension` support
- Brightness Control
- Native and Custom dialogs
- Support for StateTriggers
- ProgressBar
- Pointer Events
- [Routed Events](features/routed-events.md)
- GeneralTransform.TransformBounds
- Window.Services.Store (Store ID and links)
- Windows.ApplicationModel.Package (InstallDate)
- `ApplicationData.LocalSettings` and `RoamingSettings` support
- Windows.UI.ViewManagement.ApplicationView.VisibleBounds (Rounded corner/notched screen support)  - `SimpleOrientationSensor`
- ApplicationData Folders
- ScrollViewer Snap Points, Extent, Viewport Properties
- ScrollViewer MakeVisible, BringIntoView
- InputPane (Occlusion, Visibility, Events, ...)
- Pointer Capture
- Slider
- FlipView
- Customizable Date and Time Pickers
- Orientation management
- Accessibility (Font Scaling, screen readers)
- Xamarin UITest support and full _Dependency Property_ access
- StatusBar management (occlusion, color, events)
- Runtime XAML reader
- Phased binding (x:Phase)
- XAML code-behind event registration
- WriteableBitmap
- NavigationView
- BitmapIcon
- MediaPlayer
- ViewBox

### Non-UI features

- Windows.UI.Storage (StorageFile, StorageFolder, Settings)
- Windows.UI.Application
- Windows.UI.CoreDispatcher
  - UI Priority dispatch
- Windows.Graphics.Display.DisplayInformation orientation
- Windows.Media.SpeechRecognition
- Windows.Media.Capture.CameraCaptureUI
