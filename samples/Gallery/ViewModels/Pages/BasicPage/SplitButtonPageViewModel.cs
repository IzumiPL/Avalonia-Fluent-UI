using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class SplitButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("SplitButton");

    [ObservableProperty]
    private bool _splitButtonIsDisable;
}
