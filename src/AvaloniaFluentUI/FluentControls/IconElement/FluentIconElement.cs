using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// A unified icon control that automatically detects the icon type from the <see cref="Source"/> property.
/// Supports <see cref="Symbol"/>, <see cref="IconSource"/>, <see cref="IImage"/>, path data strings,
/// URI strings, glyph strings, and <see cref="IconElement"/> instances.
/// </summary>
public class FluentIconElement : IconElement
{
    private static readonly IconElementConverter s_converter = new();

    /// <summary>
    /// Defines the <see cref="Source"/> property.
    /// </summary>
    public static readonly StyledProperty<object> SourceProperty =
        AvaloniaProperty.Register<FluentIconElement, object>(nameof(Source));

    /// <summary>
    /// Gets or sets the icon source. The icon type is automatically detected based on the value:
    /// <list type="bullet">
    ///   <item><see cref="Symbol"/> enum → <see cref="SymbolIcon"/></item>
    ///   <item><see cref="IconSource"/> → corresponding <see cref="IconElement"/> via <see cref="IconHelpers"/></item>
    ///   <item><see cref="IImage"/> → <see cref="ImageIcon"/></item>
    ///   <item><see cref="IconElement"/> → used directly</item>
    ///   <item><see cref="string"/> → auto-detect: Symbol name, Path data, URI, or glyph</item>
    /// </list>
    /// </summary>
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            OnSourceChanged(change);
            InvalidateMeasure();
        }
        else if (change.Property == ForegroundProperty)
        {
            if (_child is IconElement icon)
            {
                icon.Foreground = Foreground;
            }
        }
    }

    private void OnSourceChanged(AvaloniaPropertyChangedEventArgs args)
    {
        DetachChild();

        var value = args.NewValue;
        if (value == null)
            return;

        IconElement icon = null;

        // Already an IconElement — use directly
        if (value is IconElement existing)
        {
            icon = existing;
        }
        // Geometry — create FluentPathIcon directly
        else if (value is Geometry geometry)
        {
            icon = new FluentPathIcon { Data = geometry };
        }
        // Use the existing converter for all other types
        else if (s_converter.CanConvertFrom(value.GetType()))
        {
            icon = s_converter.ConvertFrom(value) as IconElement;
        }

        if (icon != null)
        {
            AttachChild(icon);

            // Ensure the child inherits Foreground after being added to the visual tree
            if (IsSet(ForegroundProperty))
            {
                icon.Foreground = Foreground;
            }
            // else if (Foreground != null)
            // {
                // icon.Foreground = Foreground;
            // }
        }
    }

    private void AttachChild(IconElement child)
    {
        _child = child;
        ((ISetLogicalParent)_child).SetParent(this);
        VisualChildren.Add(_child);
        LogicalChildren.Add(_child);

        // Bind child's Foreground to this control's Foreground so property inheritance works
        // even when Foreground is resolved later from the theme/visual tree
        // _child.Bind(ForegroundProperty, this.GetObservable(ForegroundProperty));
    }

    private void DetachChild()
    {
        if (_child != null)
        {
            ((ISetLogicalParent)_child).SetParent(null);
            LogicalChildren.Clear();
            VisualChildren.Remove(_child);
            _child = null;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return LayoutHelper.MeasureChild(_child, availableSize, new Thickness());
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return LayoutHelper.ArrangeChild(_child, finalSize, new Thickness());
    }

    private Control _child;
}
