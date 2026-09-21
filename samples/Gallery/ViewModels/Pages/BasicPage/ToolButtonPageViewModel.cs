using Avalonia.Layout;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class ToolButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("ToolButton");

    [ObservableProperty]
    private HorizontalAlignment[] _horizontalAlignments =
    [
        HorizontalAlignment.Left,
        HorizontalAlignment.Right,
        HorizontalAlignment.Stretch,
        HorizontalAlignment.Center
    ];

    [ObservableProperty]
    private HorizontalAlignment _toolButtonContentAlignment = HorizontalAlignment.Center;

    [ObservableProperty]
    private bool _toolButtonIsDisabled;

    [ObservableProperty]
    private bool _transparentToolButtonIsDisabled;

    [ObservableProperty]
    private bool _accentToolButtonIsDisabled;

    [ObservableProperty]
    private bool _roundToolButtonIsDisabled;

    [ObservableProperty]
    private bool _accentRoundToolButtonIsDisabled;

    [ObservableProperty]
    private bool _outlineToolButtonIsDisabled;

    [ObservableProperty]
    private bool _roundOutlineToolButtonIsDisabled;
}
