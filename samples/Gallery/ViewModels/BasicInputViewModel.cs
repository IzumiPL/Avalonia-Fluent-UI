using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;
using Gallery.Services;

namespace Gallery.ViewModels;

public partial class BasicInputViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("BasicInput");
    
    public List<ButtonItemModel> ButtonItemSource { get; private set; }
    
    public BasicInputViewModel()
    {
        ButtonItemSource = ButtonItemModel.CreateList(
            ("Button", "Button", "Button", "A control that responds to user input and emit clicked signal."),
            ("Checkbox", "CheckBox", "Button", "A control that a user can select or clear."),
            ("ComboBox", "ComboBox", "ComboBox", "A drop-down list of items a user can select from."),
            ("DropDownButton", "DropDownButton", "Button", "A button that display a flyout of choices when clicked."),
            ("HyperlinkButton", "HyperlinkButton", "Button", "A button that appears as hyperlink text, and can navigate to a RUL or handle a Click event."),
            ("RadioButton", "RadioButton", "Button", "A control that allows a user to select a single option from a group of options."),
            ("Slider", "Slider", "Slider", "A control that lets the user select from a range of values by moving a Thumb control along a track."),
            ("SplitButton", "SplitButton", "Button", "A two-part button that displays a flyout when its secondary part is clicked."),
            ("ToggleSwitch", "SwitchButton", "Button", "A switch that can be toggled between 2 states."),
            ("ToggleButton", "ToggleButton", "Button", "A button that can be switched between two states like a CheckBox.")
        );
    }
}
