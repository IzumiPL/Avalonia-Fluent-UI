using System;
using Avalonia;

namespace AvaloniaFluentUI.Controls;

public enum ToastSeverity
{
    Informational,
    Success,
    Warning,
    Error,
    Custom
}

public enum ToastInfoBarPosition
{
    TopLeft,
    Top,
    TopRight,
    BottomLeft,
    Bottom,
    BottomRight
}

/// <summary>
/// Toast-style info bar with solid-color severity background.
/// Visual template is defined in <c>ToastInfoBar.axaml</c>.
/// </summary>
public class ToastInfoBar : InfoBarBase
{
    public static readonly StyledProperty<ToastSeverity> SeverityProperty =
        AvaloniaProperty.Register<ToastInfoBar, ToastSeverity>(nameof(Severity));

    public static readonly StyledProperty<ToastInfoBarPosition> PositionsProperty =
        AvaloniaProperty.Register<ToastInfoBar, ToastInfoBarPosition>(nameof(Positions));

    public ToastInfoBarPosition Positions
    {
        get => GetValue(PositionsProperty);
        set => SetValue(PositionsProperty, value);
    }

    public ToastSeverity Severity
    {
        get => GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    public override int PositionValue => (int)Positions;
}
