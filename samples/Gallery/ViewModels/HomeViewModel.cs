using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;
using Gallery.Services;

namespace Gallery.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    public override string Title => "Fluent Gallery";
    
    [ObservableProperty]
    private Vector _scrollViewerOffset =  new Vector();

    public Vector Vector => ScrollViewerOffset;
    
    public HomeViewModel()
    {
#if DEBUG
        Debug.WriteLine("HomeViewModel Init");
#endif
        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;

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

        DateTimeItemSource = ButtonItemModel.CreateList(
            ("CalendarDatePicker", "CalendarDatePicker", "DateTime", "A control that lets a user pick a date value using a calendar."),
            ("DatePicker", "DatePicker", "DateTime", "A control that lets a user pick a date value."),
            ("TimePicker", "TimePicker", "DateTime", "A configurable control that lets a user pick a time value.")
        );

        DialogItemSource = ButtonItemModel.CreateList(
            ("Flyout", "TaskDialog", "Dialog", "A task dialog."),
            ("Flyout", "Flyout", "Flyout", "Shows contextual information and enables user interaction."),
            ("ContentDialog", "ContentDialog", "Dialog", "A content dialog with mask."),
            ("TeachingTip", "TeachingTip", "Flyout", "A content-rich flyout for guiding users and enabling teaching moments.")
        );

        LayoutItemSource = ButtonItemModel.CreateList(
            ("Border", "Border", "Border", "Simple border layout"),
            ("Canvas", "Canvas", "Border", "Can draw any shape canvas control"),
            ("SplitView", "SplitView","", "split view layout"),
            ("Grid", "Grid", "Panel", "A grid layout"),
            ("RelativePanel", "RelativePanel", "Panel", "Relative panel, control relative layout"),
            ("StackPanel", "StackPanel", "Panel", "A stackPanel layout"),
            ("Expander", "Expander", "Panel", "A expander layout")
        );

        MenuAndToolBarItemSource = ButtonItemModel.CreateList(
            ("MenuFlyout", "Menu", "ContextMenu", "Shows a contextual list of simple commands or options."),
            ("MenuBar", "MenuBar", "Menu", "Simple top menu bar"),
            ("CommandBar", "CommandBar", "CommandBar", "Display the command bar"),
            ("CommandBarFlyout", "CommandBarFlyout", "CommandBar", "A mini-toolbar displaying proactive commands, and an optional menu of commands.")
        );

        NavigationViewItemSource = ButtonItemModel.CreateList(
            ("NavigationView", "NavigationView", "NavigationView", "Navigation panel for page switching and menu navigation"),
            ("BreadcrumbBar", "BreadcrumbBar", "BreadcrumbBar", "Breadcrumb navigation view"),
            ("Pivot", "Segmented", "SegmentedView", "This is the segmented navigation bar")
        );

        StatusAndInformationItemSource = ButtonItemModel.CreateList(
            ("ToolTip", "ToolTip", "StatusAndInformation",  "A control tooltip, hover show tooltip"),
            ("InfoBadge", "InfoBadge", "StatusAndInformation", "Information badges can display a variety of information"),
            ("InfoBar", "InfoBar", "StatusAndInformation", "Information bar can display a variety of information and can be closed"),
            ("ProgressBar", "ProgressBar", "StatusAndInformation", "The progress bar has two states: confirmed and uncertain."),
            ("ProgressRing", "ProgressRing", "StatusAndInformation", "A progress ring")
        );

        TextItemSource = ButtonItemModel.CreateList(
            ("TextBlock", "TextBlock", "TextBlock", "Text block, used to display text"),
            ("TextBox", "TextBox", "TextBox", "Text input box"),
            ("PasswordBox", "PasswordBox", "TextBox", "Password input box, which can be turned on and off to display the password"),
            ("NumberBox", "NumberBox", "NumberBox", "Numeric input box that can be fine-tuned")
        );

        ViewItemSource = ButtonItemModel.CreateList(
            ("FlipView", "FlipView", "CarouselView", "Carousel view, a control suitable for displaying multiple pictures"),
            ("PageTransition", "PageTransition", "CarouselView", "Page switching control with animation"),
            ("ListBox", "ListBox", "List", "List box, can display multiple items"),
            ("TreeView", "TreeView", "TreeView", "A tree view")
        );
    }

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

    public List<ButtonItemModel> ButtonItemSource { get; }
    public List<ButtonItemModel> DateTimeItemSource { get; }
    public List<ButtonItemModel> DialogItemSource { get; }
    public List<ButtonItemModel> LayoutItemSource { get; }
    public List<ButtonItemModel> MenuAndToolBarItemSource { get; }
    public List<ButtonItemModel> NavigationViewItemSource { get; }
    public List<ButtonItemModel> StatusAndInformationItemSource { get; }
    public List<ButtonItemModel> TextItemSource { get; }
    public List<ButtonItemModel> ViewItemSource { get; }

    // Localized string properties
    public string GettingStartedTitle => LocalizationService.Instance.GetString("GettingStarted");
    public string GettingStartedContent => LocalizationService.Instance.GetString("GettingStartedContent");
    public string GitHubRepoTitle => LocalizationService.Instance.GetString("GitHubRepo");
    public string GitHubRepoContent => LocalizationService.Instance.GetString("GitHubRepoContent");
    public string CodeSamplesTitle => LocalizationService.Instance.GetString("CodeSamples");
    public string CodeSamplesContent => LocalizationService.Instance.GetString("CodeSamplesContent");
    public string SendFeedbackTitle => LocalizationService.Instance.GetString("SendFeedback");
    public string SendFeedbackContent => LocalizationService.Instance.GetString("SendFeedbackContent");
    public string SectionBasicInput => LocalizationService.Instance.GetString("Section_BasicInput");
    public string SectionDateTime => LocalizationService.Instance.GetString("Section_DateTime");
    public string SectionDialog => LocalizationService.Instance.GetString("Section_Dialog");
    public string SectionLayout => LocalizationService.Instance.GetString("Section_Layout");
    public string SectionMenuAndToolBar => LocalizationService.Instance.GetString("Section_MenuAndToolBar");
    public string SectionNavigationView => LocalizationService.Instance.GetString("Section_NavigationView");
    public string SectionStatusAndInformation => LocalizationService.Instance.GetString("Section_StatusAndInformation");
    public string SectionText => LocalizationService.Instance.GetString("Section_Text");
    public string SectionView => LocalizationService.Instance.GetString("Section_View");
    
    private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(GettingStartedTitle));
        OnPropertyChanged(nameof(GettingStartedContent));
        OnPropertyChanged(nameof(GitHubRepoTitle));
        OnPropertyChanged(nameof(GitHubRepoContent));
        OnPropertyChanged(nameof(CodeSamplesTitle));
        OnPropertyChanged(nameof(CodeSamplesContent));
        OnPropertyChanged(nameof(SendFeedbackTitle));
        OnPropertyChanged(nameof(SendFeedbackContent));
        OnPropertyChanged(nameof(SectionBasicInput));
        OnPropertyChanged(nameof(SectionDateTime));
        OnPropertyChanged(nameof(SectionDialog));
        OnPropertyChanged(nameof(SectionLayout));
        OnPropertyChanged(nameof(SectionMenuAndToolBar));
        OnPropertyChanged(nameof(SectionNavigationView));
        OnPropertyChanged(nameof(SectionStatusAndInformation));
        OnPropertyChanged(nameof(SectionText));
        OnPropertyChanged(nameof(SectionView));
    }
}
