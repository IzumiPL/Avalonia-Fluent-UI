using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;

public class MultiSelectionComboBoxItem : ListBoxItem
{
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        e.Handled = true;
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        var point = e.GetPosition(this);
        if (new Rect(Bounds.Size).Contains(point))
        {
            IsSelected = !IsSelected;
            e.Handled = true;
        }
    }
}
