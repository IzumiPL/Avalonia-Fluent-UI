using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Controls.Primitives;
using AvaloniaFluentUI.Styling;

namespace AvaloniaFluentUI.Windowing;

/// <summary>
/// Custom Window that supports a modern Windows look and title bar customization,
/// with a graceful fallback for MacOS and Linux
/// </summary>
public partial class AppWindow : Window
{
    /// <summary>
    /// Defines the <see cref="Icon"/> property
    /// </summary>
    public static readonly new StyledProperty<IImage> IconProperty =
        AvaloniaProperty.Register<AppWindow, IImage>(nameof(Icon));

    /// <summary>
    /// Defines the AllowInteractionInTitleBar attached property
    /// </summary>
    public static readonly AttachedProperty<bool> AllowInteractionInTitleBarProperty =
        AvaloniaProperty.RegisterAttached<AppWindow, Control, bool>("AllowInteractionInTitleBar");

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<AppWindow, double>(nameof(IconSize), defaultValue: 16);

    public static readonly StyledProperty<bool> FullScreenButtonIsVisibleProperty =
        AvaloniaProperty.Register<AppWindow, bool>(nameof(FullScreenButtonIsVisible));

    public bool FullScreenButtonIsVisible
    {
        get => GetValue(FullScreenButtonIsVisibleProperty);
        set => SetValue(FullScreenButtonIsVisibleProperty, value);
    } 

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    /// <summary>
    /// Gets the value of the <see cref="AllowInteractionInTitleBarProperty"/> attached property for the given control
    /// </summary>
    public static bool GetAllowInteractionInTitleBar(Control c) => c.GetValue(AllowInteractionInTitleBarProperty);

    /// <summary>
    /// Sets the value of the <see cref="AllowInteractionInTitleBarProperty"/> attached property for the given control
    /// </summary>
    /// <param name="c"></param>
    /// <param name="b"></param>
    public static void SetAllowInteractionInTitleBar(Control c, bool b) => c.SetValue(AllowInteractionInTitleBarProperty, b);

    /// <summary>
    /// Gets or sets the icon for the window
    /// </summary>
    /// <remarks>
    /// Note that this type is <see cref="IImage"/> and not <see cref="WindowIcon"/>, like on Window
    /// This is done to allow using a window icon in managed titlebar. Provided the
    /// image is an <see cref="IBitmap"/>, it should convert to a WindowIcon without 
    /// issue and you'll still get the icon in the taskbar, on other OS's, etc.
    /// </remarks>
    public new IImage Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets a value whether the AppWindow should hide its minimize/maximize buttons like 
    /// a dialog window. This property is only respected on Windows.
    /// </summary>
    public bool ShowAsDialog
    {
        get => _hideSizeButtons;
        set
        {
            _hideSizeButtons = value;
            PseudoClasses.Set(":dialog", value);
        }
    }

    /// <summary>
    /// Gets or sets the splash screen that should show when the window first loads
    /// </summary>
    public IApplicationSplashScreen SplashScreen
    {
        get => _splashContext?.SplashScreen;
        set
        {
            if (value == null)
            {
                if (_splashContext != null)
                {
                    _splashContext.Host.SplashScreen = null;
                }

                _splashContext = null;
                PseudoClasses.Set(":splashScreen", false);
            }
            else
            {
                _splashContext = new SplashScreenContext(value);
                PseudoClasses.Set(":splashScreen", true);
            }
        }
    }

    /// <summary>
    /// Gets the Titlebar description information for the AppWindow
    /// </summary>
    /// <remarks>
    /// Use this property to customize the colors, height, and whether the window contents should
    /// display in the titlebar area
    /// </remarks>
    public AppWindowTitleBar TitleBar => _titleBar;

    /// <summary>
    /// Gets the interface for custom platform-specific features through the AppWindow class
    /// NOTE: Only implemented on Windows right now
    /// </summary>
    public IAppWindowPlatformFeatures PlatformFeatures { get; private set; }

    protected internal bool IsWindows11 { get; internal set; }

