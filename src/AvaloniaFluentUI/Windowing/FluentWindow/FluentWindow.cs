using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
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
[TemplatePart(Name = PART_DEFAULT_TITLE_BAR, Type =  typeof(Grid))]
[TemplatePart(Name = PART_MINIMIZE_BUTTON, Type = typeof(Button))]
[TemplatePart(Name = PART_MAXIMIZE_BUTTON, Type = typeof(Button))]
[TemplatePart(Name = PART_CLOSE_BUTTON, Type = typeof(Button))]
public partial class FluentWindow : Window
{
    /// <summary>
    /// Defines the <see cref="Icon"/> property
    /// </summary>
    public static readonly new StyledProperty<IImage> IconProperty =
        AvaloniaProperty.Register<FluentWindow, IImage>(nameof(Icon));

    /// <summary>
    /// Defines the AllowInteractionInTitleBar attached property
    /// </summary>
    public static readonly AttachedProperty<bool> AllowInteractionInTitleBarProperty =
        AvaloniaProperty.RegisterAttached<FluentWindow, Control, bool>("AllowInteractionInTitleBar");

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<FluentWindow, double>(nameof(IconSize), defaultValue: 16);

    public static readonly StyledProperty<bool> FullScreenButtonIsVisibleProperty =
        AvaloniaProperty.Register<FluentWindow, bool>(nameof(FullScreenButtonIsVisible));

    public static readonly StyledProperty<Thickness> TitleBarMarginProperty =
        AvaloniaProperty.Register<FluentWindow, Thickness>(nameof(TitleBarMargin));

    public static readonly StyledProperty<bool> MinButtonIsVisibleProperty =
        AvaloniaProperty.Register<FluentWindow, bool>(nameof(MinButtonIsVisible), defaultValue: true);

    public static readonly StyledProperty<bool> MaxButtonIsVisibleProperty =
        AvaloniaProperty.Register<FluentWindow, bool>(nameof(MaxButtonIsVisible), defaultValue: true);

    public static readonly StyledProperty<bool> CloseButtonIsVisibleProperty =
        AvaloniaProperty.Register<FluentWindow, bool>(nameof(CloseButtonIsVisible), defaultValue: true);
    
    /// <summary>
    /// Defines the <see cref="TitleBarHeight"/> property
    /// </summary>
    public static readonly StyledProperty<double> TitleBarHeightProperty =
        AvaloniaProperty.Register<FluentWindow, double>(nameof(TitleBarHeight));

    /// <summary>
    /// Defines the <see cref="TitleBarContentIsVisibleProperty"/> property
    /// </summary>
    public static readonly StyledProperty<bool> TitleBarContentIsVisibleProperty =
        AvaloniaProperty.Register<FluentWindow, bool>(nameof(TitleBarContentIsVisible), defaultValue: true);

    public static readonly StyledProperty<object?> TitleBarContentProperty =
        AvaloniaProperty.Register<FluentWindow, object?>(nameof(TitleBarContent));
    
    public static readonly StyledProperty<IDataTemplate?> TitleBarContentTemplateProperty =
        AvaloniaProperty.Register<FluentWindow, IDataTemplate?>(nameof(TitleBarContentTemplate));

    public static readonly StyledProperty<Thickness> TitleBarContentMarginProperty =
        AvaloniaProperty.Register<FluentWindow, Thickness>(nameof(TitleBarContentMargin), new Thickness(8, 0, 140, 0));

    public static readonly StyledProperty<bool> TitleBarIsVisibleProperty =
        AvaloniaProperty.Register<FluentWindow, bool>(nameof(TitleBarIsVisible), defaultValue: true);

    public bool TitleBarIsVisible
    {
        get => GetValue(TitleBarIsVisibleProperty);
        set => SetValue(TitleBarIsVisibleProperty, value);
    }

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
    public bool TitleBarContentIsVisible
    {
        get => GetValue(TitleBarContentIsVisibleProperty);
        set => SetValue(TitleBarContentIsVisibleProperty, value);
    }

    public bool CloseButtonIsVisible
    {
        get => GetValue(CloseButtonIsVisibleProperty);
        set => SetValue(CloseButtonIsVisibleProperty, value);
    }
    
    public bool MaxButtonIsVisible
    {
        get => GetValue(MaxButtonIsVisibleProperty);
        set => SetValue(MaxButtonIsVisibleProperty, value);
    }

    public bool MinButtonIsVisible
    {
        get => GetValue(MinButtonIsVisibleProperty);
        set => SetValue(MinButtonIsVisibleProperty, value);
    }

