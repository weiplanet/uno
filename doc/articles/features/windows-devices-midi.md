# Uno Support for Windows.Devices.Midi

MIDI device input and output is supported across iOS, Android, macOS, and WebAssembly.

To see how to implement MIDI support in your application, you can follow the sample project provided in the [UWP samples repository](https://github.com/microsoft/Windows-universal-samples/tree/master/Samples/MIDI). This shows MIDI device enumeration, various forms of input as well as output.

In case of **WebAssembly**, additional security check is required to be able to receive/send system exclusive MIDI messages. If you require this functionality, please set the `WinRTFeatureConfiguration.Midi.RequestSystemExclusiveAccess` flag early in the application lifecycle.

```C#
#if __WASM__
WinRTFeatureConfiguration.Midi.RequestSystemExclusiveAccess = true;
#endif
```