    protected internal bool IsWindows { get; internal set; }

    protected override Type StyleKeyOverride => typeof(AppWindow);

    private SplashScreenContext _splashContext;
    private Grid _defaultTitleBar;
    private AppWindowTitleBar _titleBar;
    private bool _hideSizeButtons;

    // Resource names used in SetTitleBarColors

    private const string TITLE_BAR_BACKGROUND = "FluentTitleBarBackground";
    private const string TITLE_BAR_FOREGROUND = "FluentTitleBarForeground";
    private const string TITLE_BAR_INACTIVE_BACKGROUND = "FluentTitleBarBackgroundInactive";
    private const string TITLE_BAR_INACTIVE_FOREGROUND = "FluentTitleBarForegroundInactive";
    private const string SYSTEM_CAPTION_BACKGROUND = "FATitle_SysCaptionBackground";
    private const string SYSTEM_CAPTION_FOREGROUND = "FluentSysCaptionForeground";
    private const string SYSTEM_CAPTION_BACKGROUND_HOVER = "FluentSysCaptionBackgroundHover";
    private const string SYSTEM_CAPTION_FOREGROUND_HOVER = "FluentSysCaptionForegroundHover";
    private const string SYSTEM_CAPTION_BACKGROUND_PRESSED = "FluentSysCaptionBackgroundPressed";
    private const string SYSTEM_CAPTION_FOREGROUND_PRESSED = "FluentSysCaptionForegroundPressed";
    private const string SYSTEM_CAPTION_BACKGROUND_INACTIVE = "FluentSysCaptionBackgroundInactive";
    private const string SYSTEM_CAPTION_FOREGROUND_INACTIVE = "FluentSysCaptionForegroundInactive";
    
    /// <summary>
    /// Defines the <see cref="TitleBarHeight"/> property
    /// </summary>
    public static readonly StyledProperty<double> TitleBarHeightProperty =
        AvaloniaProperty.Register<AppWindow, double>(nameof(TitleBarHeight));

    /// <summary>
    /// Defines the <see cref="IsTitleBarContentVisible"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsTitleBarContentVisibleProperty =
        AvaloniaProperty.Register<AppWindow, bool>(nameof(IsTitleBarContentVisible), defaultValue: true);

    /// <summary>
    /// Defines the <see cref="WindowIcon"/> property
    /// </summary>
    public static readonly StyledProperty<IImage> WindowIconProperty =
        AvaloniaProperty.Register<AppWindow, IImage>(nameof(WindowIcon));

    public static readonly StyledProperty<object?> TitleBarContentProperty =
        AvaloniaProperty.Register<AppWindow, object?>(nameof(TitleBarContent));
    
    public static readonly StyledProperty<IDataTemplate?> TitleBarContentTemplateProperty =
        AvaloniaProperty.Register<AppWindow, IDataTemplate?>(nameof(TitleBarContentTemplate));

    public static readonly StyledProperty<Thickness> TitleBarContentMarginProperty =
        AvaloniaProperty.Register<AppWindow, Thickness>(nameof(TitleBarContentMargin), new Thickness(8, 0, 140, 0));

    public Thickness TitleBarContentMargin
    {
        get => GetValue(TitleBarContentMarginProperty);
        set => SetValue(TitleBarContentMarginProperty, value);
    }
    
    public IDataTemplate? TitleBarContentTemplate
    {
        get => GetValue(TitleBarContentTemplateProperty);
        set => SetValue(TitleBarContentTemplateProperty, value);
    }
    
