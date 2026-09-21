using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class RadioButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("RadioButton");

    [ObservableProperty]
    private bool _chipsIconRadioButtonIsEnabled;

    [ObservableProperty]
    private bool _radioButtonIsDisabled;

    [ObservableProperty]
    private bool _chipsRadioButtonIsEnabled;

    [ObservableProperty]
    private bool _subTitleRadioButtonIsDisabled;
}
