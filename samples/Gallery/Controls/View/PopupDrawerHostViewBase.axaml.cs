using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using AvaloniaFluentUI.Controls;

namespace Gallery.Controls;

public class PopupDrawerHostViewBase : ViewBase 
{
    /// <summary>
    ///     Defines the <see cref="PopupDrawerContent" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> PopupDrawerContentProperty =
        AvaloniaProperty.Register<PopupDrawerHostViewBase, object?>(nameof(PopupDrawerContent));

    public object? PopupDrawerContent
    {
        get => GetValue(PopupDrawerContentProperty);
        set => SetValue(PopupDrawerContentProperty, value);
    }
    
    public PopupDrawerHostViewBase() { }
    
    public PopupDrawerHostViewBase(string page) : base(page) { }

    internal PopupDrawer PopupDrawer { get; private set; } = null!;
    
    protected override Type StyleKeyOverride => typeof(PopupDrawerHostViewBase);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        PopupDrawer = e.NameScope.Find<PopupDrawer>("PopupDrawer")!;
    }
}

