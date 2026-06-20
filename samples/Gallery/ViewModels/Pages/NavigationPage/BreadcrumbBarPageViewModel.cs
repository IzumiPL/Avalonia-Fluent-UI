using System.ComponentModel;
using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class BreadcrumbBarPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("BreadcrumbBar");
    
    public string[] BreadcrumbBarItemSource => @"C:\Users\Administrator\OneDrive\Pictures\Camera Roll".Split("\\");
}
