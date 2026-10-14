using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class TitleBarPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("TitleBar");
}
