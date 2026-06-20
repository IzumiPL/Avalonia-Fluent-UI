using System.ComponentModel;
using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class ContextMenuViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("ContextMenu");
}
