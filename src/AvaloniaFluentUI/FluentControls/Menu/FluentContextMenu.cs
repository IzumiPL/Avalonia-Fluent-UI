using System;
using AvaloniaFluentUI.Media.Animation;

namespace AvaloniaFluentUI.Controls;

public class FluentContextMenu : Avalonia.Controls.ContextMenu
{
    public FluentContextMenu()
    {
        Opened += OnMenuOpened;
    }

    private void OnMenuOpened(object sender, EventArgs e) => FluentAnimation.ScaleAndSliderInAsync(this, -Bounds.Height);
}
