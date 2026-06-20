using System.ComponentModel;
using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class FrameViewPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Frame");
}
