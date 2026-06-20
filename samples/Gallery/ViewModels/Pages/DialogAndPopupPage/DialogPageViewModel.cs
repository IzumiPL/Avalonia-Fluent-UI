using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class DialogPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Dialog");
}
