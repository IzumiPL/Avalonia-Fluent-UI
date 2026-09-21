using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class OutlineButtonPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/OutlineButtonPage.axaml");

    public OutlineButtonPage() : base("OutlineButton")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"OutlineButton", StrokeButtonCard},
            {"OutlineToolButton", StrokeToolButtonCard},
        };
    }
}
