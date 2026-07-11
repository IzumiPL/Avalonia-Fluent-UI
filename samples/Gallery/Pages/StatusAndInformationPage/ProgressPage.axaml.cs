using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ProgressPage : ViewBase 
{
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
