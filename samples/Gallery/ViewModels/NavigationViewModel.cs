using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaFluentUI.UI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class NavigationViewModel : ViewModelBase
{
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
    private NavigationViewPaneDisplayMode _currentPaneDisplayMode = NavigationViewPaneDisplayMode.Auto;

    [ObservableProperty]
    private bool _backButtonIsEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NavigationCurrentItemFormat))]
    private NavigationViewItem? _navigationCurrentItem;

    public string NavigationCurrentItemFormat => NavigationCurrentItem?.ToString() ?? "Null";

    [ObservableProperty]
    private ObservableCollection<TabViewItem> _tabItemSource = new ObservableCollection<TabViewItem>();

    private int tabCount = 3;

    [ObservableProperty]
    private object _tabViewCurrentItem;

    public NavigationViewModel()
    {
        // for (int i = 1; i <= tabCount; i++)
        // {
        //     TabItemSource.Add( new TabViewItem
        //     {
        //         Header = $"和泉妃爱世界第一可爱x{i}!🥰",
        //         Content = 
        //             new TextBlock 
        //             {
        //                 Text = $"和泉妃爱世界第一可爱x{i}!🥰",
        //                 FontSize = 24,
        //                 HorizontalAlignment = HorizontalAlignment.Center,
        //                 VerticalAlignment = VerticalAlignment.Center,
        //                 Height = 328 
        //             }
        //     });
        // }
    }

    [RelayCommand]
    private void AddTab()
    {
        tabCount++;
        TabItemSource.Add(new TabViewItem
        {
            Header = $"和泉妃爱世界第一可爱x{tabCount}!🥰",
            Content = 
                new TextBlock 
                {
                    
                    Text = $"和泉妃爱世界第一可爱x{tabCount}!🥰",
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Height = 328 
                }
        });
    }

    public string[] BreadcrumbBarItemSource => @"C:\Users\Administrator\OneDrive\Pictures\Camera Roll".Split("\\");

    [ObservableProperty]
    private object _segmentedSelectedItem;

    [ObservableProperty]
    private string _selectedItemFormat = "Null";

    partial void OnSegmentedSelectedItemChanged(object value)
    {
        if (value is SegmentedItem item)
        {
            SelectedItemFormat = "当前选中页面: " + item.Content;
        }
    }
}
