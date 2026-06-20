using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using Gallery.Extensions;

namespace Gallery.ViewModels;

public partial class BorderPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Border");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentBorderBrush))]
    private Brush? _borderBrush = new SolidColorBrush(Colors.DarkOrange);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentBorderBackground))]
    private Brush? _borderBackground = new SolidColorBrush(Colors.Azure);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentBoxShadow))]
    private BoxShadows _boxShadow = new BoxShadows(new BoxShadow {Color = Color.Parse("#000000"), Blur = 12, OffsetX = 0, OffsetY = 5, Spread = 0, IsInset = false});

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentBorderThickness))]
    private Thickness _borderThickness = new Thickness(1);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentBorderRadius))]
    private CornerRadius _borderRadius = new CornerRadius(8);

    [ObservableProperty]
    private bool _isEnableBorderShadow = true;

    [ObservableProperty]
    private string? _borderBrushHex;

    [ObservableProperty]
    private string? _borderBackgroundHex;

    [ObservableProperty]
    private string? _borderWidth;

    [ObservableProperty]
    private string? _br;
    
    public string CurrentBorderBrush => $"BorderBrush: {BorderBrush}";
    public string CurrentBorderBackground => $"BorderBackground: {BorderBackground}";
    public string CurrentBoxShadow => $"BoxShadow: {BoxShadow}";
    public string CurrentBorderThickness => $"BorderThickness: {BorderThickness}";
    public string CurrentBorderRadius => $"BorderRadius: {BorderRadius}";
    
    [ObservableProperty]
    private double _canvasLeft = 100;

    [ObservableProperty]
    private double _canvasTop = 100;
    
    partial void OnIsEnableBorderShadowChanged(bool value)
    {
        BoxShadow = value
            ? new BoxShadows(new BoxShadow { Color = Color.Parse("#000000"), Blur = 12, OffsetX = 0, OffsetY = 5, Spread = 0, IsInset = false })
            : new BoxShadows();
    }

    partial void OnBorderBrushHexChanged(string? value)
    {
        if (VerifyColor(value, out var brush)) BorderBrush = brush;
    }

    partial void OnBorderBackgroundHexChanged(string? value)
    {
        if (VerifyColor(value, out var brush)) BorderBackground = brush;
    }

    partial void OnBorderWidthChanged(string? value) => BorderThickness = new Thickness(value.ToDoubleOrZero());

    partial void OnBrChanged(string? value) => BorderRadius = new CornerRadius(value.ToDoubleOrZero());
    
    private bool VerifyColor(string? hex, out Brush? brush)
    {
        hex = hex?.Trim();
        if (!string.IsNullOrEmpty(hex) && Color.TryParse(hex, out var c))
        {
            brush = new SolidColorBrush(c);
            return true;
        }
        brush = null;
        return false;
    }
}
