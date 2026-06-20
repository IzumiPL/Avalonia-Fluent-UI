using System.ComponentModel;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class NavigationViewPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("NavigationView");

    [ObservableProperty]
    private NavigationViewPaneDisplayMode _currentPaneDisplayMode = NavigationViewPaneDisplayMode.Auto;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentPage))]
    private NavigationViewItem? _currentItem;

    public string CurrentPage => CurrentItem?.Content?.ToString() ?? "Null";

    public NavigationViewDisplayMode[] DisplayModes => 
    [
        NavigationViewDisplayMode.Compact, 
        NavigationViewDisplayMode.Expanded, 
        NavigationViewDisplayMode.Minimal
    ];

    [ObservableProperty]
    private NavigationViewDisplayMode _currentDisplayMode = NavigationViewDisplayMode.Expanded;

    public NavigationViewPaneDisplayMode[] PaneDisplayModes => 
    [ 
        NavigationViewPaneDisplayMode.Auto, 
        NavigationViewPaneDisplayMode.Left, 
        NavigationViewPaneDisplayMode.Top, 
        NavigationViewPaneDisplayMode.LeftCompact, 
        NavigationViewPaneDisplayMode.LeftMinimal
    ];
    
    [ObservableProperty]
    private bool _backButtonIsEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NavigationCurrentItemFormat))]
    private NavigationViewItem? _navigationCurrentItem;

    public string NavigationCurrentItemFormat => NavigationCurrentItem?.ToString() ?? "Null";
}
