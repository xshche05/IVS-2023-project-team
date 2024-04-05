using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace IvsCalc.Classes.Controls; 

/// <summary>
/// Class that provides attached property to add keyboard shortcuts to controls
/// </summary>
public class Keyboard : DependencyObject {
    /// <summary>
    /// Shortcut attached property
    /// </summary>
    public static readonly DependencyProperty ShortcutProperty = DependencyProperty.RegisterAttached("Shortcut", typeof(string), typeof(Keyboard), new PropertyMetadata(null, ShortcutPropertyChanged));

    /// <summary>
    /// Setter for <see cref="ShortcutProperty"/>
    /// </summary>
    /// <param name="element">A <see cref="DependencyObject"/> that property is attached to</param>
    /// <param name="value">Property's new value</param>
    public static void SetShortcut(DependencyObject element, string value) => element.SetValue(ShortcutProperty, value);
    
    /// <summary>
    /// Getter for <see cref="ShortcutProperty"/>
    /// </summary>
    /// <param name="element">A <see cref="DependencyObject"/> that property is attached to</param>
    /// <returns>Property's value</returns>
    public static string GetShortcut(DependencyObject element) => (string)element.GetValue(ShortcutProperty);

    /// <summary>
    /// Converts <paramref name="shortcut"/> to an instance of <see cref="KeyboardAccelerator"/> and adds it to <paramref name="element"/>
    /// </summary>
    /// <param name="element">Target element</param>
    /// <param name="shortcut">Shortcut or list of shortcuts (separated by comma and space)</param>
    private static void AddShortcut(UIElement element, string shortcut) {
        VirtualKey key = VirtualKey.None;
        VirtualKeyModifiers modifiers = VirtualKeyModifiers.None;
        
        if (shortcut.Contains("+")) {
            IEnumerable<string> keys = shortcut.Split('+');

            // Pressed key
            Enum.TryParse(keys.Last(), out key);

            // Remove the last key
            keys = keys.Take(keys.Count() - 1);

            // Modifier keys
            foreach (string modifier in keys) {
                Enum.TryParse(modifier, out VirtualKeyModifiers modifierKey);
                modifiers |= modifierKey;
            }
        } else {
            Enum.TryParse(shortcut, out key);
        }


        KeyboardAccelerator accelerator = new() {
            Modifiers = modifiers,
            Key = key
        };
        
        element.KeyboardAccelerators.Add(accelerator);
    }
    
    /// <summary>
    /// Event handler for <see cref="ShortcutProperty"/>'s changes
    /// </summary>
    /// <param name="obj">A <see cref="DependencyObject"/> that property is attached to</param>
    /// <param name="e">Default arguments</param>
    private static void ShortcutPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
        if (obj is not UIElement element) return;

        element.KeyboardAccelerators.Clear();
        
        string shortcut = (string)e.NewValue;
        if (string.IsNullOrEmpty(shortcut)) return;

        char sep = '\0';

        if (shortcut.Contains(' '))
            sep = ' ';
        
        if (shortcut.Contains(','))
            sep = ',';

        if (sep != '\0') {
            foreach(string s in shortcut.Split(sep)) {
                AddShortcut(element, s);
            }
        } else {
            AddShortcut(element, shortcut);
        }
    }
}