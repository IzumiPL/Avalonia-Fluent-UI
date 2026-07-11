using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class InformationPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Information");
}
