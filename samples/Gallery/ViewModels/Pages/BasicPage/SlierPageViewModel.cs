using System.ComponentModel;
using Avalonia.Layout;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class SlierPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Slider");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SliderCurrentValueFormat))]
    private double _sliderCurrentValue;

    public string SliderCurrentValueFormat => SliderCurrentValue.ToString("F");

    [ObservableProperty]
    private Orientation _toolTipSliderOrientation = Orientation.Horizontal;

    public Orientation[] Orientations => [Orientation.Horizontal, Orientation.Vertical];
    
    [ObservableProperty]
    private bool _toolTipSliderIsDisabled;  
}
