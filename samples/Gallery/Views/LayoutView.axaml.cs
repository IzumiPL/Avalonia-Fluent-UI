using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Controls;
using Gallery.Extensions;
using Gallery.Messages;

namespace Gallery.Views;

public partial class LayoutView : UserControl 
{
    public LayoutView() // : base("Layout")
    {
        InitializeComponent();

        // CodeCards = new Dictionary<string, CodeCard>()
        // {
            // {"Border", BorderCard},
            // {"Canvas", CanvasCard},
            // {"SplitView", SplitViewCard},
            // {"Grid", GridCard},
            // {"RelativePanel", RelativePanelCard},
            // {"StackPanel", StackPanelCard},
            // {"Expander", ExpanderCard}
        // };
    }
}
