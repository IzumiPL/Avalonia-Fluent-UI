using System;
using Avalonia.Media;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class StatusAndInformationViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("StatusAndInformation");

    public InfoBarSeverity[] InfoBarSeverityItems => 
    [
        InfoBarSeverity.Informational,
        InfoBarSeverity.Success,
        InfoBarSeverity.Warning,
        InfoBarSeverity.Error
    ];

    [ObservableProperty]
    private InfoBarSeverity _currentInfoBarSeverity = InfoBarSeverity.Success;

    [ObservableProperty]
    private bool _infoBarIsClosable;

    [ObservableProperty]
    private bool _infoBarIsOpen = true;

    [RelayCommand]
    private void ResetInfoBar()
    {
        InfoBarIsOpen = true;
    }

    [ObservableProperty]
    private bool _progressBarIsIndeterminate = true;

    [ObservableProperty]
    private double _progressBarCurrentValue = 24.0;

    [ObservableProperty]
    private double _progressRingCurrentValue = 24.0;

    public IBrush ProgressRingBackground => new SolidColorBrush(ProgressRingColor);

    [ObservableProperty]
    private bool _showPercent = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressRingBackground))]
    private Color _progressRingColor = Colors.Transparent;
}
