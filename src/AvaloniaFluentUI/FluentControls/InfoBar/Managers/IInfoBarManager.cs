namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Common interface for notification managers.
/// Concrete managers (ToastInfoBarManager, PopUpInfoBarManager, etc.)
/// provide additional positioning APIs specific to their notification type.
/// </summary>
public interface IInfoBarManager
{
    /// <summary>
    /// 消息条之间的间距
    /// </summary>
    double Spacing { get; set; }
    
    /// <summary>
    /// 消息条的外边距
    /// </summary>
    double Margin { get; set; }
    
    /// <summary>
    /// 消息条的最大宽度, 只有在<see cref="IsAutoResize"/>为<c>False</c>时才生效
    /// </summary>
    double InfoBarMaxWidth { get; }
    
    /// <summary>
    /// 是否自动改变大小
    /// </summary>
    bool IsAutoResize { get; }

    /// <summary>
    /// 设置消息条显示载体
    /// </summary>
    /// <param name="host"></param>
    void SetHost(InfoBarHost host);
    
    /// <summary>
    /// 更新所有消息条的位置
    /// </summary>
    void UpdateAllInfoBarPosition();
    
    /// <summary>
    /// 更新指定位置消息条的位置
    /// </summary>
    /// <param name="position"></param>
    void UpdateInfoBarPosition(InfoBarPosition position);
    
    /// <summary>
    /// 调整消息条的大小,大小为<see cref="InfoBarMaxWidth"/>的大小,只有在<see cref="IsAutoResize"/>为<c>True</c>才有效
    /// </summary>
    void AdjustedSize();
}
