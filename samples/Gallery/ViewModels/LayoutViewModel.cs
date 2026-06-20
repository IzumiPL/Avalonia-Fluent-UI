using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;
using Gallery.Services;

namespace Gallery.ViewModels;

public partial class LayoutViewModel : ViewModelBase
{
    public List<ButtonItemModel> LayoutItemSource { get; }
    public override string Title => LocalizationService.Instance.GetString("Layout");
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MenuHorizontalAlignment))]
    private bool _isPaneOpen;

    public LayoutViewModel()
    {
        LayoutItemSource = ButtonItemModel.CreateList(
            ("Border", "Border", "Border", "Simple border layout"),
            ("Canvas", "Canvas", "Border", "Can draw any shape canvas control"),
            ("SplitView", "SplitView","", "split view layout"),
            ("Grid", "Grid", "Panel", "A grid layout"),
            ("RelativePanel", "RelativePanel", "Panel", "Relative panel, control relative layout"),
            ("StackPanel", "StackPanel", "Panel", "A stackPanel layout"),
            ("Expander", "Expander", "Panel", "A expander layout")
        );
    }

    public HorizontalAlignment MenuHorizontalAlignment => IsPaneOpen ? HorizontalAlignment.Right : HorizontalAlignment.Left;

    [RelayCommand]
    private void TogglePanel()
    {
        IsPaneOpen = !IsPaneOpen;
    }
}
