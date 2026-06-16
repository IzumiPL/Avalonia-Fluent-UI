using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaFluentUI.Media.Animation;

namespace AvaloniaFluentUI.Controls;

public class FluentFlyout : Flyout
{
    protected override void OnClosing(CancelEventArgs args)
    {
        base.OnClosing(args);
    }

    protected override void OnOpened()
    {
        if (Popup.Child is { } presenter)
        {
            var (offset, property) = Placement switch
            {
                PlacementMode.Center
                    => (0.75, null),

                PlacementMode.Left
                    or PlacementMode.LeftEdgeAlignedBottom
                    or PlacementMode.LeftEdgeAlignedTop
                    or PlacementMode.Right
                    or PlacementMode.RightEdgeAlignedBottom
                    or PlacementMode.RightEdgeAlignedTop
                    => (
                        Placement is PlacementMode.Left
                            or PlacementMode.LeftEdgeAlignedBottom
                            or PlacementMode.LeftEdgeAlignedTop
                            ? 32d
                            : -32d,
                        TranslateTransform.XProperty
                    ),

                _ => (
                    Placement is PlacementMode.Top
                        or PlacementMode.TopEdgeAlignedLeft
                        ? 32d
                        : -32d,
                    TranslateTransform.YProperty
                )
            };

            if (property is null)
            {
                FluentAnimation.CenterScaleAsync(presenter, offset);
            }
            else
            {
                FluentAnimation.SlideInAsync(presenter, offset, property);
            }
        }

        base.OnOpened();
    }
}
