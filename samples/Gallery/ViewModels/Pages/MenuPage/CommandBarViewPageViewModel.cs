using System.ComponentModel;
using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class CommandBarViewPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("CommandBar");

}
