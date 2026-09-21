using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.Settings;

public class AppSettings
{
    public TitleBarSettings TitleBar { get; } = new();
    public NavigationSettings Navigation { get; } = new();
    public MainWindowSettings MainWindow { get; } = new();
}

public partial class TitleBarSettings : ObservableObject
{
    [ObservableProperty]
    private bool _titleBarIsVisible;

    [ObservableProperty]
    private Color _titleBarBackground = Colors.Transparent;
    
    [ObservableProperty]
    private bool _titleBarContentIsVisible = true;
    
    [ObservableProperty]
    private bool _titleBarTitleIsVisible = true;
    
    [ObservableProperty]
    private bool _titleBarIconIsVisible = true;

    [ObservableProperty]
    private double _titleBarHeight = 45;
}

public partial class MainWindowSettings : ObservableObject
{
    [ObservableProperty]
    private bool _canResize = true;
    
    [ObservableProperty]
    private bool _canMaximize = true;
    
    [ObservableProperty]
    private bool _canMinimize = true;
    
    [ObservableProperty]
    private bool _closeButtonIsVisible = true;
    
    [ObservableProperty]
    private bool _minimizeButtonIsVisible = true;
    
    [ObservableProperty]
    private bool _maximizeButtonIsVisible = true;
}

public partial class NavigationSettings : ObservableObject
{
    [ObservableProperty]
    private double _paneClosedWidth = 48;

    [ObservableProperty]
    private double _paneOpenWidth = 328;

    [ObservableProperty]
    private double _expandedModeThresholdWidth = 1008;

    [ObservableProperty]
    private bool _isBackButtonVisible = true;

    [ObservableProperty]
    private bool _isPaneToggleButtonVisible = true;

    [ObservableProperty]
    private bool _isSettingsItemVisible = true;

    [ObservableProperty]
    private bool _isPaneFooterSeparatorVisible = true;

    [ObservableProperty]
    private bool _isPaneVisible = true;
}
