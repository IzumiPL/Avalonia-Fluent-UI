using System;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class TabsPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/NavigationPage/TabsPage.axaml"); 
    
    public TabsPage() : base("Tabs")
    {
        InitializeComponent();
    }
}
