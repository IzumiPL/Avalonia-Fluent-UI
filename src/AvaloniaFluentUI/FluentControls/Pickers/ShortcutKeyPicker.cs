using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using AvaloniaFluentUI.Locale;

namespace AvaloniaFluentUI.Controls;

public class ShortcutKeyPicker : PickerButton 
{
    public static readonly StyledProperty<KeyGesture?> KeyGestureProperty =
        AvaloniaProperty.Register<ShortcutKeyPicker, KeyGesture?>(nameof(KeyGesture));

    public static readonly StyledProperty<IEnumerable<string>> KeysProperty =
        AvaloniaProperty.Register<ShortcutKeyPicker, IEnumerable<string>>(nameof(Keys));

    public KeyGesture? DefaultKeyGesture { get; set; }

    public IEnumerable<string> Keys
    {
        get => GetValue(KeysProperty);
        private set => SetValue(KeysProperty, value);
    }
    
    public KeyGesture? KeyGesture
    {
        get => GetValue(KeyGestureProperty);
        set => SetValue(KeyGestureProperty, value);
    }

    private ShortcutKeyPanel _shortcutKeyPanel;
    private KeyGesture? _inputKeyGesture;

    public ShortcutKeyPicker()
    {
        Focusable = true;
        _shortcutKeyPanel = new ShortcutKeyPanel{ Height = 50 };
    }

    protected async override void OnClick()
    {
        base.OnClick();
        
        var dialog = new ContentDialog
        {
            Title = LocalizationService.Instance.GetString("ActivateShortcutKeys"),
            PrimaryButtonText = LocalizationService.Instance.GetString("Save"),
            DefaultButton = ContentDialogButton.Primary,
            CloseButtonText = LocalizationService.Instance.GetString("Cancel"),
            SecondaryButtonText = LocalizationService.Instance.GetString("Reset"),
            Content = _shortcutKeyPanel,
            ContentWidth = 400,
            ContentHeight = 360,
        };

        dialog.KeyDown += OnShortcutKeyDown;
        dialog.SecondaryButtonClick += OnResetShortcutKey;
        dialog.PrimaryButtonClick += OnAcceptShortcutKey;
        
        _shortcutKeyPanel.Keys = Keys;
        await dialog.ShowAsync();

        if (_shortcutKeyPanel.Parent is ContentControl cc)
        {
            cc.Content = null;
        }
        
        dialog.KeyDown -= OnShortcutKeyDown;
        dialog.SecondaryButtonClick -= OnResetShortcutKey;
        dialog.PrimaryButtonClick -= OnAcceptShortcutKey;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == KeyGestureProperty)
        {
            Keys = KeyGesture != null ? FormatGesture(KeyGesture) : Array.Empty<string>();
        }
    }

    private void OnResetShortcutKey(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (DefaultKeyGesture != null)
        {
            Keys = FormatGesture(DefaultKeyGesture);
            _inputKeyGesture = null;
            KeyGesture = DefaultKeyGesture;
        }
    }

    private void OnAcceptShortcutKey(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (_inputKeyGesture == null) return;
        Keys = FormatGesture(_inputKeyGesture);
    }

    private void OnShortcutKeyDown(object? sender, KeyEventArgs e)
    {
        if (IsModifierKey(e.Key)) return;
        
        if (e.KeyModifiers == KeyModifiers.None && !IsStandaloneShortcut(e.Key)) return;

        _inputKeyGesture = new KeyGesture(e.Key, e.KeyModifiers); 
        _shortcutKeyPanel.Keys = FormatGesture(_inputKeyGesture);
        
        e.Handled = true;
    }

    private static bool IsModifierKey(Key key)
    {
        return key is
            Key.LeftCtrl or
            Key.RightCtrl or
            Key.LeftAlt or
            Key.RightAlt or
            Key.LeftShift or
            Key.RightShift or
            Key.LWin or
            Key.RWin;
    }

    private static bool IsStandaloneShortcut(Key key)
    {
        return key switch
        {
            Key.F1 => true,
            Key.F2 => true,
            Key.F3 => true,
            Key.F4 => true,
            Key.F5 => true,
            Key.F6 => true,
            Key.F7 => true,
            Key.F8 => true,
            Key.F9 => true,
            Key.F10 => true,
            Key.F11 => true,
            Key.F12 => true,
            _ => false
        };
    }
    
    private static IEnumerable<string> FormatGesture(KeyGesture gesture)
    {
        List<string> parts = [];

        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Control)) 
            parts.Add("Ctrl");

        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Shift))
            parts.Add("Shift");

        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Alt))
            parts.Add("Alt");

        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Meta))
            parts.Add("Win");

        parts.Add(gesture.Key.ToString());

        return parts;
    }
}
