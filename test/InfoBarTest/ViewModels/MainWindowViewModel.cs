using AvaloniaFluentUI.Controls;

namespace InfoBarTest.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string[] Types => ["Information", "Success", "Warning", "Error"];
    public InfoBarPosition[] Positions => [InfoBarPosition.Top, InfoBarPosition.TopLeft, InfoBarPosition.TopRight, InfoBarPosition.Bottom, InfoBarPosition.BottomLeft, InfoBarPosition.BottomRight];
    public InfoBarSeverity[] Severities => [InfoBarSeverity.Informational, InfoBarSeverity.Success, InfoBarSeverity.Warning, InfoBarSeverity.Error, InfoBarSeverity.Custom];
}