    public object? TitleBarContent
    {
        get => GetValue(TitleBarContentProperty);
        set => SetValue(TitleBarContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the managed titlebar for AppWindow
    /// </summary>
    public double TitleBarHeight
    {
        get => GetValue(TitleBarHeightProperty);
        set => SetValue(TitleBarHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the titlebar content is visible (Icon and App name text)
    /// </summary>
    public bool IsTitleBarContentVisible
    {
        get => GetValue(IsTitleBarContentVisibleProperty);
        set => SetValue(IsTitleBarContentVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon used in the managed titlebar of AppWindow
    /// </summary>
    public IImage WindowIcon
    {
        get => GetValue(WindowIconProperty);
        set => SetValue(WindowIconProperty, value);
    }
    
    public AppWindow()
    {
        _titleBar = new AppWindowTitleBar(this);
        PseudoClasses.Add(":noFullScreen");

        if (OperatingSystem.IsWindows() && !Design.IsDesignMode)
        {
            InitializeAppWindow();
        }

        PointerPressed += OnWindowPointerPressed;
    }

    private void OnWindowPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (!IsWindows || _defaultTitleBar == null)
            return;

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var point = e.GetPosition(_defaultTitleBar);
            if (HitTestTitleBar(point))
            {
                if (e.ClickCount == 2)
                {
                    WindowState = WindowState == WindowState.Maximized
                        ? WindowState.Normal
                        : WindowState.Maximized;
                    e.Handled = true;
                }
                else
                {
                    BeginMoveDrag(e);
                }
            }
        }
    }

    static AppWindow()
    {
        if (OperatingSystem.IsWindows())
            ExtendClientAreaToDecorationsHintProperty.OverrideDefaultValue<AppWindow>(true);
    }

    public void EnabledAcrylicBlue(bool enable)
    {
        if (enable)
        {
            Background = Brush.Parse(FluentAvaloniaTheme.Instance.IsDarkTheme ? "#30161616" : "#30F3F3F3");  
            TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
            return;
        } 
        ResetBackground();
    }

    public void EnabledMica(bool enable)
    {
        if (enable && IsWindows11)
        {
            Background = Brushes.Transparent;
            TransparencyLevelHint = [WindowTransparencyLevel.Mica];
            return;
        }
        ResetBackground();
    }

    private void ResetBackground()
    {
        TransparencyLevelHint = [];
        Background = Brush.Parse(FluentAvaloniaTheme.Instance.IsDarkTheme ? "#202020" : "#F3F3F3"); 
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);      

        if (IsWindows && !Design.IsDesignMode)
        {
            _defaultTitleBar = e.NameScope.Find<Grid>("DefaultTitleBar");

            // This will set all our TemplateSettings properties
            OnTitleBarHeightChanged(_titleBar.Height);

            SetTitleBarColors();
        }

        if (SplashScreen != null)
        {
            var host = e.NameScope.Find<AppSplashScreen>("SplashHost");
            if (host != null)
            {
                _splashContext.Host = host;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == ExtendClientAreaToDecorationsHintProperty)
        {
            if (IsWindows)
            {
                throw new InvalidOperationException("AppWindow cannot be customized with ExtendClientAreaToDecorationsHintProperty." +
                    "Use the TitleBar property or a regular Avalonia window");
            }
        }
        else if (change.Property == IconProperty)
        {
            base.Icon = new WindowIcon(change.NewValue as Bitmap);
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, change.NewValue != null);
        }
        else if (change.Property == ActualThemeVariantProperty)
        {
            SetTitleBarColors();
        }

        if (change.Property == FullScreenButtonIsVisibleProperty)
        {
            TitleBarContentMargin = change.GetNewValue<bool>() ?  new Thickness(8, 0, 185, 0) : new Thickness(8, 0, 140, 0);
        }
    }

    protected override async void OnOpened(EventArgs e)
    {
        if (_splashContext != null && !_splashContext.HasShownSplashScreen && !Design.IsDesignMode)
        {
            PseudoClasses.Set(":splashOpen", true);
            var time = DateTime.Now;

            // n00b async/await mistake - need to await here, thansk to GH taj-ny for finding and fixing this
            await _splashContext.RunJobs();

            var delta = DateTime.Now - time;
            if (delta.TotalMilliseconds < _splashContext.SplashScreen.MinimumShowTime)
            {
                await Task.Delay(Math.Max(1, _splashContext.SplashScreen.MinimumShowTime - (int)delta.TotalMilliseconds));
            }

            LoadApp();
        }

        base.OnOpened(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        _splashContext?.TryCancel();

        base.OnClosed(e);     
    }

    internal void OnTitleBarHeightChanged(double height)
    {
        TitleBarHeight = height;
    }

    internal void TitleBarColorsChanged()
    {
        SetTitleBarColors();
    }

    internal bool HitTestTitleBar(Point p)
    {
        if (_defaultTitleBar == null)
            return false;

        if (p.Y < _titleBar.Height)
        {
            if (!ComplexHitTest(p))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    internal bool ComplexHitTest(Point p)
    {
        var result = this.InputHitTest(p) as InputElement;

        // Special case for TabViewListView during drag operations where blank space 
        // is inserted and causes HitTest to fail (since nothing focusable is there)
        if (result is Visual v && v.TemplatedParent is TabViewListView)
            return false;

        if (result == _defaultTitleBar)
            return true;

        while (result != null)
        {
            if (result.IsHitTestVisible && result.Focusable)
                return false;

            result = result.GetVisualParent() as InputElement;
        }

        return true;
    }

    private void SetTitleBarColors()
    {
        if (_titleBar == null)
            return;

        SetResource(TITLE_BAR_BACKGROUND, _titleBar.BackgroundColor);
        SetResource(TITLE_BAR_FOREGROUND, _titleBar.ForegroundColor);

        SetResource(TITLE_BAR_INACTIVE_BACKGROUND, _titleBar.InactiveBackgroundColor);
        SetResource(TITLE_BAR_INACTIVE_FOREGROUND, _titleBar.InactiveForegroundColor);

        SetResource(SYSTEM_CAPTION_BACKGROUND, _titleBar.ButtonBackgroundColor);
        SetResource(SYSTEM_CAPTION_FOREGROUND, _titleBar.ButtonForegroundColor);

        SetResource(SYSTEM_CAPTION_BACKGROUND_HOVER, _titleBar.ButtonHoverBackgroundColor);
        SetResource(SYSTEM_CAPTION_FOREGROUND_HOVER, _titleBar.ButtonHoverForegroundColor);

        SetResource(SYSTEM_CAPTION_BACKGROUND_PRESSED, _titleBar.ButtonPressedBackgroundColor);
        SetResource(SYSTEM_CAPTION_FOREGROUND_PRESSED, _titleBar.ButtonPressedForegroundColor);

        SetResource(SYSTEM_CAPTION_BACKGROUND_INACTIVE, _titleBar.ButtonInactiveBackgroundColor);
        SetResource(SYSTEM_CAPTION_FOREGROUND_INACTIVE, _titleBar.ButtonInactiveForegroundColor);

        void SetResource(string name, Color? color)
        {
            if (color.HasValue)
            {
                Resources[name] = color;
            }
            else
            {
                Resources.Remove(name);
            }
        }
    }

    internal void OnShowFullScreenButtonChanged(bool value)
    {
        PseudoClasses.Set(":noFullScreen", value);
    }
    
    private async void LoadApp()
    {
        if (Presenter is not ContentPresenter cp)
            return;

        cp.IsVisible = true;

        // Taking this out, it's causing flickering of the content after the splash fade animation
        // Another regression in the animation system for 11.0...
        //using var disp = cp.SetValue(OpacityProperty, 0d, Avalonia.Data.BindingPriority.Animation);

        var aniSplash = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(250),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters =
                    {
                        new Setter(OpacityProperty, 1d)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(OpacityProperty, 0d),
                    },
                    KeySpline = new KeySpline(0,0,0,1)
                }
            }
        };

        var aniCP = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(167),
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters =
                    {
                        new Setter(OpacityProperty, 0d)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(OpacityProperty, 1d),
                    },
                    KeySpline = new KeySpline(0,0,0,1)
                }
            }
        };

        await Task.WhenAll(aniSplash.RunAsync(_splashContext.Host),
            aniCP.RunAsync((Animatable)Presenter));

        PseudoClasses.Set(":splashOpen", false);
        _splashContext.HasShownSplashScreen = true;
    }
}
