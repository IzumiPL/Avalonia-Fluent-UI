using System.Collections.ObjectModel;
using Avalonia.Controls;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Icons;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;

namespace Gallery.ViewModels;

public partial class TabsPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Tabs");

    private int _tabItemCount = 5;

    public TabsPageViewModel()
    {
        TabViewItems = new ObservableCollection<TabIteModel>(
        [
            new TabIteModel("Document 1", true, null, "Document 1", FluentIcon.Document),
            new TabIteModel("Document 2", true, null, "Document 2", FluentIcon.Document),
            new TabIteModel("Document 3", true, null, "Document 3", FluentIcon.Document),
            new TabIteModel("Document 4", true, null, "Document 4", FluentIcon.Document),
            new TabIteModel("Document 5", true, null, "Document 5", FluentIcon.Document),
        ]);
    }

    [ObservableProperty]
    private Dock _tabStripPlacement = Dock.Top;

    public Dock[] TabStripPlacements => [Dock.Top, Dock.Left, Dock.Right, Dock.Bottom];

    [ObservableProperty]
    private bool _tabViewIsAddButtonVisible;

    [ObservableProperty]
    private bool _tabViewAllowDropTabs;

    [ObservableProperty]
    private bool _tabViewCanReorderTabs;

    [ObservableProperty]
    private TabViewCloseButtonOverlayMode _tabViewCloseButtonOverlayMode = TabViewCloseButtonOverlayMode.Auto;

    [ObservableProperty]
    private object? _tabViewSelectedItem;

    public ObservableCollection<TabIteModel> TabViewItems { get; set; }

    public TabViewTabStripLocation[] TabViewTabStripLocations => [TabViewTabStripLocation.Left, TabViewTabStripLocation.Top, TabViewTabStripLocation.Right, TabViewTabStripLocation.Bottom];
    public TabViewWidthMode[] TabViewTabWidthModes => [TabViewWidthMode.Equal, TabViewWidthMode.SizeToContent, TabViewWidthMode.Compact];
    public TabViewCloseButtonOverlayMode[] TabViewCloseButtonOverlayModes => [TabViewCloseButtonOverlayMode.Auto, TabViewCloseButtonOverlayMode.OnPointerOver, TabViewCloseButtonOverlayMode.Always];

    [ObservableProperty]
    private TabViewTabStripLocation _tabViewTabStripLocation = TabViewTabStripLocation.Top;

    [ObservableProperty]
    private TabViewWidthMode _tabViewTabWidthMode = TabViewWidthMode.Equal;

    [ObservableProperty]
    private string? _tabViewStripHeader = "Header";

    [ObservableProperty]
    private string? _tabViewStripFooter = "Footer";

    [ObservableProperty]
    private bool _tabViewStripHeaderIsVisible;

    [ObservableProperty]
    private bool _tabViewStripFooterIsVisible;

    [RelayCommand]
    public void AddTab()
    {
        _tabItemCount++;
        TabViewItems.Add(new TabIteModel($"Document {_tabItemCount}", true, null, $"Document {_tabItemCount}", FluentIcon.Document));
    }

    private void CloseTabViewItem(TabIteModel item)
    {
        if (item.IsClosable)
        {
            TabViewItems.Remove(item);
        }
    }

    public void TabViewRequestClose(object? item)
    {
        if (item is TabIteModel tab)
        {
            CloseTabViewItem(tab);
        }
        else if (item is TabViewItem tvi && tvi.DataContext is TabIteModel model)
        {
            CloseTabViewItem(model);
        }
    }

    [RelayCommand]
    private void ClearAllTabViewItem()
    {
        TabViewItems.Clear();
    }
}
