using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class FilesDropPickerPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/ViewPage/FilesDropPickerPage.axaml"); 
    
    public FilesDropPickerPage()
    {
        InitializeComponent();
    }
}
