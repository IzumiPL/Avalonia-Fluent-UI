using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class MediaViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Media");

    public Stretch[] Stretchs => [Stretch.UniformToFill, Stretch.None, Stretch.Fill, Stretch.Uniform];
    
    [ObservableProperty]
    private Stretch _imageStretch = Stretch.UniformToFill;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageLabelCornerRadiusFormat))]
    private double _imageLabelCornerRadius = 8;

    [ObservableProperty]
    private BitmapBlendingMode _imageLabelBlendMode = BitmapBlendingMode.Unspecified;

    public BitmapBlendingMode[] ImageLabelBlendModes => 
    [
        BitmapBlendingMode.Unspecified,
        BitmapBlendingMode.SourceOver,
        BitmapBlendingMode.Source,
        BitmapBlendingMode.Destination,
        BitmapBlendingMode.DestinationOver,
        BitmapBlendingMode.SourceIn,
        BitmapBlendingMode.DestinationIn,
        BitmapBlendingMode.SourceOut,
        BitmapBlendingMode.DestinationOut,
        BitmapBlendingMode.SourceAtop,
        BitmapBlendingMode.DestinationAtop,
        BitmapBlendingMode.Xor,
        BitmapBlendingMode.Plus,
        BitmapBlendingMode.Screen,
        BitmapBlendingMode.Overlay,
        BitmapBlendingMode.Darken,
        BitmapBlendingMode.Lighten,
        BitmapBlendingMode.ColorDodge,
        BitmapBlendingMode.ColorBurn,
        BitmapBlendingMode.HardLight,
        BitmapBlendingMode.SoftLight,
        BitmapBlendingMode.Difference,
        BitmapBlendingMode.Exclusion,
        BitmapBlendingMode.Multiply,
        BitmapBlendingMode.Hue,
        BitmapBlendingMode.Saturation,
        BitmapBlendingMode.Color,
        BitmapBlendingMode.Luminosity,
    ];

    [ObservableProperty]
    private BitmapInterpolationMode _imageLabelInterpolationMode = BitmapInterpolationMode.HighQuality;

    public BitmapInterpolationMode[] ImageLabelInterpolationModes => 
    [
        BitmapInterpolationMode.None,
        BitmapInterpolationMode.LowQuality,
        BitmapInterpolationMode.Unspecified,
        BitmapInterpolationMode.MediumQuality,
        BitmapInterpolationMode.HighQuality,
    ];

    [ObservableProperty]
    private double _imageLabelDecodeWidth = 0;

    [ObservableProperty]
    private double _imageLabelDecodeHeight = 0;

    [ObservableProperty]
    private string? _imageLabelDelegate = null;
    
    public CornerRadius ImageLabelCornerRadiusFormat => new CornerRadius(ImageLabelCornerRadius);

    public double ImageLabelWidthFormat => ImageLabelWidth < 0 ? Double.NaN : ImageLabelWidth;
    public double ImageLabelHeightFormat =>  ImageLabelHeight < 0 ? Double.NaN : ImageLabelHeight; 

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(ImageLabelHeightFormat))]
    private double _imageLabelHeight = -1;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageLabelWidthFormat))]
    private double _imageLabelWidth = -1;
}
