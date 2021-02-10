# Uno Support for Windows.Devices.Haptics

## `VibrationDevice`

The `RequestAccessAsync` method is implemented on all platforms and returns `Allowed` on all platforms, except for Android and Tizen. In case of Android, the `android.permission.VIBRATE` permission needs to be declared. In case of Tizen, the `http://tizen.org/privilege/haptic` privilege needs to be declared.

The `GetDefaultAsync` method is implemented on all platforms and returns `null` for the unsupported platforms (WPF, GTK).

## `SimpleHapticsController`

The `SupportedFeedback` property returns the list of supported feedback types for the given platform. In most cases this includes `Click` and `Press`.

The following code snippet illustrates the usage of `VibrationDevice` and `SimpleHapticsController`:

```
var result = await VibrationDevice.RequestAccessAsync();
if (result == VibrationAccessStatus.Allowed)
{
    var vibrationDevice = await VibrationDevice.GetDefaultAsync();
    if (vibrationDevice != null)
    {
        var simpleHapticsController = vibrationDevice.SimpleHapticsController;
        var feedbackType = simpleHapticsController.SupportedFeedback.FirstOrDefault(
            feedback => feedback.Waveform == KnownSimpleHapticsControllerWaveforms.Press);
        if (feedbackType != null)
        {
            simpleHapticsController.SendHapticFeedback(feedbackType);
        }
    }
}
```