using AvaloniaFluentUI.Controls;

namespace Gallery.Services;

public static class InfoBarService
{
    public static PopupInfoBarManager PopupInfoBarManager { get; set; } = null!;
    public static ToastInfoBarManager ToastInfoBarManager { get; set; } = null!;
}
