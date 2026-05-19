using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;

namespace Gallery.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public HomeViewModel()
    {
#if DEBUG
        Debug.WriteLine("HomeViewModel Init");
#endif
        ButtonItemSource = new List<ButtonItemModel>()
        {
            new("Button", "Button", "A control that responds to user input and emit clicked signal."),
            new("Checkbox", "CheckBox", "A control that a user can select or clear."),
            new("ComboBox", "ComboBox", "A drop-down list of items a user can select from."),
            new("DropDownButton", "DropDownButton", "A button that display a flyout of choices when clicked."),
            new("HyperlinkButton", "HyperlinkButton", "A button that appears as hyperlink text, and can navigate to a RUL or handle a Click event."),
            new("RadioButton", "RadioButton", "A control that allows a user to select a single option from a group of options."),
            new("Slider", "Slider", "A control that lets the user select from a range of values by moving a Thumb control along a track."),
            new("SplitButton", "SplitButton", "A two-part button that displays a flyout when its secondary part is clicked."),
            new("ToggleSwitch", "SwitchButton", "A switch that can be toggled between 2 states."),
            new("ToggleButton", "ToggleButton", "A button that can be switched between two states like a CheckBox."),
        };

        DateTimeItemSource = new List<ButtonItemModel>()
        {
            new("CalendarDatePicker", "CalendarDatePicker", "A control that lets a user pick a date value using a calendar."),
            new("DatePicker", "DatePicker", "A control that lets a user pick a date value."),
            new("TimePicker", "TimePicker", "A configurable control that lets a user pick a time value."),
        };

        DialogItemSource = new List<ButtonItemModel>()
        {
            new("Flyout", "TaskDialog", "A task dialog."),
            new("Flyout", "Flyout", "Shows contextual information and enables user interaction."),
            new("ContentDialog", "ContentDialog", "A content dialog with mask."),
            new("TeachingTip", "TeachingTip", "A content-rich flyout for guiding users and enabling teaching moments."),
        };

        LayoutItemSource = new List<ButtonItemModel>()
        {
            new("Border", "Border", "Simple border layout"),
            new("Canvas", "Canvas", "Can draw any shape canvas control"),
            new("SplitView", "SplitView", "split view layout"),
            new("Grid", "Grid", "A grid layout"),
            new("RelativePanel", "RelativePanel", "Relative panel, control relative layout"),
            new("StackPanel", "StackPanel", "A stackPanel layout"),
            new("Expander", "Expander", "A expander layout"),
        };

        MenuAndToolBarItemSource = new List<ButtonItemModel>()
        {
            new("MenuFlyout", "Menu", "Shows a contextual list of simple commands or options."),
            new("MenuBar", "MenuBar", "Simple top menu bar"),
            new("CommandBar", "CommandBar", "Display the command bar"),
            new("CommandBarFlyout", "CommandBarFlyout", "A mini-toolbar displaying proactive commands, and an optional menu of commands."),
        };

        NavigationViewItemSource = new List<ButtonItemModel>()
        {
            new("NavigationView", "NavigationView", "Navigation panel for page switching and menu navigation"),
            new("PageTransition", "PageTransition", "Page switching control with animation"),
            new("BreadcrumbBar", "BreadcrumbBar", "Breadcrumb navigation view"),
            new("Pivot", "Segmented", "This is the segmented navigation bar")
        };

        StatusAndInformationItemSource = new List<ButtonItemModel>()
        {
            new("ToolTip", "ToolTip", "A control tooltip, hover show tooltip"),
            new("InfoBadge", "InfoBadge", "Information badges can display a variety of information"),
            new("InfoBar", "InfoBar", "Information bar can display a variety of information and can be closed"),
            new("ProgressBar", "ProgressBar", "The progress bar has two states: confirmed and uncertain."),
            new("ProgressRing", "ProgressRing", "A progress ring"),
        };

        TextItemSource = new List<ButtonItemModel>()
        {
            new("TextBlock", "TextBlock", "Text block, used to display text"),
            new("TextBox", "TextBox", "Text input box"),
            new("PasswordBox", "PasswordBox", "Password input box, which can be turned on and off to display the password"),
            new("NumberBox", "NumberBox", "Numeric input box that can be fine-tuned"),
        };

        ViewItemSource = new List<ButtonItemModel>()
        {
            new("ListBox", "ListBox", "List box, can display multiple items"),
            new("TreeView", "TreeView", "A tree view"),
            new("ScrollViewer", "FlipView", "Carousel view, a control suitable for displaying multiple pictures")
        };
    }

    public event Action<string, string>? GotoControlEvent;

    public void ReleaseImages()
    {
        foreach (var item in AllSources)
        {
            item.ReleaseImage();
        }
    }

    public IEnumerable<ButtonItemModel> AllSources
    {
        get
        {
            return ButtonItemSource
                .Concat(DateTimeItemSource)
                .Concat(DialogItemSource)
                .Concat(LayoutItemSource)
                .Concat(MenuAndToolBarItemSource)
                .Concat(NavigationViewItemSource)
                .Concat(StatusAndInformationItemSource)
                .Concat(TextItemSource)
                .Concat(ViewItemSource);
        }
    }

    [RelayCommand]
    private void Goto(object value)
    {
        if (value is Button button)
        {
            var name = button.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault(x => x.Name == "Title")?.Text;
            GotoControlEvent?.Invoke(button.Tag?.ToString()!, name);

#if DEBUG
            Debug.WriteLine($"Goto => View: {button.Tag}, Name: {name}");
#endif
        }
    }

    public List<ButtonItemModel> ButtonItemSource { get; }
    public List<ButtonItemModel> DateTimeItemSource { get; }
    public List<ButtonItemModel> DialogItemSource { get; }
    public List<ButtonItemModel> LayoutItemSource { get; }
    public List<ButtonItemModel> MenuAndToolBarItemSource { get; }
    public List<ButtonItemModel> NavigationViewItemSource { get; }
    public List<ButtonItemModel> StatusAndInformationItemSource { get; }
    public List<ButtonItemModel> TextItemSource { get; }
    public List<ButtonItemModel> ViewItemSource { get; }
}
