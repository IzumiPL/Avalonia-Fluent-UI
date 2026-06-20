using System.Collections.Generic;
using Avalonia.Media;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class TextBlockPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("TextBlock");

    public TextBlockPageViewModel()
    {
        for (int i = 6; i <= 64; i += 2)
        {
            _fontSizeItems.Add(i);
        }
    }
    
    [ObservableProperty]
    private bool _textBlockSettingsPaneIsExpand;

    [ObservableProperty]
    private string _textBlockText = "Hello World!";

    [ObservableProperty]
    private double _textBlockFontSize = 32.0;

    [ObservableProperty]
    private TextDecorationCollection? _textDecorations;

    [ObservableProperty]
    private TextDecorationCollection?[] _textDecorationsItems =
    [
        null,
        Avalonia.Media.TextDecorations.Strikethrough,
        Avalonia.Media.TextDecorations.Underline,
        Avalonia.Media.TextDecorations.Baseline,
        Avalonia.Media.TextDecorations.Overline
    ];

    [ObservableProperty]
    private double _textBlockLetterSpacing;

    [ObservableProperty]
    private FontWeight _textBlockFontWeight = FontWeight.Normal;
    
    [ObservableProperty]
    private FontStyle _textBlockFontStyle = FontStyle.Normal;

    [ObservableProperty]
    private List<double> _fontSizeItems = new();

    [ObservableProperty]
    private List<FontWeight> _fontWeightItems = [
        FontWeight.Thin,
        FontWeight.ExtraLight,
        FontWeight.UltraLight,
        FontWeight.Light,
        FontWeight.SemiLight,
        FontWeight.Normal,
        FontWeight.Regular,
        FontWeight.Medium,
        FontWeight.DemiBold,
        FontWeight.SemiBold,
        FontWeight.Bold,
        FontWeight.ExtraBold,
        FontWeight.UltraBold,
        FontWeight.Black,
        FontWeight.Heavy,
        FontWeight.Solid,
        FontWeight.ExtraBlack,
        FontWeight.UltraBlack,
    ];

    [ObservableProperty]
    private List<FontStyle> _fontStyleItems = [
        FontStyle.Normal,
        FontStyle.Italic,
        FontStyle.Oblique
    ];

    [RelayCommand]
    private void ExpandTextBlockSettings() => TextBlockSettingsPaneIsExpand = !TextBlockSettingsPaneIsExpand;
    
}
