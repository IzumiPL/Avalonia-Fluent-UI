using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class SliderPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/SlierPage.axaml");

    public SliderPage() : base("Slider")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>() { { "Slider", SliderCard }, };
    }
}

