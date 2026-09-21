using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class CheckBoxPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("CheckBox");

    [ObservableProperty]
    private bool _checkBoxIsDisable;

    [ObservableProperty]
    private bool _checkBoxIsThreeState;
}
