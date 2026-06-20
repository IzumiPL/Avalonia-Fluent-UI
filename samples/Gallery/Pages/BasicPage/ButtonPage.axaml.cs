using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class ButtonPage : ViewBase 
{
    public ButtonPage() : base("Button")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Button", StandardButtonCard},
            {"CheckBox", CheckBoxCard},
            {"DropDownButton", DropDownButtonCard},
            {"HyperlinkButton", HyperlinkButtonCard},
            {"RadioButton", RadioButtonCard},
            {"SplitButton", SplitButtonCard},
            {"SwitchButton", ToggleSwitchButtonCard},
            {"ToggleButton", ToggleButtonCard},
        };
    }
}
