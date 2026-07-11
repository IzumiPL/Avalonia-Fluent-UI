using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class Tag : ContentControl
{
    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<Tag, object?>(nameof(IconSource));

    public object? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsEnabled && change.Property == ForegroundProperty)
        {
            var foreground = change.GetNewValue<IBrush>();
            if (foreground is ISolidColorBrush brush)
            {
                var color = brush.Color;
                var nb = Color.FromArgb(50, color.R, color.G, color.B);
                Background =  new SolidColorBrush(nb);

                var nf = Color.FromArgb(150, color.R, color.G, color.B);
                BorderBrush = new SolidColorBrush(nf);
            }
        }
    }
}
