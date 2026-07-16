using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public class WizardPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("WizardView");
}
