using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class SettingCardPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/ViewPage/SettingCardPage.axaml"); 
    
    public SettingCardPage()
    {
        InitializeComponent();
    }
}
