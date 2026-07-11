using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class ProgressPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("ProgressBar");
    
    [ObservableProperty]
    private bool _progressBarIsIndeterminate = true;

    [ObservableProperty]
    private double _progressBarCurrentValue = 24.0;

    [ObservableProperty]
    private double _progressRingCurrentValue = 24.0;

    public IBrush ProgressRingBackground => new SolidColorBrush(ProgressRingColor);
    
    public Orientation[] ProgressBarOrientations => [ Orientation.Horizontal, Orientation.Vertical ];

    [ObservableProperty]
    private bool _filledProgressBarIsIndeterminate;

    [ObservableProperty]
    private double _filledProgressBarCurrentValue = 64;

    [ObservableProperty]
    private bool _filledProgressBarShowProgressText;

    [ObservableProperty]
    private bool _showPercent = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressRingBackground))]
    private Color _progressRingColor = Colors.Transparent;
}
