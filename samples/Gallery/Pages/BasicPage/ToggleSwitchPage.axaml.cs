using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ToggleSwitchPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/ToggleSwitchPage.axaml");

    public ToggleSwitchPage() : base("ToggleSwitch")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"SwitchButton", ToggleSwitchButtonCard},
        };
    }
}
