using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;

[PseudoClasses(PC_PRESSED)]
public class Card : SimpleCard
{
    private const string PC_PRESSED = ":pressed";

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        PseudoClasses.Add(PC_PRESSED);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        PseudoClasses.Remove(PC_PRESSED);
    }
}
