using Avalonia.Layout;
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
    [NotifyPropertyChangedFor(nameof(OutlinePushButtonGroupName))]
    private bool _outlinePushButtonIsMC = true;
    
    public string? OutlinePushButtonGroupName => OutlinePushButtonIsMC ? null : "OutlinePushButtonGroup1";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutlineToolButtonGroupName))]
    private bool _outlineToolButtonIsMC = true;
    
    public string? OutlineToolButtonGroupName => OutlineToolButtonIsMC ? null : "OutlineToolButtonGroup1";
    
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
