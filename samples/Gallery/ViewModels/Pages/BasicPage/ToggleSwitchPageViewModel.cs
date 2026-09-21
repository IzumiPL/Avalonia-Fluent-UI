using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class ToggleSwitchPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("ToggleSwitch");

    [ObservableProperty]
    private bool _toggleSwitchButtonIsDisable;
}
