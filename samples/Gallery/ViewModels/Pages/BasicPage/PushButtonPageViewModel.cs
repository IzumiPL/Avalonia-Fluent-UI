using Avalonia.Layout;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class PushButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("PushButton");

    [ObservableProperty]
    private HorizontalAlignment[] _horizontalAlignments =
    [
        HorizontalAlignment.Left,
        HorizontalAlignment.Right,
        HorizontalAlignment.Stretch,
        HorizontalAlignment.Center
    ];

    [ObservableProperty]
    private HorizontalAlignment _pushButtonContentAlignment = HorizontalAlignment.Center;

    [ObservableProperty]
    private bool _standardButtonIsDisabled;

    [ObservableProperty]
    private bool _standardButtonWithIconIsDisabled;

    [ObservableProperty]
    private bool _accentStandardButtonIsDisabled;

    [ObservableProperty]
    private bool _transparentStandardButtonIsDisabled;

    [ObservableProperty]
    private bool _transparentStandardButtonWithIconIsDisabled;

    [ObservableProperty]
    private bool _roundButtonIsDisabled;

    [ObservableProperty]
    private bool _accentRoundButtonIsDisabled;

    [ObservableProperty]
    private bool _outlineButtonIsDisabled;

    [ObservableProperty]
    private bool _roundOutlineButtonIsDisabled;

    [ObservableProperty]
    private bool _outlineToolButtonIsDisabled;

    [ObservableProperty]
    private bool _roundOutlineToolButtonIsDisabled;
}
