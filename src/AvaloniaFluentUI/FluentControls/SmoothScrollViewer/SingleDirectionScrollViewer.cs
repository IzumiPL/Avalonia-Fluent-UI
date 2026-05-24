using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace AvaloniaFluentUI.Controls;

public class SingleDirectionScrollViewer : SmoothScrollViewer
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<SingleDirectionScrollViewer, Orientation>(nameof(Orientation));

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == OrientationProperty)
        {
            if (Orientation == Orientation.Vertical)
            {
                SetCurrentValue(HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                SetCurrentValue(VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
            }
            else
            {
                SetCurrentValue(VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                SetCurrentValue(HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
            }
        }
    }
}
