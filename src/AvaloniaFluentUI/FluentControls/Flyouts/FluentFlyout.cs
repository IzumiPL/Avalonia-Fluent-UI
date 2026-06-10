using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaFluentUI.Media.Animation;

namespace AvaloniaFluentUI.Controls;

public class FluentFlyout : Flyout
{
    protected override void OnOpened()
    {
        if (Popup.Child is { } presenter)
        {
            var value = Placement switch
            {
                PlacementMode.Center or PlacementMode.Pointer => 0.75d,
                PlacementMode.Left or PlacementMode.LeftEdgeAlignedBottom or PlacementMode.LeftEdgeAlignedTop
                    or PlacementMode.Top or PlacementMode.TopEdgeAlignedLeft => 32d,
                _ => -32d
            };

            if (value < 1 && value > 0)
            {
                FluentAnimation.CenterScaleAsync(presenter, value);
            }
            else
            {
                StyledProperty<double> property;
                switch (Placement)
                {
                    case PlacementMode.Left or PlacementMode.LeftEdgeAlignedTop or PlacementMode.LeftEdgeAlignedBottom
                        or PlacementMode.Right or PlacementMode.RightEdgeAlignedBottom
                        or PlacementMode.RightEdgeAlignedTop:
                        property = TranslateTransform.XProperty;
                        break;
                    default:
                        property = TranslateTransform.YProperty;
                        break;
                }

                FluentAnimation.SlideInAsync(presenter, value, property);
            }
        }

        base.OnOpened();
    }
}
