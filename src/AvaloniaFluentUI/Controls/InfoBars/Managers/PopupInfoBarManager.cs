using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Manages the lifecycle and positioning of <see cref="PopupInfoBar"/> popups.
/// </summary>
public class PopupInfoBarManager : InfoBarManagerBase<PopupInfoBar>
{
    /// <summary>
    /// 创建并弹出自定义等级的消息条
    /// </summary>
    /// <param name="title">消息条的标题</param>
    /// <param name="content">消息条的内容,可放置任意控件</param>
    /// <param name="position">消息条要弹出的位置</param>
    /// <param name="severity">消息条的等级</param>
    /// <param name="isClosable">消息条是否可手动关闭</param>
    /// <param name="duration">消息条的弹出持续时间</param>
    /// <param name="orientation">消息条内容的布局方向</param>
    public void New(string title, object content, InfoBarPosition position,
        InfoBarSeverity severity, bool isClosable, int duration, Orientation orientation)
    {
        Add(new PopupInfoBar()
        {
            // MaxWidth = IsAutoResize ? InfoBarMaxWidth : double.PositiveInfinity,
            MaxWidth =  InfoBarMaxWidth,
            Title = title,
            Content = content,
            Position = position,
            Severity = severity,
            Duration = duration,
            IsClosable = isClosable,
            Orientation = orientation
        });
    }

    /// <summary>
    /// 创建并弹出成功消息条
    /// </summary>
    /// <param name="title">消息条的标题</param>
    /// <param name="content">消息条的内容,可放置任意控件</param>
    /// <param name="position">消息条要弹出的位置</param>
    /// <param name="isClosable">消息条是否可手动关闭</param>
    /// <param name="duration">消息条的弹出持续时间</param>
    /// <param name="orientation">消息条内容的布局方向</param>
    public void Success(string title, object content, InfoBarPosition position = InfoBarPosition.TopRight, 
        bool isClosable = false, int duration = 3000, Orientation orientation = Orientation.Vertical)
        => New(title, content, position, InfoBarSeverity.Success, isClosable, duration, orientation);

    /// <summary>
    /// 创建并弹出普通消息条
    /// </summary>
    /// <param name="title">消息条的标题</param>
    /// <param name="content">消息条的内容,可放置任意控件</param>
    /// <param name="position">消息条要弹出的位置</param>
    /// <param name="isClosable">消息条是否可手动关闭</param>
    /// <param name="duration">消息条的弹出持续时间</param>
    /// <param name="orientation">消息条内容的布局方向</param>
    public void Information(string title, object content, InfoBarPosition position = InfoBarPosition.TopRight,
        bool isClosable = false, int duration = 3000, Orientation  orientation = Orientation.Vertical)
        => New(title, content, position, InfoBarSeverity.Informational, isClosable, duration, orientation);

    /// <summary>
    /// 创建并弹出警告消息条
    /// </summary>
    /// <param name="title">消息条的标题</param>
    /// <param name="content">消息条的内容,可放置任意控件</param>
    /// <param name="position">消息条要弹出的位置</param>
    /// <param name="isClosable">消息条是否可手动关闭</param>
    /// <param name="duration">消息条的弹出持续时间</param>
    /// <param name="orientation">消息条内容的布局方向</param>
    public void Warning(string title, object content, InfoBarPosition position = InfoBarPosition.TopRight, 
        bool isClosable = false, int duration = 3000,  Orientation orientation = Orientation.Vertical)
        => New(title, content, position, InfoBarSeverity.Warning, isClosable, duration, orientation);

    /// <summary>
    /// 创建并弹出错误消息条
    /// </summary>
    /// <param name="title">消息条的标题</param>
    /// <param name="content">消息条的内容,可放置任意控件</param>
    /// <param name="position">消息条要弹出的位置</param>
    /// <param name="isClosable">消息条是否可手动关闭</param>
    /// <param name="duration">消息条的弹出持续时间</param>
    /// <param name="orientation">消息条内容的布局方向</param>
    public void Error(string title, object content, InfoBarPosition position = InfoBarPosition.TopRight,
        bool isClosable = true, int duration = -1,  Orientation orientation = Orientation.Vertical)
        => New(title, content, position, InfoBarSeverity.Error, isClosable, duration, orientation);

    /// <summary>
    /// 传播关键并弹出自定义前景,背景的消息条
    /// </summary>
    /// <param name="title">消息条的标题</param>
    /// <param name="content">消息条的内容,可放置任意控件</param>
    /// <param name="position">消息条要弹出的位置</param>
    /// <param name="isClosable">消息条是否可手动关闭</param>
    /// <param name="duration">消息条的弹出持续时间</param>
    /// <param name="background">消息条的背景色</param>
    /// <param name="foreground">消息条的前景色</param>
    /// <param name="orientation">消息条内容的布局方向</param>
    public void Custom(string title, object content, InfoBarPosition position, bool isClosable,
        int duration, IBrush background, IBrush foreground, Orientation orientation = Orientation.Vertical)
    {
        Add(new PopupInfoBar
        {
            Title = title,
            Content = content,
            Position = position,
            Duration = duration,
            IsClosable = isClosable,
            Background = background,
            Foreground = foreground,
            Orientation = orientation,
            Severity = InfoBarSeverity.Custom
        });
    }
}
