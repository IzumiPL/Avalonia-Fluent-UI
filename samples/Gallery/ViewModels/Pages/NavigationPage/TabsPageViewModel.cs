using System.ComponentModel;
using Avalonia.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class TabsPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Tabs");
    
    [ObservableProperty]
    private Dock _tabStripPlacement = Dock.Top;

    public Dock[] TabStripPlacements => [ Dock.Top, Dock.Left, Dock.Right, Dock.Bottom];
}
