using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gallery.Extensions;

namespace Gallery.ViewModels;

public partial class PanelPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Panel");

    public PanelPageViewModel()
    {
        for (int i = 1; i <= 20; i++)
        {
            Items.Add($"Item: {i}");
        }
        Items.CollectionChanged += (_, _) =>
        {
            ItemCountFormat = $"项目个数: {Items.Count}";
        };
    }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DockLeftWidth))]
    private string? _inputDockLeftWidth = "100";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DockRightWidth))]
    private string? _inputDockRightWidth = "100";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DockTopHeight))]
    private string? _inputDockTopHeight = "64";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DockBottomHeight))]
    private string? _inputDockBottomHeight = "64";
    
    public double DockLeftWidth => InputDockLeftWidth.ToDoubleOrZero();
    public double DockRightWidth => InputDockRightWidth.ToDoubleOrZero();
    public double DockBottomHeight => InputDockBottomHeight.ToDoubleOrZero();
    public double DockTopHeight => InputDockTopHeight.ToDoubleOrZero();

    [ObservableProperty]
    private Orientation _stackPanelOrientation = Orientation.Vertical;

    [ObservableProperty]
    private int _selectedOrientationIndex;

    [ObservableProperty]
    private ObservableCollection<TabItem> _tabItems =
    [
        new TabItem { Header = "Tab 1", Content = "Tab 1 Interface" },
        new TabItem { Header = "Tab 2", Content = "Tab 2 Interface" },
        new TabItem { Header = "Tab 3", Content = "Tab 3 Interface" },
        new TabItem { Header = "Tab 4", Content = "Tab 4 Interface" },
        new TabItem { Header = "Tab 5", Content = "Tab 5 Interface" },
    ];

    [RelayCommand]
    private void OnAddTabItem()
    {
        TabItems.Add(new TabItem { Header = $"Tab {TabItems.Count + 1}", Content = $"Tab {TabItems.Count + 1} Interface" });
    }

    partial void OnSelectedOrientationIndexChanged(int value)
    {
        StackPanelOrientation = value == 0 ? Orientation.Vertical : Orientation.Horizontal;
    }

    [ObservableProperty]
    private ObservableCollection<string> _items = new ObservableCollection<string>();

    [ObservableProperty]
    private int[] _addItemCounts = [10, 50, 100, 200, 500, 1000];

    [ObservableProperty]
    private string _itemCountFormat = "项目个数: 20"; 
    
    [ObservableProperty]
    private int _addCount;
    
    [RelayCommand]
    private async Task AddItem()
    {
        int count = Items.Count;
        
        for (int i = count + 1; i <= AddCount + count; i++)
        {
            Items.Add($"Item {i}");

            await Task.Yield();
            await Task.Delay(10);
        }
    }
}
