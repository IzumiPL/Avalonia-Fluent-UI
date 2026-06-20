using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;
using Gallery.Services;

namespace Gallery.ViewModels;

public partial class MenuAndToolBarViewModel : ViewModelBase
{
    public List<ButtonItemModel> MenuAndToolBarItemSource { get; }
    public override string Title => LocalizationService.Instance.GetString("MenuAndToolBar");
    
    public MenuAndToolBarViewModel()
    {
        MenuAndToolBarItemSource = ButtonItemModel.CreateList(
            ("MenuFlyout", "Menu", "ContextMenu", "Shows a contextual list of simple commands or options."),
            ("MenuBar", "MenuBar", "Menu", "Simple top menu bar"),
            ("CommandBar", "CommandBar", "CommandBar", "Display the command bar"),
            ("CommandBarFlyout", "CommandBarFlyout", "CommandBar", "A mini-toolbar displaying proactive commands, and an optional menu of commands.")
        );
    }
}
