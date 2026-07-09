using Avalonia.Layout;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class ButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Button");

    [ObservableProperty]
    private HorizontalAlignment[] _horizontalAlignments =
    [
        HorizontalAlignment.Left,
        HorizontalAlignment.Right,
        HorizontalAlignment.Stretch,
        HorizontalAlignment.Center
    ];

    // PushButton
    [ObservableProperty]
    private HorizontalAlignment _pushButtonContentAlignment = HorizontalAlignment.Center;

    [ObservableProperty]
    private bool _pushButtonIsDisable;

    // ToolButton 
    [ObservableProperty]
    private HorizontalAlignment _toolButtonContentAlignment = HorizontalAlignment.Center;
    
    [ObservableProperty]
    private bool _toolButtonIsDisable;

    // StatusSwitchButton
    [ObservableProperty]
    private bool _statusSwitchButtonIsDisable;
    
    // SplitButton
    [ObservableProperty]
    private bool _splitButtonIsDisable;

    // RadioButton
    [ObservableProperty]
    private bool _radioButtonIsDisabled;

    // HyperlinkButton
    [ObservableProperty]
    private bool _hyperlinkButtonIsDisable;

    // ToggleSwitchButton
    [ObservableProperty]
    private bool _toggleSwitchButtonIsDisable;

    // CheckBox
    [ObservableProperty]
    private bool _checkBoxIsDisable;

    [ObservableProperty]
    private bool _checkBoxIsThreeState;

    // DropDownButton
    [ObservableProperty]
    private bool _dropDownButtonIsDisable;
    
    [ObservableProperty]
    private bool _transparentDropDownButtonIsDisable;
    
    // OutlineToolButton
    [ObservableProperty]
    private bool _outlineToolButtonIsDisabled;

    // OutlinePushButton
    [ObservableProperty]
    private bool _outlinePushButtonIsDisabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutlinePushButtonGroup))]
    private bool _outlinePushButtonIsMC = true;

    public OutlineButtonGroup? OutlinePushButtonGroup => OutlinePushButtonIsMC ? _outlineButtonGroup : null;
    private readonly OutlineButtonGroup _outlineButtonGroup = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutlineToolButtonGroup))]
    private bool _outlineToolButtonIsMC = true;

    public OutlineButtonGroup? OutlineToolButtonGroup => OutlineToolButtonIsMC ? _outlineToolButtonGroup : null;
    private readonly OutlineButtonGroup _outlineToolButtonGroup = new();
    
    // RadioButton
    [ObservableProperty]
    private bool _subTitleRadioButtonIsDisabled; 

    [ObservableProperty]
    private bool _chipsRadioButtonIsEnabled;

    [ObservableProperty]
    private bool _outlinedClassButtonIsDisabled; 

    // RoundButton
    [ObservableProperty]
    private bool _roundButtonIsDisabled; 
    
    // FilledButton
    [ObservableProperty]
    private bool _filledButtonIsDisabled;
}
