using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class SettingCardPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("SettingCard");
}
