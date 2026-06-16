using System;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using AvaloniaFluentUI.Media.Animation;

namespace AvaloniaFluentUI.Controls;

public class FluentContextMenu : Avalonia.Controls.ContextMenu
{
    public FluentContextMenu()
    {
        Opened += OnMenuOpened;
    }

    private void OnMenuOpened(object sender, EventArgs e) => FluentAnimation.SlideInAsync(this, -Bounds.Height, TranslateTransform.YProperty, 300.0, new SplineEasing(0.1, 0.9, 0.5, 1.0));
}
