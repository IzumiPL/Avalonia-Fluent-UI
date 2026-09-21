using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class HyperlinkButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("HyperlinkButton");

    [ObservableProperty]
    private bool _hyperlinkButtonIsDisable;
}
