using System;
using System.ComponentModel;
using Avalonia.Controls;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class FlyoutPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Flyout");
    
    [ObservableProperty]
    private bool _fluentFlyoutIsOpen;

    [ObservableProperty]
    private PlacementMode _flyoutPlacement = PlacementMode.Top;

    [ObservableProperty]
    private PlacementMode[] _flyoutPlacements =
    [
        PlacementMode.Left,
        PlacementMode.Top,
        PlacementMode.Right,
        PlacementMode.Bottom,
        PlacementMode.LeftEdgeAlignedBottom,
        PlacementMode.LeftEdgeAlignedTop,
        PlacementMode.TopEdgeAlignedLeft,
        PlacementMode.TopEdgeAlignedRight,
        PlacementMode.RightEdgeAlignedBottom,
        PlacementMode.RightEdgeAlignedTop,
        PlacementMode.BottomEdgeAlignedLeft,
        PlacementMode.BottomEdgeAlignedRight,
        PlacementMode.Pointer,
        PlacementMode.Center
    ];

    [ObservableProperty]
    private bool _teachingTipIsOpen;

    [ObservableProperty]
    private TeachingTipPlacementMode _teachingTipPlacement = TeachingTipPlacementMode.Top;

    public TeachingTipPlacementMode[] TeachingTipPlacements => 
    [ 
        TeachingTipPlacementMode.Top, 
        TeachingTipPlacementMode.Auto, 
        TeachingTipPlacementMode.Left, 
        TeachingTipPlacementMode.Right, 
        TeachingTipPlacementMode.Center, 
        TeachingTipPlacementMode.Bottom, 
        TeachingTipPlacementMode.LeftTop, 
        TeachingTipPlacementMode.TopLeft, 
        TeachingTipPlacementMode.RightTop, 
        TeachingTipPlacementMode.TopRight, 
        TeachingTipPlacementMode.BottomLeft, 
        TeachingTipPlacementMode.LeftBottom, 
        TeachingTipPlacementMode.BottomRight, 
        TeachingTipPlacementMode.RightBottom, 
    ];

    [RelayCommand]
    private void CloseFlyout()
    {
        FluentFlyoutIsOpen = false;
    }

    [RelayCommand]
    private void CloseTeachingTip()
    {
        TeachingTipIsOpen = false;
    }

    [RelayCommand]
    private void ShowTeachingTip()
    {
        Console.WriteLine(TeachingTipIsOpen);
        if (TeachingTipIsOpen) TeachingTipIsOpen = false;
        TeachingTipIsOpen = true;
    }
}
