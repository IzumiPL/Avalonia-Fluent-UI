using Avalonia;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class GradientCard : SimpleCard 
{
    public static readonly StyledProperty<RelativePoint> StartPointProperty =
        LinearGradientBrush.StartPointProperty.AddOwner<GradientCard>();

    public static readonly StyledProperty<RelativePoint> EndPointProperty =
        LinearGradientBrush.EndPointProperty.AddOwner<GradientCard>();

    public static readonly StyledProperty<GradientStops> GradientStopsProperty =
        GradientBrush.GradientStopsProperty.AddOwner<GradientCard>();
    
    public GradientStops GradientStops
    {
        get => GetValue(GradientStopsProperty);
        set => SetValue(GradientStopsProperty, value);
    }

    public RelativePoint EndPoint
    {
        get => GetValue(EndPointProperty);
        set => SetValue(EndPointProperty, value);
    }
    
    public RelativePoint StartPoint
    {
        get => GetValue(StartPointProperty);
        set => SetValue(StartPointProperty, value);
    }

    public GradientCard()
    {
        GradientStops = [];
    }
}

