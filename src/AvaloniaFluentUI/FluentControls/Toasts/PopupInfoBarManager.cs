using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Manages the lifecycle and positioning of <see cref="PopupInfoBar"/> popups.
/// </summary>
public class PopupInfoBarManager : InfoBarManagerBase<PopupInfoBar>
{
    public void New(string title, object content, PopupInfoBarPosition position,
        PopupInfoBarSeverity severity, bool isClosable, int duration)
    {
        Add(new PopupInfoBar() 
        {
            MaxWidth = HostMaxWidth,
            Title = title,
            Content = content,
            Positions = position,
            Severity = severity,
            Duration = duration,
            IsClosable = isClosable
        });
    }

    public void Success(string title, object content,
        PopupInfoBarPosition position, bool isClosable = false, int duration = 3000)
        => New(title, content, position, PopupInfoBarSeverity.Success, isClosable, duration);

    public void Information(string title, object content,
        PopupInfoBarPosition position, bool isClosable = false, int duration = 3000)
        => New(title, content, position, PopupInfoBarSeverity.Informational, isClosable, duration);

    public void Warning(string title, object content,
        PopupInfoBarPosition position, bool isClosable = false, int duration = 3000)
        => New(title, content, position, PopupInfoBarSeverity.Warning, isClosable, duration);

    public void Error(string title, object content,
        PopupInfoBarPosition position, bool isClosable = true, int duration = -1)
        => New(title, content, position, PopupInfoBarSeverity.Error, isClosable, duration);

    public void Custom(string title, object content, PopupInfoBarPosition position,
        bool isClosable, int duration, IBrush background, IBrush foreground)
    {
        Add(new PopupInfoBar
        {
            Title = title,
            Content = content,
            Positions = position,
            Duration = duration,
            IsClosable = isClosable,
            Background = background,
            Foreground = foreground,
            Severity = PopupInfoBarSeverity.Custom
        });
    }
}
