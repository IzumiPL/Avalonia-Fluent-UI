using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class FilledButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("FilledButton");

    [ObservableProperty]
    private bool _filledPushButtonIsDisabled;

    [ObservableProperty]
    private bool _filledToolButtonIsDisabled;
}
