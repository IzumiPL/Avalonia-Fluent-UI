using Avalonia.Controls;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;

public class MultiSelectionComboBoxItem : ListBoxItem
{
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.Handled && IsEffectivelyEnabled)
        {
            IsSelected = !IsSelected;
            e.Handled = true;
        }
    }
}
