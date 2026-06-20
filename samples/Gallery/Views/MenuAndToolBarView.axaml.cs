using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaFluentUI.Controls;
using Gallery.Controls;

namespace Gallery.Views;

public partial class MenuAndToolBarView : UserControl 
{
    public MenuAndToolBarView()// : base("MenuAndToolBar")
    {
        InitializeComponent();
        
        // CodeCards = new Dictionary<string, CodeCard>()
        // {
            // {"Menu", MenuCard},
            // {"MenuBar", MenuBarCard},
            // {"CommandBar", CommandBarCard},
            // {"CommandBarFlyout", CommandBarFlyoutCard}
        // };
    }
}
