using System.Collections.Generic;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Views;

public partial class StatusAndInformationView : ViewBase
{
    public StatusAndInformationView() : base("StatusAndInformation")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"ToolTip", ToolTipCard},
            {"InfoBadge", InfoBadgeCard},
            {"InfoBar", InfoBarCard},
            {"ProgressBar", ProgressBarCard},
            {"ProgressRing", ProgressRingCard}
        };
    }
}
