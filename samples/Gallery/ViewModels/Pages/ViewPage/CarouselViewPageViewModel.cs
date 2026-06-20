using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaFluentUI.Controls.Enums;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;

namespace Gallery.ViewModels;

public partial class CarouselViewPageViewModel : ViewModelBase
{
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CarouselAllCountFormat))]
    private int _carouselAllCount = 5;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CarouselCurrentIndexFormat))]
    private int _carouselCurrentIndex;

    [ObservableProperty]
    private ObservableCollection<CarouselData> _carouselItems;

    [ObservableProperty]
    private FlipOrientation _currentFlipOrientation = FlipOrientation.Horizontal;

    [ObservableProperty]
    private double _flipViewAutoPlayInterval = 1500;

    [ObservableProperty]
    private bool _flipViewIsAutoPlay;

    [ObservableProperty]
    private ObservableCollection<string> _flipViewItems;

    [ObservableProperty]
    private bool _isAutoPlay;

    private int _target = 1;

    public CarouselViewPageViewModel()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnCarouselAutoPlay;

        var pages = new CarouselData[CarouselAllCount];
        for (var i = 1; i <= CarouselAllCount; i++)
        {
            pages[i - 1] = new CarouselData($"Page {i}", GetRandomHexColor());
        }

        CarouselItems = new ObservableCollection<CarouselData>(pages);

        FlipViewItems = new ObservableCollection<string>
        {
            "avares://Gallery/Assets/Images/mc.jpg",
            "avares://Gallery/Assets/Images/1.jpg",
            "avares://Gallery/Assets/Images/2.jpg",
            "avares://Gallery/Assets/Images/3.jpg",
            "avares://Gallery/Assets/Images/4.jpg",
            "avares://Gallery/Assets/Images/bg.jpg"
        };
    }

    public override string Title => LocalizationService.Instance.GetString("CarouselView");

    public string CarouselAllCountFormat => LocalizationService.Instance.GetString("PageCount") + ": " + CarouselAllCount;

    public string CarouselCurrentIndexFormat => LocalizationService.Instance.GetString("CurrentPage") + ": " + (CarouselCurrentIndex + 1);

    public FlipOrientation[] FlipOrientations => [FlipOrientation.Horizontal, FlipOrientation.Vertical];

    public double[] Intervals => [500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 2000, 5000];

    [ObservableProperty]
    private int _flipViewMaxVisiblePips = 6;

    public int[] FlipViewMaxVisiblePipsItems => [1, 2, 3, 4, 5, 6];

    private void OnCarouselAutoPlay(object? sender, EventArgs e)
    {
        if (CarouselCurrentIndex == CarouselAllCount - 1)
        {
            _target = -1;
        }

        if (CarouselCurrentIndex == 0)
        {
            _target = 1;
        }

        CarouselCurrentIndex += _target;
    }

    [RelayCommand]
    private void AddCarousel()
    {
        CarouselItems.Add(
            new CarouselData(
                $"Page {CarouselItems.Count + 1}",
                GetRandomHexColor())
        );
        CarouselAllCount++;
    }

    private IBrush GetRandomHexColor()
    {
        var random = new Random();
        var rgb = random.Next(0x1000000);
        return Brush.Parse($"#{rgb:X6}");
    }

    partial void OnIsAutoPlayChanged(bool value)
    {
        if (value)
        {
            _timer.Start();
            return;
        }

        _timer.Stop();
    }
}
