using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class HyperlinkButtonPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/HyperlinkButtonPage.axaml");

    public HyperlinkButtonPage() : base("HyperlinkButton")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"HyperlinkButton", HyperlinkButtonCard},
        };
    }
}
