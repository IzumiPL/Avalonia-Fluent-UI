using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;


[PseudoClasses(PC_PRESSED, PC_DESCRIPTION)]
public class SettingCard : HeaderedContentControl
{
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<SettingCard, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<SettingCard, object?>(nameof(IconSource));

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<SettingCard, double>(nameof(IconSize), 24);

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public object? IconSource 
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }
    
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    
    private const string PC_PRESSED = ":pressed";
    private const string PC_DESCRIPTION = ":description";

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DescriptionProperty)
        {
            OnDescriptionChanged(change);
        }
    }

    private void OnDescriptionChanged(AvaloniaPropertyChangedEventArgs args)
    {
        PseudoClasses.Set(PC_PRESSED,  args.NewValue != null);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        PseudoClasses.Add(PC_PRESSED);
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        PseudoClasses.Remove(PC_PRESSED);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        PseudoClasses.Remove(PC_PRESSED);
    }
}
