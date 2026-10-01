using System.Collections.Generic;
using AvaloniaFluentUI.Locale;
using Gallery.Models;

namespace Gallery.ViewModels;

public partial class WindowingViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("Window");
    
    public List<ButtonItemModel> WindowingViewItemSource { get; }

    public WindowingViewModel()
    {
        WindowingViewItemSource = ButtonItemModel.CreateList(
            ("FluentWindow", "FluentWindow", "FluentWindow", "A flexible, customizable window management system for app development."),
            ("TitleBar", "FluentTitleBar", "TitleBar", "An example showing how to use the default TitleBar control.")
        );
    }
}
