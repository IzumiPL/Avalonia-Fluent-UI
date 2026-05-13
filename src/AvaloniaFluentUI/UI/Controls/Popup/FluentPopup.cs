using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using AvaloniaFluentUI.Animations;

namespace AvaloniaFluentUI.UI.Controls;

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
                _ = PlayOpenAnimationAsync();
            }
        }
    }
    
    protected virtual async Task PlayOpenAnimationAsync()
    {
        var child = Child as Visual;
        if (child == null) return;

        await FluentAnimation.SlideInAsync(child, OffSet, TranslateTransform.YProperty);
    }
}

public class ScaleFluentPopup : FluentPopup 
{
    public ScaleFluentPopup()
    {
        OffSet = 0.75;
    }
    
    protected override async Task PlayOpenAnimationAsync()
    {
        var child = Child as Visual;
        if (child == null) return;

        await FluentAnimation.CenterScaleAsync(child, OffSet);
    }
}
