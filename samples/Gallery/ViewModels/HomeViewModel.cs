using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
            new ButtonItemModel(LoadImage("Button"), "Button", "A control that responds to user input and emit clicked signal."),
            new ButtonItemModel(LoadImage("Checkbox"), "CheckBox", "A control that a user can select or clear."),
            new ButtonItemModel(LoadImage("ComboBox"), "ComboBox", "A drop-down list of items a user can select from."),
            new ButtonItemModel(LoadImage("DropDownButton"), "DropDownButton", "A button that display a flyout of choices when clicked."),
            new ButtonItemModel(LoadImage("HyperlinkButton"), "HyperlinkButton", "A button that appears as hyperlink text, and can navigate to a RUL or handle a Click event."),
            new ButtonItemModel(LoadImage("RadioButton"), "RadioButton", "A control that allows a user to select a single option from a group of options."),
            new ButtonItemModel(LoadImage("Slider"), "Slider", "A control that lets the user select from a range of values by moving a Thumb control along a track."),
            new ButtonItemModel(LoadImage("SplitButton"), "SplitButton", "A two-part button that displays a flyout when its secondary part is clicked."),
            new ButtonItemModel(LoadImage("ToggleSwitch"), "SwitchButton", "A switch that can be toggled between 2 states."),
            new ButtonItemModel(LoadImage("ToggleButton"), "ToggleButton", "A button that can be switched between two states like a CheckBox."),
        };

        DateTimeItemSource = new List<ButtonItemModel>()
        {
            new ButtonItemModel(LoadImage("CalendarDatePicker"), "CalendarDatePicker", "A control that lets a user pick a date value using a calendar."),
            new ButtonItemModel(LoadImage("DatePicker"), "DatePicker", "A control that lets a user pick a date value."),
            new ButtonItemModel(LoadImage("TimePicker"), "TimePicker", "A configurable control that lets a user pick a time value."),
        };

        DialogItemSource = new List<ButtonItemModel>()
        {
            new ButtonItemModel(LoadImage("Flyout"), "TaskDialog", "A task dialog."),
            new ButtonItemModel(LoadImage("Flyout"), "Flyout", "Shows contextual information and enables user interaction."),
            new ButtonItemModel(LoadImage("ContentDialog"), "ContentDialog", "A content dialog with mask."),
            new ButtonItemModel(LoadImage("TeachingTip"), "TeachingTip", "A content-rich flyout for guiding users and enabling teaching moments."),
        };

        LayoutItemSource = new List<ButtonItemModel>()
        {
            new ButtonItemModel(LoadImage("Border"), "Border", "Simple border layout"),
            new ButtonItemModel(LoadImage("Canvas"), "Canvas", "Can draw any shape canvas control"),
            new ButtonItemModel(LoadImage("SplitView"), "SplitView", "split view layout"),
            new ButtonItemModel(LoadImage("Grid"), "Grid", "A grid layout"),
            new ButtonItemModel(LoadImage("RelativePanel"), "RelativePanel", "Relative panel, control relative layout"),
            new ButtonItemModel(LoadImage("StackPanel"), "StackPanel", "A stackPanel layout"),
            new ButtonItemModel(LoadImage("Expander"), "Expander", "A expander layout"),
        };

        MenuAndToolBarItemSource = new List<ButtonItemModel>()
        {
            new ButtonItemModel(LoadImage("MenuFlyout"), "Menu", "Shows a contextual list of simple commands or options."),
            new ButtonItemModel(LoadImage("MenuBar"), "MenuBar", "Simple top menu bar"),
            new ButtonItemModel(LoadImage("CommandBar"), "CommandBar", "Display the command bar"),
            new ButtonItemModel(LoadImage("CommandBarFlyout"), "CommandBarFlyout", "A mini-toolbar displaying proactive commands, and an optional menu of commands."),
        };

        NavigationViewItemSource = new List<ButtonItemModel>()
        {
            new ButtonItemModel(LoadImage("NavigationView"), "NavigationView", "Navigation panel for page switching and menu navigation"),
            new ButtonItemModel(LoadImage("PageTransition"), "PageTransition", "Page switching control with animation"),
            // new ButtonItemModel(LoadImage("TabView"), "TabView", "A tabview view"),
            new ButtonItemModel(LoadImage("BreadcrumbBar"), "BreadcrumbBar", "Breadcrumb navigation view")
        };

        StatusAndInformationItemSource = new List<ButtonItemModel>()
        {
            new ButtonItemModel(LoadImage("ToolTip"), "ToolTip", "A control tooltip, hover show tooltip"),
            new ButtonItemModel(LoadImage("InfoBadge"), "InfoBadge", "Information badges can display a variety of information"),
            new ButtonItemModel(LoadImage("InfoBar"), "InfoBar", "Information bar can display a variety of information and can be closed"),
            new ButtonItemModel(LoadImage("ProgressBar"), "ProgressBar", "The progress bar has two states: confirmed and uncertain."),
            new ButtonItemModel(LoadImage("ProgressRing"), "ProgressRing", "A progress ring"),
        };

        TextItemSource = new List<ButtonItemModel>()
        {
            new ButtonItemModel(LoadImage("TextBlock"), "TextBlock", "Text block, used to display text"),
            new ButtonItemModel(LoadImage("TextBox"), "TextBox", "Text input box"),
            new ButtonItemModel(LoadImage("PasswordBox"), "PasswordBox", "Password input box, which can be turned on and off to display the password"),
            new ButtonItemModel(LoadImage("NumberBox"), "NumberBox", "Numeric input box that can be fine-tuned"),
        };

        ViewItemSource = new List<ButtonItemModel>()
        {
            new ButtonItemModel(LoadImage("ListBox"), "ListBox", "List box, can display multiple items"),
            new ButtonItemModel(LoadImage("TreeView"), "TreeView", "A tree view"),
            new ButtonItemModel(LoadImage("ScrollViewer"), "FlipView", "Carousel view, a control suitable for displaying multiple pictures")
        };
    }

    private Bitmap LoadImage(string name)
    {
        return new Bitmap(AssetLoader.Open(new Uri($"avares://Gallery/Assets/Controls/{name}.png")));
    }

    public event Action<string, string>? GotoControlEvent;

    [RelayCommand]
    private void Goto(object value)
    {
        if (value is Button button)
        {
            var name = button.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault(x => x.Name == "Title")?.Text;
            // Console.WriteLine(name);
            // Console.WriteLine(button.Tag); 
            GotoControlEvent?.Invoke(button.Tag?.ToString()!, name);

#if DEBUG
            Debug.WriteLine($"Goto => View: {button.Tag}, Name: {name}");
#endif
        }
        // GotoControlEvent?.Invoke(model.Page, name); //, control);
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
