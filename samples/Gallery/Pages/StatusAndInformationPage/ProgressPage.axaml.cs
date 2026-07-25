using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ProgressPage : ViewBase 
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/StatusAndInformationPage/ProgressPage.axaml"); 
    
    public ProgressPage() : base("ProgressBar")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"ProgressBar", ProgressBarCard},
            {"ProgressRing", ProgressRingCard}
        };
    }
}
