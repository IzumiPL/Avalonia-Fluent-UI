using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Views;

public partial class View : UserControl 
{
    public View()// : base("View")
    {
        InitializeComponent();

        // CodeCards = new Dictionary<string, CodeCard>()
        // {
            // {"ListBox", ListBoxCard},
            // {"TreeView", TreeViewCard},
            // {"FlipView", FlipViewCard}
        // };
    }
}
