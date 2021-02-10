﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.UI.Core;
using Private.Infrastructure;

namespace MUXControlsTestApp.Utilities
{
	internal class IdleSynchronizer
	{
		public static void Wait()
		{
#if __WASM__
			if (!CoreDispatcher.Main.IsThreadingSupported)
			{
				return;
			}
#endif
			TestServices.WindowHelper.WaitForIdle().Wait(TestUtilities.DefaultWaitMs);
		}
	}
}
