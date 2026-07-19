using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.Controls;

    
[PseudoClasses(PC_PRESSED)]
public class PickerButton : TemplatedControl
{
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<ColorPickerButton, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);
    
    public event EventHandler<RoutedEventArgs> Click
    {
        add { AddHandler(ClickEvent, value); }
        remove { RemoveHandler(ClickEvent, value); }
    } 
    
    private const string PC_PRESSED = ":pressed";

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        PseudoClasses.Set(PC_PRESSED, false);

        if (!IsEnabled)
            return;

        if (e.InitialPressMouseButton != MouseButton.Left)
            return;

        var point = e.GetPosition(this);
        if (new Rect(Bounds.Size).Contains(point))
        {
            OnClick();
        }
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        PseudoClasses.Set(PC_PRESSED, true);
    }

    protected virtual void OnClick()
    { 
        RaiseEvent(new RoutedEventArgs(ClickEvent));
    }
}