    public Thickness TitleBarMargin
    {
        get => GetValue(TitleBarMarginProperty);
        set => SetValue(TitleBarMarginProperty, value);
    }

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
    public FluentWindowTitleBar TitleBar => _titleBar;

    /// <summary>
    /// Gets the interface for custom platform-specific features through the AppWindow class
    /// NOTE: Only implemented on Windows right now
    /// </summary>
    public IAppWindowPlatformFeatures PlatformFeatures { get; private set; }

    protected internal bool IsWindows11 { get; internal set; }

    protected internal bool IsWindows { get; internal set; }

    protected internal bool IsLinux { get; internal set; }

    protected override Type StyleKeyOverride => typeof(FluentWindow);
    
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

    private SplashScreenContext _splashContext;
    private Grid _defaultTitleBar;
    private FluentWindowTitleBar _titleBar;
    private Border _windowBorder;
    private bool _hideSizeButtons;
    
    private Button? _minimizeButton;
    private Button? _maximizeButton;
    private Button? _closeButton;
    
    private const string PART_DEFAULT_TITLE_BAR = "PART_DefaultTitleBar";
    private const string SPLASH_HOST = "SplashHost";
    
    private const string PART_MINIMIZE_BUTTON = "PART_MinimizeButton";
    private const string PART_MAXIMIZE_BUTTON = "PART_MaximizeButton";
    private const string PART_CLOSE_BUTTON = "PART_CloseButton";

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
    
    public FluentWindow()
    {
        _titleBar = new FluentWindowTitleBar(this);
        PseudoClasses.Add(":noFullScreen");

        if (OperatingSystem.IsWindows() && !Design.IsDesignMode)
        {
            InitializeWindowPlatform();
        }
        else if (OperatingSystem.IsLinux() && !Design.IsDesignMode)
        {
            InitializeLinuxPlatform();
        }

        PointerPressed += OnWindowPointerPressed;
        
        if (!IsWindows11)
        {
            PseudoClasses.Add(":isNotWin11");
        } 
    }

    private void OnWindowPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (_defaultTitleBar == null)
            return;

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var point = e.GetPosition(_defaultTitleBar);
            if (HitTestTitleBar(point))
            {
                if (CanMaximize && CanResize && e.ClickCount == 2)
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

    public void EnabledAcrylicBlue(bool enable)
    {
        if (enable)
        {
            Background = Brush.Parse(AvaloniaFluentTheme.Instance.IsDarkTheme ? "#30161616" : "#30F3F3F3");  
            TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
            return;
        } 
        ResetBackground();
    }

    public void EnabledMica(bool enable)
    {
        if (IsWindows11 && enable)
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
        Background = Brush.Parse(AvaloniaFluentTheme.Instance.IsDarkTheme ? "#202020" : "#F3F3F3"); 
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);      
        _minimizeButton?.Click -= OnMinimizeButtonClicked;
        _maximizeButton?.Click -= OnMaximizeButtonClicked;
        _closeButton?.Click -= OnCloseButtonClicked;
        
        _minimizeButton = e.NameScope.Find<Button>(PART_MINIMIZE_BUTTON);
        _maximizeButton = e.NameScope.Find<Button>(PART_MAXIMIZE_BUTTON);
        _closeButton = e.NameScope.Find<Button>(PART_CLOSE_BUTTON);
        
        _minimizeButton?.Click += OnMinimizeButtonClicked;
        _maximizeButton?.Click += OnMaximizeButtonClicked;
        _closeButton?.Click += OnCloseButtonClicked;

        if (!Design.IsDesignMode)
        {
            _defaultTitleBar = e.NameScope.Find<Grid>(PART_DEFAULT_TITLE_BAR);

            OnTitleBarHeightChanged(_titleBar.Height);

            SetTitleBarColors();
        }

        _windowBorder = e.NameScope.Find<Border>("RootBorder");

        if (SplashScreen != null)
        {
            var host = e.NameScope.Find<AppSplashScreen>(SPLASH_HOST);
            if (host != null)
            {
                _splashContext.Host = host;
            }
        }
    }
    
    private void OnCloseButtonClicked(object? sender, RoutedEventArgs e) => Close();

    private void OnMaximizeButtonClicked(object? sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void OnMinimizeButtonClicked(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == ExtendClientAreaToDecorationsHintProperty)
        {
            // if (IsWindows)
            // {
                // throw new InvalidOperationException("AppWindow cannot be customized with ExtendClientAreaToDecorationsHintProperty." +
                    // "Use the TitleBar property or a regular Avalonia window");
            // }
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

        if (change.Property == CanResizeProperty)
        {
            _maximizeButton?.IsEnabled = change.GetNewValue<bool>();
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

        // TODO: 当前样式未启用
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
