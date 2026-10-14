using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class FluentWindowPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("FluentWindow");
}
