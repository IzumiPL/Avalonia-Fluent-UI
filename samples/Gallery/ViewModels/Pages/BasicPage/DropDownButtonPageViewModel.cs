using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class DropDownButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("DropDownButton");

    [ObservableProperty]
    private bool _dropDownButtonIsDisable;

    [ObservableProperty]
    private bool _transparentDropDownButtonIsDisable;
}
