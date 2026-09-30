using Avalonia.Media;
using AvaloniaFluentUI.Icons;
using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class SegmentedViewPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("SegmentedView");

    public Geometry[] IconSegmentedItems =>
    [
        FluentIcon.Home,
        FluentIcon.Application,
        FluentIcon.Message,
        FluentIcon.View,
        FluentIcon.Music,
        FluentIcon.GitHub,
        FluentIcon.Help,
        FluentIcon.Setting
    ];
}
