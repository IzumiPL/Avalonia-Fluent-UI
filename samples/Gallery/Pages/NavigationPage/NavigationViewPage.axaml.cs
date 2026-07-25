using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class NavigationViewPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/NavigationPage/NavigationViewPage.axaml"); 
    
    public NavigationViewPage() : base("NavigationView")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"NavigationView", NavigationViewCard},
        };
    }
}
