using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaFluentUI.Media.Animation;

namespace AvaloniaFluentUI.Controls;

public class FluentPopup : Avalonia.Controls.Primitives.Popup
{
    public static readonly StyledProperty<double> OffSetProperty =
        AvaloniaProperty.Register<FluentPopup, double>(nameof(OffSet), defaultValue: -16);

    public double OffSet
    {
        get => GetValue(OffSetProperty);
        set => SetValue(OffSetProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenProperty)
        {
            var isOpen = change.GetNewValue<bool>();
            if (isOpen)
            {
                RunOpenAnimationAsync();
            }
        }
    }
    
    protected virtual void RunOpenAnimationAsync()
    {
        var child = Child as Visual;
        if (child == null) return;

        var property = Placement switch
        { 
            PlacementMode.Left or 
                PlacementMode.Right or 
                PlacementMode.LeftEdgeAlignedBottom or 
                PlacementMode.LeftEdgeAlignedTop or 
                PlacementMode.RightEdgeAlignedBottom or 
                PlacementMode.RightEdgeAlignedTop 
                => TranslateTransform.XProperty,
            PlacementMode.Center => null,
            _ => TranslateTransform.YProperty
        };
        if (property != null)
        {
            FluentAnimation.SlideInAsync(child, OffSet, property);
        }
        else
        {
            FluentAnimation.CenterScaleAsync(child, OffSet);
        }
    }
}

public class ScaleFluentPopup : FluentPopup 
{
    public ScaleFluentPopup()
    {
        OffSet = 0.75;
    }
    
    protected override void RunOpenAnimationAsync()
    {
        var child = Child as Visual;
        if (child == null) return;

        FluentAnimation.CenterScaleAsync(child, OffSet);
    }
}
