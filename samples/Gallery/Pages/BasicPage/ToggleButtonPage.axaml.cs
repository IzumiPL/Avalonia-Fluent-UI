using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ToggleButtonPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/ToggleButtonPage.axaml");

    public ToggleButtonPage() : base("ToggleButton")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"ToggleButton", ToggleButtonCard},
        };
    }
}
