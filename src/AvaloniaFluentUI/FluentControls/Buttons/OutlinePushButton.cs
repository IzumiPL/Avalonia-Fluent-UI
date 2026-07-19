using Avalonia;

namespace AvaloniaFluentUI.Controls;

public class OutlinePushButton : OutlineButtonBase 
{
    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<OutlineButtonBase, object?>(nameof(IconSource));
    
    public object? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }
}
