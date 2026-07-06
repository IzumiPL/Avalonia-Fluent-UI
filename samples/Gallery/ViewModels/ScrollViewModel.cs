using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public class ScrollViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Scroll");
}
