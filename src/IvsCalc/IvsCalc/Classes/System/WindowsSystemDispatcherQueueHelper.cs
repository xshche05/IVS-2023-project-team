using System;
using System.Runtime.InteropServices;

namespace IvsCalc.Classes.System;

/// <summary>
/// Helper class to ensure DispatcherQueueController for the current thread.
/// </summary>
class WindowsSystemDispatcherQueueHelper {
    [StructLayout(LayoutKind.Sequential)]
    struct DispatcherQueueOptions {
        internal int dwSize;
        internal int threadType;
        internal int apartmentType;
    }

    [DllImport("CoreMessaging.dll")]
    private static extern unsafe int CreateDispatcherQueueController(DispatcherQueueOptions options, IntPtr* instance);

    IntPtr _dispatcherQueueController = IntPtr.Zero;
    
    /// <summary>
    /// Ensures that the current thread has a DispatcherQueueController.
    /// </summary>
    public void EnsureWindowsSystemDispatcherQueueController() {
        if (Windows.System.DispatcherQueue.GetForCurrentThread() != null) {
            // one already exists, so we'll just use it.
            return;
        }

        if (_dispatcherQueueController == IntPtr.Zero) {
            DispatcherQueueOptions options;
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType = 2;    // DQTYPE_THREAD_CURRENT
            options.apartmentType = 2; // DQTAT_COM_STA

            unsafe {
                IntPtr dispatcherQueueController;
                CreateDispatcherQueueController(options, &dispatcherQueueController);
                _dispatcherQueueController = dispatcherQueueController;
            }
        }
    }
}