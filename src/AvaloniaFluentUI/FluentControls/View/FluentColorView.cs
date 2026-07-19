using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Standalone colour picker control. Sub-controls (ColorSpectrum, ColorSliders,
/// NumericUpDowns) all bind to the single <see cref="HsvColor"/> property so they
/// stay in perfect sync without manual event handlers.
/// </summary>
[TemplatePart(Name = PART_HEX_INPUT, Type = typeof(TextBox))]
public class FluentColorView : TemplatedControl
{
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<FluentColorView, Color>(nameof(Color));

    public static readonly StyledProperty<HsvColor> HsvColorProperty =
        AvaloniaProperty.Register<FluentColorView, HsvColor>(nameof(HsvColor));

    public static readonly StyledProperty<IBrush> OriginalColorProperty =
        AvaloniaProperty.Register<FluentColorView, IBrush>(nameof(OriginalColor));

    public static readonly StyledProperty<IBrush> NewColorProperty =
        AvaloniaProperty.Register<FluentColorView, IBrush>(nameof(NewColor));

    public static readonly StyledProperty<string> HexTextProperty =
        AvaloniaProperty.Register<FluentColorView, string>(nameof(HexText), "FF00BFFF");

    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public HsvColor HsvColor
    {
        get => GetValue(HsvColorProperty);
        set => SetValue(HsvColorProperty, value);
    }

    public IBrush OriginalColor
    {
        get => GetValue(OriginalColorProperty);
        set => SetValue(OriginalColorProperty, value);
    }

    public IBrush NewColor
    {
        get => GetValue(NewColorProperty);
        set => SetValue(NewColorProperty, value);
    }

    public string HexText
    {
        get => GetValue(HexTextProperty);
        set => SetValue(HexTextProperty, value);
    }

    public event EventHandler<Color>? ColorChanged;

    private TextBox? _hexInput;
    private bool _suppressUpdate;

    private const string PART_HEX_INPUT = "PART_HexInput";

    static FluentColorView()
    {
        ColorProperty.Changed.AddClassHandler<FluentColorView>((s, e) =>
        {
            if (s._suppressUpdate) return;
            if (e.NewValue is Color c)
            {
                s._suppressUpdate = true;
                s.SetCurrentValue(HsvColorProperty, c.ToHsv());
                s.SyncDisplay(c);
                s._suppressUpdate = false;
            }
        });

        HsvColorProperty.Changed.AddClassHandler<FluentColorView>((s, e) =>
        {
            if (s._suppressUpdate) return;
            if (e.NewValue is HsvColor hsv)
            {
                s._suppressUpdate = true;
                var c = hsv.ToRgb();
                s.SetCurrentValue(ColorProperty, c);
                s.SyncDisplay(c);
                s._suppressUpdate = false;
            }
        });
    }

    private void SyncDisplay(Color c)
    {
        HexText = $"{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
        NewColor = new SolidColorBrush(c);
        ColorChanged?.Invoke(this, c);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_hexInput != null)
        {
            _hexInput.KeyDown -= OnHexBoxKeyDown;
            _hexInput.LostFocus -= OnHexBoxLostFocus;
        }

        base.OnApplyTemplate(e);

        _hexInput = e.NameScope.Find<TextBox>(PART_HEX_INPUT);

        if (_hexInput != null)
        {
            _hexInput.KeyDown += OnHexBoxKeyDown;
            _hexInput.LostFocus += OnHexBoxLostFocus;
        }

        OriginalColor = new SolidColorBrush(Color);
        NewColor = new SolidColorBrush(Color);
    }

    private void OnHexBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            UpdateHexText();
            e.Handled = true;
        }
    }

    private void OnHexBoxLostFocus(object? sender, RoutedEventArgs e) => UpdateHexText();

    private void UpdateHexText()
    {
        if (_hexInput == null || _suppressUpdate) return;
        var text = _hexInput.Text?.Trim();
        if (string.IsNullOrEmpty(text)) return;
        if (text.Length == 6) text = "FF" + text;
        if (text.Length != 8 || !uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var argb))
            return;

        Color = Color.FromArgb(
            (byte)((argb >> 24) & 0xFF),
            (byte)((argb >> 16) & 0xFF),
            (byte)((argb >> 8) & 0xFF),
            (byte)(argb & 0xFF));
    }
}
