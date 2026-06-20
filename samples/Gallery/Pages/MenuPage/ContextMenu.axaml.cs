using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Gallery.Controls;
using Gallery.Extensions;

namespace Gallery.Pages;

public partial class ContextMenuPage : ViewBase
{
    public ContextMenuPage() : base("ContextMenu")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Menu", MenuCard},
        };
    }
}
