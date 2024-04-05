using System;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Xaml;

namespace IvsCalc.Classes.System {
    /// <summary>
    /// There is an existing (though closed issue) of titlebar not changing color when a custom titlebar is set.
    /// This happens because someone decided to hard-code caption colors in WinUI resources.
    /// See issue <a href="https://github.com/microsoft/microsoft-ui-xaml/issues/7125">microsoft-ui-xaml#7125</a>
    /// </summary>
    /// <remarks>
    /// An entire class just to fix Microsoft's mess...
    /// </remarks>
    internal class TitleBarHelper {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        private const int WM_ACTIVATE = 0x0006;
        private const int WA_ACTIVE = 0x01;
        private const int WA_INACTIVE = 0x00;

        /// <summary>
        /// Trigger a repaint of the titlebar for <paramref name="window"/>
        /// </summary>
        /// <param name="window">Target window</param>
        private static void TriggerTitleBarRepaint(Window window) {
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            IntPtr activeWindow = GetActiveWindow();

            if (hwnd == activeWindow) {
                SendMessage(hwnd, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
                SendMessage(hwnd, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
            } else {
                SendMessage(hwnd, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
                SendMessage(hwnd, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Changes window chrome icons color to match the system theme
        /// </summary>
        /// <param name="window">Target window</param>
        /// <returns></returns>
        public static Windows.UI.Color ApplySystemThemeToCaptionButtons(Window window) {
            ResourceDictionary res = Application.Current.Resources;

            Windows.UI.Color color;

            if (Application.Current.RequestedTheme == ApplicationTheme.Dark) {
                color = Colors.White;
            } else {
                color = Colors.Black;
            }

            UpdateTitleBarColors(window, color);

            return color;
        }

        /// <summary>
        /// Sets correct colors for the titlebar and triggers a repaint
        /// </summary>
        /// <param name="window">Target window</param>
        /// <param name="color">Foreground color</param>
        public static void UpdateTitleBarColors(Window window, Windows.UI.Color color) {
            ResourceDictionary res = Application.Current.Resources;
            res["WindowCaptionForeground"] = color;
            res["WindowCaptionBackground"] = Colors.Transparent;
            res["WindowCaptionBackgroundDisabled"] = Colors.Transparent;
            TriggerTitleBarRepaint(window);
        }

    }
}
