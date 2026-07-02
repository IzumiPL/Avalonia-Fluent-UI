using System.Collections.Generic;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Common interface for notification managers.
/// Concrete managers (ToastInfoBarManager, PopUpInfoBarManager, etc.)
/// provide additional positioning APIs specific to their notification type.
/// </summary>
public interface IInfoBarManager
{
    void SetHost(InfoBarHost host);
    void UpdateAllInfoBarPosition();
    void UpdateInfoBarPosition(int value);
    void AdjustedSize();
}
