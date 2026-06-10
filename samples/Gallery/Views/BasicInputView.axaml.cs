using System.Collections.Generic;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Views;

public partial class BasicInputView : ViewBase
{
    public BasicInputView() : base("BasicInput")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Button", StandardButtonCard},
            {"CheckBox", CheckBoxCard},
            {"ComboBox", ComboBoxCard},
            {"DropDownButton", DropDownButtonCard},
            {"HyperlinkButton", HyperlinkButtonCard},
            {"RadioButton", RadioButtonCard},
            {"Slider", SliderCard},
            {"SplitButton", SplitButtonCard},
            {"SwitchButton", ToggleSwitchButtonCard},
            {"ToggleButton", ToggleButtonCard},
        };
    }
}
