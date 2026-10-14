using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Windowing;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class TitleBarPage : ViewBase 
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/ViewPage/TitleBarPage.axaml"); 
    
    public TitleBarPage() : base("TitleBar")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            { "FluentTitleBar", FluentTitleBarCard},
        };
    }
}
