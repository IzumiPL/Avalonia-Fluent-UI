using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AvaloniaFluentUI.Controls;

public class TimeLineItem : ContentControl 
{
    /// <summary>
    ///     Defines the <see cref="Header" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<TimeLineItem, object?>(nameof(Header));

    /// <summary>
    ///     Defines the <see cref="HeaderTemplate" /> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<TimeLineItem, IDataTemplate?>(nameof(HeaderTemplate));

    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
    
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}
