using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using AvaloniaFluentUI.Controls.Enums;

namespace AvaloniaFluentUI.Controls;

public class SingleDirectionScrollViewer : SmoothScrollViewer
{
    public static readonly StyledProperty<SmoothScrollOrientation> OrientationProperty =
        AvaloniaProperty.Register<SingleDirectionScrollViewer, SmoothScrollOrientation>(nameof(Orientation));

    public SmoothScrollOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == OrientationProperty)
        {
            if (Orientation == SmoothScrollOrientation.Vertical)
            {
                SetCurrentValue(HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                SetCurrentValue(VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
            }
            else
            {
                SetCurrentValue(VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                SetCurrentValue(HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
            }

            Console.WriteLine("Orientation Changed");
            Console.WriteLine(Orientation);
        }
    }
}
