using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class CheckBoxPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/BasicPage/CheckBoxPage.axaml");

    public CheckBoxPage() : base("CheckBox")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"CheckBox", CheckBoxCard},
        };
    }
}
