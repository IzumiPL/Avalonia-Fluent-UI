using Avalonia;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gallery.Extensions;

namespace Gallery.ViewModels;

public partial class PopupDrawerPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("PopupControl");

    public int[] RadiusItems => [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24];

    public CornerRadius DrawerRadius => new CornerRadius(TopLeftRadius, TopRightRadius, BottomRightRadius, BottomLeftRadius);

    public double DrawerWidth => DrawerWidthText.ToDoubleOrZero();
    public double DrawerHeight => DrawerHeightText.ToDoubleOrZero();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DrawerWidth))]
    private string? _drawerWidthText = "328";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DrawerHeight))]
    private string? _drawerHeightText = "328";

    [ObservableProperty]
    private bool _drawerCloseButtonIsVisible = true;

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(DrawerRadius))]
    private int _topLeftRadius;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(DrawerRadius))]
    private int _topRightRadius;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(DrawerRadius))]
    private int _bottomRightRadius;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(DrawerRadius))] 
    private int _bottomLeftRadius;
    
    [ObservableProperty]
    private bool _leftDrawerIsOpen;

    [ObservableProperty]
    private bool _rightDrawerIsOpen;

    [ObservableProperty]
    private bool _topDrawerIsOpen;

    [ObservableProperty]
    private bool _bottomDrawerIsOpen;

    [ObservableProperty]
    private bool _drawerIsLightDismissEnabled;

    [ObservableProperty]
    private bool _isShowSimultaneously;

    partial void OnDrawerIsLightDismissEnabledChanged(bool value)
    {
        if (value)
        {
            IsShowSimultaneously = false;
        }
    }

    partial void OnIsShowSimultaneouslyChanged(bool value)
    {
        if (value)
        {
            DrawerIsLightDismissEnabled = false;
        }
    }

    [RelayCommand]
    private void ToggleDrawer(string value)
    {
        switch (value)
        {
            case "Top":
                if (!IsShowSimultaneously)
                {
                    LeftDrawerIsOpen = false;
                    RightDrawerIsOpen = false;
                    BottomDrawerIsOpen = false;
                }

                TopDrawerIsOpen = !TopDrawerIsOpen;
                break;
            case "Bottom":
                if (!IsShowSimultaneously)
                {
                    LeftDrawerIsOpen = false;
                    RightDrawerIsOpen = false;
                    TopDrawerIsOpen = false;
                }
                
                BottomDrawerIsOpen = !BottomDrawerIsOpen;
                break;
            case "Left":
                if (!IsShowSimultaneously)
                {
                    TopDrawerIsOpen = false;
                    RightDrawerIsOpen = false;
                    BottomDrawerIsOpen = false;
                }
                
                LeftDrawerIsOpen = !LeftDrawerIsOpen;
                break;
            case "Right":
                if (!IsShowSimultaneously)
                {
                    TopDrawerIsOpen = false;
                    LeftDrawerIsOpen = false;
                    BottomDrawerIsOpen = false;
                }
                
                RightDrawerIsOpen = !RightDrawerIsOpen;
                break;
        }
    }
}
