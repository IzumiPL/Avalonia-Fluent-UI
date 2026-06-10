using Avalonia;
using Avalonia.Input;

namespace Gallery.Controls;

public class CheckedBorder : Avalonia.Controls.Border
{
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<CheckedBorder, bool>(nameof(IsChecked));

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        
        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            IsChecked = !IsChecked;
        }
    }
}
