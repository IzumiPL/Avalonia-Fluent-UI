using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class IconsViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Icon");
}
