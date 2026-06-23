using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class SymbolIconPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("FontIcon");
}
