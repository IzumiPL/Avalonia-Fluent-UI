using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class OutlineButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("OutlineButton");

    [ObservableProperty]
    private bool _outlinePushButtonIsDisabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutlinePushButtonGroup))]
    private bool _outlinePushButtonIsMC = true;

    public OutlineButtonGroup? OutlinePushButtonGroup => OutlinePushButtonIsMC ? _outlineButtonGroup : null;
    private readonly OutlineButtonGroup _outlineButtonGroup = new();

    [ObservableProperty]
    private bool _outlineToolButtonIsDisabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutlineToolButtonGroup))]
    private bool _outlineToolButtonIsMC = true;

    public OutlineButtonGroup? OutlineToolButtonGroup => OutlineToolButtonIsMC ? _outlineToolButtonGroup : null;
    private readonly OutlineButtonGroup _outlineToolButtonGroup = new();
}
