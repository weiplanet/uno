﻿using System;
using System.Runtime.InteropServices;
using CoreFoundation;

namespace Uno.Helpers
{
    internal static class IOKit
    {
        private const string IOKitLibrary = "/System/Library/Frameworks/IOKit.framework/IOKit";
        private const uint kIOPMAssertionLevelOn = 255;

        private static readonly CFString kIOPMAssertionTypePreventUserIdleDisplaySleep = "PreventUserIdleDisplaySleep";

        [DllImport(IOKitLibrary)]
        static extern uint IOPMAssertionCreateWithName(IntPtr type, uint level, IntPtr name, out uint id);

        [DllImport(IOKitLibrary)]
        static extern uint IOPMAssertionRelease(uint id);

        internal static bool PreventUserIdleDisplaySleep(CFString name, out uint id)
        {
            var result = IOPMAssertionCreateWithName(
                kIOPMAssertionTypePreventUserIdleDisplaySleep.Handle,
                kIOPMAssertionLevelOn,
                name.Handle,
                out var newId);

            id = result == 0 ? newId : 0;

            return result == 0;
        }

        internal static bool AllowUserIdleDisplaySleep(uint id)
        {
            var result = IOPMAssertionRelease(id);
            return result == 0;
        }
    }
}
