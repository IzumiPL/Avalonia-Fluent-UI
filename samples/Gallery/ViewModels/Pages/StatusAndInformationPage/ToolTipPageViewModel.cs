using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public class ToolTipPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("ToolTip");
}
