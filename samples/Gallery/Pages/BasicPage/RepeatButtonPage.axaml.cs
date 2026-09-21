using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class RepeatButtonPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/RepeatButtonPage.axaml");

    public RepeatButtonPage() : base("RepeatButton")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"RepeatButton", RepeatButtonCard},
        };
    }
}
