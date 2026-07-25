using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class BorderPage : ViewBase
{ 
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/LayoutPage/BorderPage.axaml"); 
    
    public BorderPage() : base("Border")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Border", BorderCard},
            {"Canvas", CanvasCard},
        };
    }
}
