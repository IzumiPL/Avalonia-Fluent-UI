using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class SplitButtonPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/SplitButtonPage.axaml");

    public SplitButtonPage() : base("SplitButton")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"SplitButton", SplitButtonCard},
        };
    }
}
