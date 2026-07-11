using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class InfoBarPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("InfoBar");
    
    public InfoBarSeverity[] InfoBarSeverityItems => 
    [
        InfoBarSeverity.Informational,
        InfoBarSeverity.Success,
        InfoBarSeverity.Warning,
        InfoBarSeverity.Error
    ];
    
    [ObservableProperty]
    private InfoBarSeverity _currentInfoBarSeverity = InfoBarSeverity.Success;
    
    [ObservableProperty]
    private bool _infoBarIsClosable;

    [ObservableProperty]
    private bool _infoBarIsOpen = true;

    [RelayCommand]
    private void ResetInfoBar()
    {
        InfoBarIsOpen = true;
    }
    
     public InfoBarPosition[] InfoBarPositions => 
    [
        InfoBarPosition.Top,
        InfoBarPosition.TopLeft,
        InfoBarPosition.TopRight,
        InfoBarPosition.Bottom,
        InfoBarPosition.BottomLeft,
        InfoBarPosition.BottomRight
    ];
}
