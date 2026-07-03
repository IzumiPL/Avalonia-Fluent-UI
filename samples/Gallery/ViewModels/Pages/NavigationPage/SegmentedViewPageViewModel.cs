using System.ComponentModel;
using Avalonia.Media;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Icons;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using Gallery.Extensions;

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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IconSegmentedViewWidthFormat))]
    private string? _iconSegmentedViewWidth;

    public double IconSegmentedViewWidthFormat => IconSegmentedViewWidth.ToDoubleOrNan();

    [ObservableProperty]
    private object _segmentedToggleSelectedItem;
    
    [ObservableProperty]
    private string _segmentedSelectedItemFormat = "Null";

    partial void OnSegmentedToggleSelectedItemChanged(object value)
    {
        if (value is SegmentedItem item)
        {
            SegmentedSelectedItemFormat = LocalizationService.Instance.GetString("CurrentSelectedPage") + ": " + item.Content;
        }
    }

    [ObservableProperty]
    private object _segmentedSelectedItem;

    [ObservableProperty]
    private string _selectedItemFormat = "Null";

    partial void OnSegmentedSelectedItemChanged(object value)
    {
        if (value is SegmentedItem item)
        {
            SelectedItemFormat =  LocalizationService.Instance.GetString("CurrentSelectedPage") + ": " + item.Content;
        }
    } 
}
