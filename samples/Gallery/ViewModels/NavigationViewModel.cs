using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;
using Gallery.Services;

namespace Gallery.ViewModels;

public partial class NavigationViewModel : ViewModelBase
{
    public List<ButtonItemModel> NavigationViewItemSource { get; }
    public override string Title => LocalizationService.Instance.GetString("Navigation");

    public NavigationViewModel()
    {
        NavigationViewItemSource = ButtonItemModel.CreateList(
            ("NavigationView", "NavigationView", "NavigationView", "Navigation panel for page switching and menu navigation"),
            ("BreadcrumbBar", "BreadcrumbBar", "BreadcrumbBar", "Breadcrumb navigation view"),
            ("Pivot", "Segmented", "SegmentedView", "This is the segmented navigation bar")
        );
    }
}
