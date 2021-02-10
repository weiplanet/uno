﻿#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.Content;

namespace Windows.Phone.Devices.Notification
{
	public partial class VibrationDevice
	{
		private const string Permission = "android.permission.VIBRATE";
		private static VibrationDevice _instance;
		private static bool _initializationAttempted;

		private readonly Vibrator _vibrator;

		private VibrationDevice(Vibrator vibrator) =>
			_vibrator = vibrator;

		public static VibrationDevice GetDefault()
		{
			if (!_initializationAttempted && _instance == null)
			{
				if (ContextCompat.CheckSelfPermission(Application.Context, Permission) == Android.Content.PM.Permission.Denied)
				{
					throw new InvalidOperationException($"{Permission} needs to be declared in AndroidManifest.xml");
				}
				var vibrator = Application.Context.GetSystemService(Context.VibratorService) as Vibrator;
				if (vibrator != null && vibrator.HasVibrator)
				{
					_instance = new VibrationDevice(vibrator);
				}
				_initializationAttempted = true;
			}
			return _instance;
		}

		public void Vibrate(TimeSpan duration)
		{
			var time = (long)duration.TotalMilliseconds;
			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				_vibrator.Vibrate(VibrationEffect.CreateOneShot(time, VibrationEffect.DefaultAmplitude));
			}
			else
			{
#pragma warning disable CS0618 // Type or member is obsolete
				_vibrator.Vibrate(time);
#pragma warning restore CS0618 // Type or member is obsolete
			}
		}

		public void Cancel() => _vibrator.Cancel();
	}
}
#endif
