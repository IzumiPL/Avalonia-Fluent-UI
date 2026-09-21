using Avalonia.Controls;
using Avalonia.Layout;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Toast-style info bar with solid-color severity background.
/// Visual template is defined in <c>ToastInfoBar.axaml</c>.
/// </summary>
public class ToastInfoBar : InfoBarBase
{
    protected override ColumnDefinitions GetColumnDefinitions()
    {
        return Orientation == Orientation.Horizontal ? new ColumnDefinitions("Auto, *, Auto") : new ColumnDefinitions("*, Auto");
    }

    protected override RowDefinitions GetRowDefinitions()
    {
        return Orientation == Orientation.Horizontal ? new RowDefinitions() : new RowDefinitions("Auto, *");
    }
}
