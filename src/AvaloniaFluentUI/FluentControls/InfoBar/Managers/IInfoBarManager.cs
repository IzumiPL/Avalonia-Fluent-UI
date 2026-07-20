namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Common interface for notification managers.
/// Concrete managers (ToastInfoBarManager, PopUpInfoBarManager, etc.)
/// provide additional positioning APIs specific to their notification type.
/// </summary>
public interface IInfoBarManager
{
    double Spacing { get; set; }
    double Margin { get; set; }
    double InfoBarMaxWidth { get; }

    void SetHost(InfoBarHost host);
    
    void UpdateAllInfoBarPosition();
    void UpdateInfoBarPosition(InfoBarPosition position);
    void AdjustedSize();
}
