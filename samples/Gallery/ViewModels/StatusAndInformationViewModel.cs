using System.Collections.Generic;
using AvaloniaFluentUI.Locale;
using Gallery.Models;

namespace Gallery.ViewModels;

public partial class StatusAndInformationViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("StatusAndInformation");
    
    public List<ButtonItemModel> StatusAndInformationItemSource { get; }

    public StatusAndInformationViewModel()
    { 
        StatusAndInformationItemSource = ButtonItemModel.CreateList(
            ("ToolTip", "ToolTip", "ToolTip",  "A control tooltip, hover show tooltip"),
            ("InfoBadge", "InfoBadge", "Information", "Information badges can display a variety of information"),
            ("InfoBar", "InfoBar", "InfoBar", "Information bar can display a variety of information and can be closed"),
            ("ProgressBar", "ProgressBar", "ProgressBar", "The progress bar has two states: confirmed and uncertain."),
            ("ProgressRing", "ProgressRing", "ProgressBar", "A progress ring")
        );
    }
}
