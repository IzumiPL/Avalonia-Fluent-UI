using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class BreadcrumbBarPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/NavigationPage/BreadcrumbBarPage.axaml"); 
    
    public BreadcrumbBarPage() : base("BreadcrumbBar")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"BreadcrumbBar", BreadcrumbBarCard},
        };
    }
}
