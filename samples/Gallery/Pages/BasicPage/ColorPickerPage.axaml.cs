using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ColorPickerPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/ColorPickerPage.axaml");

    public ColorPickerPage() : base("ColorPicker")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"ColorView", ColorViewCard},
        };
    }
}
