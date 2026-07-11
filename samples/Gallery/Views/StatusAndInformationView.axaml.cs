using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Views;

public partial class StatusAndInformationView : UserControl 
{
    public StatusAndInformationView()// : base("StatusAndInformation")
    {
        InitializeComponent();

        // CodeCards = new Dictionary<string, CodeCard>
        // {
            // { "ToolTip", ToolTipCard },
            // { "InfoBadge", InfoBadgeCard },
            // { "InfoBar", InfoBarCard },
            // { "ProgressBar", ProgressBarCard },
            // { "ProgressRing", ProgressRingCard }
        // };
    }
}
