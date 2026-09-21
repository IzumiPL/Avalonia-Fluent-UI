using Avalonia;

namespace AvaloniaFluentUI.Windowing;

public class FluentTitleBarTemplateSettings : AvaloniaObject
{
    /// <summary>
    ///     Defines the <see cref="IconSize" /> property.
    /// </summary>
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<FluentTitleBarTemplateSettings, double>(nameof(IconSize), 16);

    /// <summary>
    ///     Defines the <see cref="ContentIsVisible" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ContentIsVisibleProperty =
        AvaloniaProperty.Register<FluentTitleBarTemplateSettings, bool>(nameof(ContentIsVisible), true);

    /// <summary>
    ///     Defines the <see cref="Margin" /> property.
    /// </summary>
    public static readonly StyledProperty<Thickness> MarginProperty =
        AvaloniaProperty.Register<FluentTitleBarTemplateSettings, Thickness>(nameof(Margin), new Thickness(0, 0, 135.99, 0));

    /// <summary>
    ///     Defines the <see cref="IsVisible" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<FluentTitleBarTemplateSettings, bool>(nameof(IsVisible), true);

    /// <summary>
    ///     Defines the <see cref="Height" /> property.
    /// </summary>
    public static readonly StyledProperty<double> HeightProperty =
        AvaloniaProperty.Register<FluentTitleBarTemplateSettings, double>(nameof(Height), 45);

    /// <summary>
    ///     Defines the <see cref="ContentMargin" /> property.
    /// </summary>
    public static readonly StyledProperty<Thickness> ContentMarginProperty =
        AvaloniaProperty.Register<FluentTitleBarTemplateSettings, Thickness>(nameof(ContentMargin), new Thickness(8, 0, 0, 0));
    
    /// <summary>
    ///     Defines the <see cref="IconIsVisible" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IconIsVisibleProperty =
        AvaloniaProperty.Register<FluentTitleBar, bool>(nameof(IconIsVisible), true);

    /// <summary>
    ///     Defines the <see cref="TitleIsVisible" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> TitleIsVisibleProperty =
        AvaloniaProperty.Register<FluentTitleBar, bool>(nameof(TitleIsVisible), true);

    /// <summary>
    /// 获取或设置标题栏是否可见
    /// </summary>
    public bool TitleIsVisible
    {
        get => GetValue(TitleIsVisibleProperty);
        set => SetValue(TitleIsVisibleProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏图标是否可见
    /// </summary>
    public bool IconIsVisible
    {
        get => GetValue(IconIsVisibleProperty);
        set => SetValue(IconIsVisibleProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏内容边距
    /// </summary>
    public Thickness ContentMargin
    {
        get => GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }
    
    /// <summary>
    /// 获取或设置标题栏高度
    /// </summary>
    public double Height
    {
        get => GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏是否可见
    /// </summary>
    public bool IsVisible
    {
        get => GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏边距
    /// </summary>
    public Thickness Margin
    {
        get => GetValue(MarginProperty);
        set => SetValue(MarginProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏内容是否可见
    /// </summary>
    public bool ContentIsVisible
    {
        get => GetValue(ContentIsVisibleProperty);
        set => SetValue(ContentIsVisibleProperty, value);
    }

    /// <summary>
    /// 获取或设置标题栏图标大小
    /// </summary>
    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
}
