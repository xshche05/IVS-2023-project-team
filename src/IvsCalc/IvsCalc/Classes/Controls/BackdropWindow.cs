using IvsCalc.Classes.System;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using TitleBarHelper = IvsCalc.Classes.System.TitleBarHelper;

namespace IvsCalc.Classes.Controls;

/// <summary>
/// A subclass of <see cref="Window"/> that implements
/// lots of useful features missing in current version
/// of WinUI 3.0.
/// </summary>
public class BackdropWindow : Window {
    /// <summary>
    /// <see cref="BackdropWindow"/> backdrop types.
    /// </summary>
    public enum BackdropType {
        Mica,
        MicaAlt,
        DesktopAcrylic,
        DefaultColor,
    }
    
    BackdropType _currentBackdrop;
    Microsoft.UI.Composition.SystemBackdrops.MicaController? _micaController;
    Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController? _acrylicController;
    Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration? _configurationSource;

    /// <summary>
    /// <see cref="BackdropWindow"/> backdrop property.
    /// </summary>
    public BackdropType Backdrop {
        get => _currentBackdrop;
        set => SetBackdrop(value);
    }

    private IntPtr _hWnd;
    private AppWindow _appWindow;

    /// <summary>
    /// System's DPI scaling factor.
    /// </summary>
    public float ScalingFactor => PInvoke.User32.GetDpiForWindow(_hWnd) / 96f;

