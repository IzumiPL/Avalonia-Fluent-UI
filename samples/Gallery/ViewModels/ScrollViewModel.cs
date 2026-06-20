using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public class ScrollViewModel : ViewModelBase
{
    public string SettingsTitle => LocalizationService.Instance.GetString("Scroll");
}
