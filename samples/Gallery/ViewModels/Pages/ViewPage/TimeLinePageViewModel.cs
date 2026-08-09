using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class TimeLinePageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("TimeLine");
}