    /// <summary>
    /// <see cref="BackdropWindow"/> constructor.
    /// Ensures that DispatcherQueueController is present on
    /// the system and sets window handles variables.
    /// </summary>
    protected BackdropWindow() {
        WindowsSystemDispatcherQueueHelper dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();
        dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

        _hWnd = WindowNative.GetWindowHandle(this);
        _appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(_hWnd));
    }

    /// <summary>
    /// Changes current window icon.
    /// </summary>
    /// <param name="path">A path to <c>.ico</c> file with icon.</param>
    protected void SetIcon(string path) {
        _appWindow.SetIcon(path);
    }

    /// <summary>
    /// Fixes title bar colors.
    /// </summary>
    private void FixTitleBarColor() {
        TitleBarHelper.UpdateTitleBarColors(this, Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.White : Colors.Black);
    }

    /// <summary>
    /// Sets <see cref="UIElement"/> as window's title bar and fixes its colors.
    /// </summary>
    /// <param name="titlebar">A titlebar</param>
    protected new void SetTitleBar(UIElement? titlebar) {
        base.SetTitleBar(titlebar);
        FixTitleBarColor();
    }

    /// <summary>
    /// Uses Microsoft.UI.Composition API to change window's backdrop.
    /// </summary>
    /// <param name="type">Backdrop of <see cref="BackdropType"/></param>
    /// <param name="theme">Optional param to force backdrop's theme</param>
    /// <remarks>
    /// This function completely removes any previous controller to reset to the default
    /// state. This is done so this sample can show what is expected to be the most
    /// common pattern of an app simply choosing one controller type which it sets at
    /// startup. If an app wants to toggle between Mica and Acrylic it could simply
    /// call RemoveSystemBackdropTarget() on the old controller and then setup the new
    /// controller, reusing any existing _configurationSource and Activated/Closed
    /// </remarks>
    public void SetBackdrop(BackdropType type, ElementTheme theme = ElementTheme.Default) {
        // Reset to default color. If the requested type is supported, we'll update to that.
        _currentBackdrop = BackdropType.DefaultColor;

        if (_micaController != null) {
            _micaController.Dispose();
            _micaController = null;
        }
        if (_acrylicController != null) {
            _acrylicController.Dispose();
            _acrylicController = null;
        }
        Activated -= Window_Activated;
        Closed -= Window_Closed;
        ((FrameworkElement)Content).ActualThemeChanged -= Window_ThemeChanged;
        _configurationSource = null;

        // This is impossible to rewrite in a switch
        // because the damn C# does not support executing 
        // code and falling into next statement
        if (type == BackdropType.Mica) {
            if (TrySetMicaBackdrop(false, theme)) {
                _currentBackdrop = type;
            } else {
                // Mica isn't supported. Try Acrylic.
                type = BackdropType.DesktopAcrylic;
            }
        }
        if (type == BackdropType.MicaAlt) {
            if (TrySetMicaBackdrop(true, theme)) {
                _currentBackdrop = type;
            } else {
                // MicaAlt isn't supported. Try Acrylic.
                type = BackdropType.DesktopAcrylic;
            }
        }
        if (type == BackdropType.DesktopAcrylic) {
            if (TrySetAcrylicBackdrop(theme)) {
                _currentBackdrop = type;
            }
            // Acrylic isn't supported, so take the next option, which is DefaultColor, which is already set.
        }
    }
    
    /// <summary>
    /// Attempts to set Mica backdrop.
    /// </summary>
    /// <param name="useMicaAlt">Switches between <see cref="BackdropType.Mica"/> and <see cref="BackdropType.MicaAlt"/></param>
    /// <param name="theme">Optional param to force backdrop's theme</param>
    /// <returns>True if set successful, otherwise false</returns>
    bool TrySetMicaBackdrop(bool useMicaAlt, ElementTheme theme = ElementTheme.Default) {
        if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported()) {
            // Hooking up the policy object
            _configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
            Activated += Window_Activated;
            Closed += Window_Closed;
            ((FrameworkElement)Content).ActualThemeChanged += Window_ThemeChanged;

            // Initial configuration state.
            _configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme(theme);

            _micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

            if (useMicaAlt) {
                _micaController.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt;
            } else {
                _micaController.Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
            }

            // Enable the system backdrop.
            _micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            _micaController.SetSystemBackdropConfiguration(_configurationSource);
            return true; // Success
        }

        return false; // Mica is not supported on this system
    }

    /// <summary>
    /// Attempts to set Acrylic backdrop.
    /// </summary>
    /// <param name="theme">Optional param to force backdrop's theme</param>
    /// <returns>True if set successful, otherwise false</returns>
    bool TrySetAcrylicBackdrop(ElementTheme theme = ElementTheme.Default) {
        if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported()) {
            // Hooking up the policy object
            _configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
            Activated += Window_Activated;
            Closed += Window_Closed;
            ((FrameworkElement)Content).ActualThemeChanged += Window_ThemeChanged;

            // Initial configuration state.
            _configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme(theme);

            _acrylicController = new Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController();
            _acrylicController.LuminosityOpacity = 0.5f;  // TODO: Add ability to adjust Tint and Luminosity
            _acrylicController.TintOpacity = 0.5f;
            _acrylicController.TintColor = Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.Black : Colors.White;

            // Enable the system backdrop.
            _acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            _acrylicController.SetSystemBackdropConfiguration(_configurationSource);

            return true; // Success
        }

        return false; // Acrylic is not supported on this system
    }
    
    /// <summary>
    /// Event handler for the window's Activated event.
    /// </summary>
    /// <param name="sender">A Window</param>
    /// <param name="args">Default arguments</param>
    private void Window_Activated(object sender, WindowActivatedEventArgs args) {
        _configurationSource!.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    /// <summary>
    /// Event handler for the window's Closed event.
    /// </summary>
    /// <param name="sender">A Window</param>
    /// <param name="args">Default arguments</param>
    private void Window_Closed(object sender, WindowEventArgs args) {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use this closed window.
        if (_micaController != null) {
            _micaController.Dispose();
            _micaController = null;
        }
        if (_acrylicController != null) {
            _acrylicController.Dispose();
            _acrylicController = null;
        }
        Activated -= Window_Activated;
        ((FrameworkElement)Content).ActualThemeChanged -= Window_ThemeChanged;
        _configurationSource = null;
    }

    /// <summary>
    /// Event handler for the window's ThemeChanged event.
    /// </summary>
    /// <param name="sender">A Window</param>
    /// <param name="args">Default arguments</param>
    private void Window_ThemeChanged(FrameworkElement sender, object args) {
        if (_configurationSource != null) {
            SetConfigurationSourceTheme();
            
            // Changing system theme does nothing to Acrylic, we need to force it
            if (_currentBackdrop == BackdropType.DesktopAcrylic) {
                SetBackdrop(BackdropType.DesktopAcrylic);
            }
            
            FixTitleBarColor();
        }
    }

    /// <summary>
    /// Changes the theme of the configuration source accordingly.
    /// </summary>
    /// <param name="theme">Optional param to force backdrop's theme</param>
    private void SetConfigurationSourceTheme(ElementTheme theme = ElementTheme.Default) {
        _configurationSource!.Theme = theme switch {
            ElementTheme.Dark => Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark,
            ElementTheme.Light => Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light,
            ElementTheme.Default => Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default,
            _ => _configurationSource!.Theme
        };
    }
}

