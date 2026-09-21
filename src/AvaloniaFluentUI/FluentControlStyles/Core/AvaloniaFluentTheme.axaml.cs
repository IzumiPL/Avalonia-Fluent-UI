using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Media;

namespace AvaloniaFluentUI.Styling;

/// <summary>
/// Theme manager for AvaloniaFluentUI, managing various components of the Fluentv2 theme
/// like AccentColor, styles, and platform settings
/// </summary>
public partial class AvaloniaFluentTheme : Styles, IResourceProvider
{
    /// <summary>
    /// Gets the current <see cref="AvaloniaFluentTheme"/> instance.
    /// </summary>
    public static AvaloniaFluentTheme Instance { get; private set; } = null!;

    /// <summary>
    /// Create new instance of <see cref="AvaloniaFluentTheme"/>.
    /// </summary>
    public AvaloniaFluentTheme()
    {
        Instance = this;
        MergedDictionaries = new AvaloniaList<IResourceDictionary>();
        MergedDictionaries.CollectionChanged += MergedDictionariesCollectionChanged;
        Init();

        Application.Current?.PropertyChanged += OnCurrentThemePropertyChanged;
    }

    private void OnCurrentThemePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(Application.ActualThemeVariant))
        {
            ThemeChanged?.Invoke(sender, e.GetNewValue<ThemeVariant>());
        }
    }

    /// <summary>
    /// 获取或设置是否使用当前系统的主题（浅色或深色模式）。
    /// </summary>
    /// <remarks>
    /// 此属性在 Windows、macOS 和 Linux 上均受支持。
    /// 但是，在 Linux 上，主题检测方式会根据用户使用的桌面环境而有所不同。
    /// 在 KDE、Cinnamon、LXDE 和 LXQt 上，需要用户的主题名称（KDE 中为配色方案名称）包含“dark”。
    /// 在 GNOME 或 Xfce 上，需要将“color-scheme”设置为“prefer-light”或“prefer-dark”，
    /// 或者 GTK 主题名称包含“dark”。
    /// 另外需要注意，高对比度主题仅支持在 Windows 上进行检测。
    /// </remarks>
    public bool PreferSystemTheme
    {
        get => _preferSystemTheme;
        set
        {
            if (_preferSystemTheme != value)
            {
                _preferSystemTheme = value;

                // Only call this if PreferSystemTheme is true to invalidate the current theme.
                if (value)
                {
                    ResolveThemeAndInitializeSystemResources();
                }
            }
        }
    }

    /// <summary>
    /// 获取或设置是否使用当前用户的系统强调色
    /// 优先级高于<see cref="AccentColor"/>, 为<c>True</c>则<c>AccentColor</c>不生效
    /// </summary>
    /// <remarks>
    /// 在 Linux 上，仅支持 KDE、LXQt 和 LXDE 的强调色检测。
    /// KDE 支持从当前配色方案、壁纸和自定义设置中获取；
    /// LXQt 从选择颜色中获取；LXDE 从自定义选择颜色中获取。
    /// </remarks>
    public bool PreferUserAccentColor
    {
        get => _preferUserAccentColor;
        set
        { 
            if(_preferUserAccentColor != value)
            {
                _preferUserAccentColor = value;

                // Unlike PreferSystemTheme, we call this everytime as LoadAccentColor handles
                // switching between a system and custom color (and back)
                LoadAccentColor();
            }            
        }
    }

    /// <summary>
    /// 获取或设置应用程序使用的强调色，并将其作为 SystemAccentColor。
    /// 获取时返回当前正在使用的强调色。
    /// 当 <see cref="PreferUserAccentColor"/> 为 <c>False</c> 时，设置此属性可以自定义强调色。
    /// </summary>
    /// <remarks>
    /// 当 <see cref="PreferUserAccentColor"/> 为 <c>True</c> 时，该属性具有更高优先级，
    /// 始终使用当前用户的系统强调色，此时设置 AccentColor 不会生效。
    /// 当 <see cref="PreferUserAccentColor"/> 为 <c>False</c> 时，使用此属性设置的自定义强调色。
    /// 指定的强调色会自动生成 6 个变体（3 个浅色变体和 3 个深色变体）。
    /// AvaloniaFluentUI 不会检查所选颜色是否具有良好的可读性或是否符合无障碍要求，
    /// 这些方面由使用者自行负责。
    /// 如需更精细地控制强调色变体，可以直接在应用程序级资源字典中覆盖
    /// SystemAccentColor 或其对应的变体。
    /// </remarks>
    public Color AccentColor
    {
        get => (_accentColorsDictionary != null && _accentColorsDictionary.TryGetValue("SystemAccentColor", out var value))
            ? (Color)value 
            : Colors.DeepSkyBlue;
        set
        {
            if (_accentColor != value)
            {
                _accentColor = value;
                if (_hasLoaded)
                {
                    LoadAccentColor();
                }
            }
        }
    }

    /// <summary>
    /// 设置当前应用程序的语言
    /// </summary>
    public string Locale
    {
        set { LocalizationService.Instance.SetCulture(value); }
    }

    /// <summary>
    /// Gets or sets a value that determines if/when style overrides should be used to alleviate issues
    /// with text alignment in some controls caused when Segoe UI or Segoe UI Variable font
    /// families do not exist. The default value is <see cref="TextVerticalAlignmentOverride.EnabledNonWindows"/>
    /// </summary>
    /// <remarks>
    /// These overrides apply to controls like RadioButton, CheckBox, ComboBox where the first line of text
    /// is explicitly aligned with the control. Adding the overrides modify the styles to use VerticalAlignment=Center
    /// to get a consistent experience, at the (small) expense of breaking Fluent design principles. If your controls
    /// never use multi-line text, you'll never see the effect of this property.
    /// </remarks>
    public TextVerticalAlignmentOverride TextVerticalAlignmentOverrideBehavior { get; set; } =
        TextVerticalAlignmentOverride.EnabledNonWindows;

    /// <summary>
    /// 主题更改事件
    /// </summary>
    public event EventHandler<ThemeVariant>? ThemeChanged;

    /// <summary>
    /// 主题色更改事件 
    /// </summary>
    public event EventHandler<Color>? AccentColorChanged;
    
    /// <summary>
    /// 获取当前主题色是否为<c>Dark</c> 
    /// </summary>
    public bool IsDarkTheme => Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

    /// <summary>
    /// 获取或设置当前应用程序的主题。
    /// </summary>
    public ThemeVariant CurrentTheme
    {
        get => Application.Current?.RequestedThemeVariant ?? ThemeVariant.Default;
        set => Application.Current?.RequestedThemeVariant = value;
    }

    /// <summary>
    /// 切换当前程序主题
    /// </summary>
    public void ToggleTheme()
    {
        Application.Current?.RequestedThemeVariant = IsDarkTheme ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    public AvaloniaList<IResourceDictionary> MergedDictionaries { get; }
      
    bool IResourceNode.HasResources => true;

    public new bool TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        // Github build failing with this not being set, even tho it passes locally
        value = null;

        // We also search the app level resources so resources can be overridden.
        // Do not search App level styles though as we'll have to iterate over them
        // to skip the FluentAvaloniaTheme instance or we'll stack overflow
        if (Application.Current?.Resources.TryGetResource(key, theme, out value) == true)
            return true;

        if (base.TryGetResource(key, theme, out value))
            return true;

        value = null;
        return false;
    }

    bool IResourceNode.TryGetResource(object key, ThemeVariant? theme, out object? value) =>
        this.TryGetResource(key, theme, out value);

    private void Init()
    {
        AvaloniaXamlLoader.Load(this);

        // First load our base and theme resources

        // When initializing, UseSystemTheme overrides any setting of RequestedTheme, this must be
        // explicitly disabled to enable setting the theme manually
        ResolveThemeAndInitializeSystemResources();

        SetTextAlignmentOverrides();
        
        _hasLoaded = true;
    }

    private void ResolveThemeAndInitializeSystemResources()
    {
        ThemeVariant? theme = null;

        // PlatformSettings on the Application should be immutable so we can store them here
        if (_platformSettings == null)
        {
            _platformSettings = Application.Current?.PlatformSettings;
            _platformSettings?.ColorValuesChanged += OnPlatformColorValuesChanged;
        }
                        
        if (OperatingSystem.IsWindows())
        {
            theme = ResolveWindowsSystemSettings(_platformSettings);
        }
        else if (OperatingSystem.IsLinux())
        {
            theme = ResolveLinuxSystemSettings(_platformSettings);
        }
        else if (OperatingSystem.IsMacOS())
        {
            theme = ResolveMacOSSystemSettings(_platformSettings);
        }
        else
        {
            // WASM & Mobile

            // Don't read from PlatformSettings if PreferSystemTheme = false, Issue #497
            if (PreferSystemTheme)
                theme = GetThemeFromIPlatformSettings(_platformSettings);

            // MacOS logic is also used for WASM/Mobile since it just pulls from
            // IPlatformSettings Color Values
            TryLoadMacOSAccentColor(_platformSettings);
        }

        // The Resolve...Settings will return null if PreferSystemTheme is false
        if (theme != null)
        {
            Application.Current?.RequestedThemeVariant = theme;
            ThemeChanged?.Invoke(this, theme);
        }
    }

    private void OnPlatformColorValuesChanged(object? sender, PlatformColorValues e)
    {
        if (PreferSystemTheme)
        {
            ThemeVariant theme;
            if (e.ContrastPreference == ColorContrastPreference.High)
            {
                theme = e.ThemeVariant == PlatformThemeVariant.Light ?
                    ThemeVariant.Light : ThemeVariant.Dark;
            }
            else
            {
                theme = e.ThemeVariant == PlatformThemeVariant.Light ?
                    ThemeVariant.Light : ThemeVariant.Dark;
            }

            Application.Current?.RequestedThemeVariant = theme;
            ThemeChanged?.Invoke(this, theme);
        }

        if (PreferUserAccentColor)
        {
            if (OperatingSystem.IsWindows())
            {
                TryLoadWindowsAccentColor();
            }
            else if (OperatingSystem.IsMacOS())
            {
                TryLoadMacOSAccentColor(_platformSettings);
            }
            else if (OperatingSystem.IsLinux())
            {
                TryLoadLinuxAccentColor();
            }
        }
    }

    private ThemeVariant? ResolveMacOSSystemSettings(IPlatformSettings? platformSettings)
    {
        ThemeVariant? theme = null;
        if (PreferSystemTheme)
        {
            theme = GetThemeFromIPlatformSettings(platformSettings);
        }

        LoadAccentColor();

        return theme;
    }

    private ThemeVariant? ResolveLinuxSystemSettings(IPlatformSettings? platformSettings)
    {
        ThemeVariant? theme = null;
        if (PreferSystemTheme)
        {
            // See TryLoadLinuxAccentColor() for note on what Avalonia IPlatformSettings supports
            // on Linux. We'll try the existing logic first before attempting IPlatformSettings
            var resolvedTheme = LinuxThemeResolver.TryLoadSystemTheme();
            theme = resolvedTheme != null ? resolvedTheme : GetThemeFromIPlatformSettings(platformSettings);
        }

        LoadAccentColor();

        return theme;
    }

    private ThemeVariant GetThemeFromIPlatformSettings(IPlatformSettings? platformSettings)
    {
        if (platformSettings == null) { return ThemeVariant.Default; }
        
        var platformColors = platformSettings.GetColorValues();
        bool isSystemInHighContrast = platformColors.ContrastPreference == ColorContrastPreference.High;
        if (!isSystemInHighContrast)
        {
           return platformColors.ThemeVariant == PlatformThemeVariant.Light ?
                ThemeVariant.Light : ThemeVariant.Dark;
        }
        else
        {
            return platformColors.ThemeVariant == PlatformThemeVariant.Light ?
                ThemeVariant.Light : ThemeVariant.Dark;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetTextAlignmentOverrides()
    {
        if (TextVerticalAlignmentOverrideBehavior == TextVerticalAlignmentOverride.Disabled ||
            (TextVerticalAlignmentOverrideBehavior == TextVerticalAlignmentOverride.EnabledNonWindows &&
            OperatingSystem.IsWindows()))
            return;

        // The following resources are added to remove the larger bottom margin/padding value
        // on some controls added to accomodate Segoe UI - this will allow vertical centering
        // These are added to the internal _themeResources dictionary, so user can still
        // override these elsewhere if desired

        Resources.Add("CheckBoxPadding", new Thickness(8, 5, 0, 5));
        Resources.Add("ComboBoxPadding", new Thickness(12, 5, 0, 5));
        Resources.Add("ComboBoxItemThemePadding", new Thickness(11, 5, 11, 5));
        // Note that this is a theme resource, but as of now is the same for all three themes
        Resources.Add("TextControlThemePadding", new Thickness(10, 5, 6, 5));

        // Now we add some style overrides to adjust some properties
        // Yes, I'm doing this in C# rather than Xaml - I don't want to create a Xaml file
        // because that will get compiled into AvaloniaXamlResource even if never used or I
        // could use a normal file and us the AvaloniaXamlLoader but that's still an additional
        // AvaloniaResource that's not necessary. Plus, not using Xaml is fun =D

        // Set VerticalContentAlignment on CheckBox to center the content
        var s = new Style(x => x.OfType<CheckBox>());
        s.Setters.Add(new Setter(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Center));
        Add(s);

        // Set Padding & VCA on RadioButton to center the content
        var s2 = new Style(x => x.OfType<RadioButton>());
        s2.Setters.Add(new Setter(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Center));
        s2.Setters.Add(new Setter(Decorator.PaddingProperty, new Thickness(8, 6, 0, 6)));
        Add(s2);

        // Center the TextBlock in ComboBox
        // This is special - we only want to do this if the content is a string - otherwise custom content
        // may get messed up b/c of the centered alignment
        var s3 = new Style(x => x.OfType<ComboBox>().Template().OfType<ContentControl>().Child().OfType<TextBlock>());
        s3.Setters.Add(new Setter(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center));
        Add(s3);
    }
       
    private void LoadAccentColor()
    {
        // PreferUserAccentColor takes priority: when true, always use the system user's
        // accent color and ignore any custom color
        if (PreferUserAccentColor)
        {
            if (OperatingSystem.IsWindows())
            {
                TryLoadWindowsAccentColor();
            }
            else if (OperatingSystem.IsLinux())
            {
                TryLoadLinuxAccentColor();
            }
            else // Mac & WASM/Mobile
            {
                TryLoadMacOSAccentColor(_platformSettings);
            }

            return;
        }

        if (!_accentColor.HasValue)
        {
            LoadDefaultAccentColor();
            return;
        }

        Color2 col = _accentColor.Value;

        UpdateAccentColors((Color)col,
            (Color)col.LightenPercent(0.05f),
            (Color)col.LightenPercent(0.10f),
            (Color)col.LightenPercent(0.15f),
            (Color)col.LightenPercent(-0.05f),
            (Color)col.LightenPercent(-0.10f),
            (Color)col.LightenPercent(-0.15f));
    }
        
    private void TryLoadMacOSAccentColor(IPlatformSettings? platformSettings)
    {
        if (platformSettings == null) { return; }
        
        try
        {
            // Replaced old logic with PlatformSettings from Avalonia
            Color2 aColor = platformSettings.GetColorValues().AccentColor1;

            UpdateAccentColors((Color)aColor,
                (Color)aColor.LightenPercent(0.05f),
                (Color)aColor.LightenPercent(0.10f),
                (Color)aColor.LightenPercent(0.15f),
                (Color)aColor.LightenPercent(-0.05f),
                (Color)aColor.LightenPercent(-0.10f),
                (Color)aColor.LightenPercent(-0.15f));
        }
        catch
        {
            LoadDefaultAccentColor();
        }
    }

    private void TryLoadLinuxAccentColor()
    {
        // Per GH#9913:
        // Only works if distro implements newest (~2021) standard of FreeDesktop. GTK and others specific settings are ignored.
        // Accent colors are not supported, and frame theme isn't changeable from the app (not sure if it's possible, if anybody wants to help - please do).
        // No high contrast support.
        // So we'll keep the existing logic here

        var aColor = LinuxThemeResolver.TryLoadAccentColor();
        if (aColor != null)
        {
            Color2 col = aColor.Value;

            UpdateAccentColors((Color)col,
                (Color)col.LightenPercent(0.05f),
                (Color)col.LightenPercent(0.10f),
                (Color)col.LightenPercent(0.15f),
                (Color)col.LightenPercent(-0.05f),
                (Color)col.LightenPercent(-0.10f),
                (Color)col.LightenPercent(-0.15f));
        }
        else
        {
            LoadDefaultAccentColor();
        }
    }

    private void LoadDefaultAccentColor()
    {
        UpdateAccentColors(Colors.DeepSkyBlue,
            Color.Parse("#0DC2FF"),
            Color.Parse("#1AC5FF"),
            Color.Parse("#26C9FF"),
            Color.Parse("#00B4F2"),
            Color.Parse("#00A9E5"),
            Color.Parse("#009ED8"));
    }

    /// <summary>
    /// 将指定资源添加到资源字典中；如果资源已存在，则更新其值。
    /// </summary>
    /// <param name="key">资源键。</param>
    /// <param name="value">资源值。</param>
    private void AddOrUpdateSystemResource(object key, object value)
    {
        Resources[key] = value;
    }

    private void UpdateAccentColors(Color accent,
        Color light1, Color light2, Color light3,
        Color dark1, Color dark2, Color dark3)
    {
        if (_accentColorsDictionary != null)
            Resources.MergedDictionaries.Remove(_accentColorsDictionary);

        _accentColorsDictionary = new ResourceDictionary
        {
            { "SystemAccentColor", accent },
            { "SystemAccentColorLight1", light1 },
            { "SystemAccentColorLight2", light2 },
            { "SystemAccentColorLight3", light3 },
            { "SystemAccentColorDark1", dark1 },
            { "SystemAccentColorDark2", dark2 },
            { "SystemAccentColorDark3", dark3 }
        };

        Resources.MergedDictionaries.Add(_accentColorsDictionary);
        AccentColorChanged?.Invoke(this, accent);
    }

    private void MergedDictionariesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (IResourceDictionary item in e.OldItems)
            {
                Resources.MergedDictionaries.Remove(item);
            }
        }

        if (e.NewItems != null)
        {
            foreach (IResourceDictionary item in e.NewItems)
            {
                Resources.MergedDictionaries.Add(item);
            }
        }
    }

    private bool _hasLoaded;
    private Color? _accentColor;
    private bool _preferSystemTheme;
    private bool _preferUserAccentColor;
    private ResourceDictionary? _accentColorsDictionary;
    private IPlatformSettings? _platformSettings;

    public const string Light = "Light";
    public const string Dark = "Dark";
}
