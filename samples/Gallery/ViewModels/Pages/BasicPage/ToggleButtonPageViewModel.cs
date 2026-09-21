using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class ToggleButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("ToggleButton");

    [ObservableProperty]
    private bool _toggleButtonIsDisabled;

    [ObservableProperty]
    private bool _transparentToggleButtonIsDisabled;

    [ObservableProperty]
    private bool _toggleSplitButtonIsDisabled;
}
