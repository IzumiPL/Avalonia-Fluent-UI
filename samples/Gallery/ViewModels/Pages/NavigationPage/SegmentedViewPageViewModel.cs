using System.ComponentModel;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class SegmentedViewPageViewModel : ViewModelBase
{ 
    public override string Title => LocalizationService.Instance.GetString("SegmentedView");
    
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
