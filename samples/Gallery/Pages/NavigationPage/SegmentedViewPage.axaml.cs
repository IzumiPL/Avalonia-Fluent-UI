using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class SegmentedViewPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/NavigationPage/SegmentedViewPage.axaml"); 
    
    public SegmentedViewPage()  : base("SegmentedView")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Segmented", SegmentedCard}
        };
    }
}
