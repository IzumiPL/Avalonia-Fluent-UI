using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class SelectionIndicator : Control 
{
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<SelectionIndicator>();

    public static readonly StyledProperty<IBrush?> IndicatorBrushProperty =
        AvaloniaProperty.Register<SelectionIndicator, IBrush?>(
            nameof(IndicatorBrush));

    public static readonly StyledProperty<double> IndicatorWidthProperty =
        AvaloniaProperty.Register<SelectionIndicator, double>(
            nameof(IndicatorWidth), 20);

    public static readonly StyledProperty<double> IndicatorHeightProperty =
        AvaloniaProperty.Register<SelectionIndicator, double>(
            nameof(IndicatorHeight), 3);

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        Border.CornerRadiusProperty.AddOwner<SelectionIndicator>();

    /// <summary>
    /// 设置或获取当前的背景色
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    /// <summary>
    /// 设置或获取当前底部指示器的背景色
    /// </summary>
    public IBrush? IndicatorBrush
    {
        get => GetValue(IndicatorBrushProperty);
        set => SetValue(IndicatorBrushProperty, value);
    }

    /// <summary>
    /// 设置或获取底部指示器的宽度
    /// </summary>
    public double IndicatorWidth
    {
        get => GetValue(IndicatorWidthProperty);
        set => SetValue(IndicatorWidthProperty, value);
    }

    /// <summary>
    /// 设置或获取底部指示器的高度
    /// </summary>
    public double IndicatorHeight
    {
        get => GetValue(IndicatorHeightProperty);
        set => SetValue(IndicatorHeightProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    static SelectionIndicator()
    {
        AffectsRender<SelectionIndicator>(BackgroundProperty, IndicatorBrushProperty, IndicatorWidthProperty, IndicatorHeightProperty, CornerRadiusProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        double width = Bounds.Width;
        double height = Bounds.Height;
        var rect = new Rect(0, 0, width, height);

        if (Background != null)
        {
            context.DrawRectangle(
                Background,
                null,
                new RoundedRect(rect, CornerRadius));
        }

        if (IndicatorBrush != null)
        {
            var x = (width - IndicatorWidth) / 2;
            var y = height - IndicatorHeight;

            context.DrawRectangle(
                IndicatorBrush,
                null,
                new RoundedRect(new Rect(x, y, IndicatorWidth, IndicatorHeight), CornerRadius));
        }
    }
}
