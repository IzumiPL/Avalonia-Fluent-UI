using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaFluentUI.Windowing;

public class FluentTitleBar : ContentControl 
{
    /// <summary>
    ///     Defines the <see cref="IconSize" /> property.
    /// </summary>
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<FluentTitleBar, double>(nameof(IconSize), 16);

    /// <summary>
    ///     Defines the <see cref="Title" /> property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<FluentTitleBar, string?>(nameof(Title), string.Empty);

    /// <summary>
    ///     Defines the <see cref="Icon" /> property.
    /// </summary>
    public static readonly StyledProperty<IImage?> IconProperty =
        AvaloniaProperty.Register<FluentTitleBar, IImage?>(nameof(Icon));

    /// <summary>
    ///     Defines the <see cref="ContentIsVisible" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ContentIsVisibleProperty =
        AvaloniaProperty.Register<FluentTitleBar, bool>(nameof(ContentIsVisible), true);

    /// <summary>
    ///     Defines the <see cref="ContentMargin" /> property.
    /// </summary>
    public static readonly StyledProperty<Thickness> ContentMarginProperty =
        AvaloniaProperty.Register<FluentTitleBar, Thickness>(nameof(ContentMargin),  new Thickness(8, 0, 0, 0));

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

    public bool TitleIsVisible
    {
        get => GetValue(TitleIsVisibleProperty);
        set => SetValue(TitleIsVisibleProperty, value);
    }

    public bool IconIsVisible
    {
        get => GetValue(IconIsVisibleProperty);
        set => SetValue(IconIsVisibleProperty, value);
    }

    public Thickness ContentMargin
    {
        get => GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }
    
    public bool ContentIsVisible
    {
        get => GetValue(ContentIsVisibleProperty);
        set => SetValue(ContentIsVisibleProperty, value);
    }

    public IImage? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
}
