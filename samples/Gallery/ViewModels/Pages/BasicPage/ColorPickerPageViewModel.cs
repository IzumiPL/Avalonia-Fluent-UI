using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class ColorPickerPageViewModel : ViewModelBase
{
    public override string Title => "Color Picker";

    [ObservableProperty]
    public partial bool FluentColorViewIsDisabled { get; set; } = false;

    [ObservableProperty]
    public partial ColorSpectrumShape FluentColorViewColorSpectrumShape { get; set; } = ColorSpectrumShape.Box;

    [ObservableProperty]
    public partial bool FluentColorViewIsAlphaVisible { get; set; } = true;

    public ColorSpectrumShape[] FluentColorViewColorSpectrumShapes => [ColorSpectrumShape.Box, ColorSpectrumShape.Ring];
}
