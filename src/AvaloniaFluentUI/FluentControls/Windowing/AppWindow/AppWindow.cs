using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
using AvaloniaFluentUI.Media;

namespace AvaloniaFluentUI.Controls.Windowing;

/// <summary>
/// Custom Window that supports a modern Windows look and title bar customization,
/// with a graceful fallback for MacOS and Linux
/// </summary>
public partial class AppWindow : Window
{
    /// <summary>
    /// Defines the <see cref="TemplateSettings"/> property
    /// </summary>
    // public static readonly StyledProperty<AppWindow> TemplateSettingsProperty =
        // AvaloniaProperty.Register<AppWindow, AppWindow>(nameof(TemplateSettings));

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
    /// Provides calculated data for items within the Template of AppWindow
    /// </summary>
    // public AppWindow TemplateSettings
    // {
        // get => GetValue(TemplateSettingsProperty);
        // private set => SetValue(TemplateSettingsProperty, value);
    // }

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

    internal MinMaxCloseControl SystemCaptionControl => _captionButtons;


    private SplashScreenContext _splashContext;
    private Border _templateRoot;
    private MinMaxCloseControl _captionButtons;
    private Panel _defaultTitleBar;
    private AppWindowTitleBar _titleBar;
    private List<WeakReference<Control>> _excludeHitTestList;
    private bool _hideSizeButtons;

    // Resource names used in SetTitleBarColors
    private const string SYSTEM_ACCENT_COLOR = "SystemAccentColor";
    private const string SYSTEM_ACCENT_COLOR_LIGHT_1 = "SystemAccentColorLight1";
    private const string SYSTEM_ACCENT_COLOR_DARK_1 = "SystemAccentColorDark1";
    private const string TEXT_FILL_COLOR_PRIMARY = "TextFillColorPrimary";

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
        AvaloniaProperty.Register<AppWindow, double>(nameof(TitleBarHeight), 40d);

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
        // TemplateSettings = new AppWindowTemplateSettings();
        _titleBar = new AppWindowTitleBar(this);

        if (OperatingSystem.IsWindows() && !Design.IsDesignMode)
        {
            InitializeAppWindow();
        }
    }

    static AppWindow()
    {
        AllowInteractionInTitleBarProperty.Changed.AddClassHandler<Control>(OnAllowInteractionInTitleBarChanged);
    }

    public void EnableWindowEffect(bool enable)
    {
        if (!IsWindows) { return; }
        
        if (enable)
        {
            if (IsWindows11)
            {
                Background = Brushes.Transparent;
                TransparencyLevelHint = [WindowTransparencyLevel.Mica];
            }
            else
            {
                Background = Brush.Parse(Application.Current?.RequestedThemeVariant == ThemeVariant.Dark ? "#A1000000" : "#C1FFFFFF");
                TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
            }
        }
        else
        { 
            Background = Brush.Parse(Application.Current?.RequestedThemeVariant == ThemeVariant.Dark ? "#202020" : "#F0F4F9");
            TransparencyLevelHint = [WindowTransparencyLevel.None];
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var sz = base.MeasureOverride(availableSize);

        // UGLY HACK: Seems with CanResize=False, the window shrinks exactly the amount
        // we modify the window in WM_NCCALCSIZE so we need to fix that here
        // But the content measures to the normal size - so in constrained environments
        // like the TaskDialog, stuff gets cut off
        if (IsWindows)
        {
            // TODO: This doesn't appear necessary for TaskDialog anymore, but just in case, I'll
            //       keep this here as a reminder
            //if (!CanResize)
            //{
                //var wid = (16 * PlatformImpl.RenderScaling);
                //var hgt = (8 * PlatformImpl.RenderScaling);
                //sz = new Size(sz.Width + wid, sz.Height + hgt);
            //}

            if (SystemCaptionControl != null)
                _titleBar.SetInset(SystemCaptionControl.DesiredSize.Width, FlowDirection);
        }        

        return sz;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);      

        if (IsWindows && !Design.IsDesignMode)
        {
            _templateRoot = e.NameScope.Find<Border>("RootBorder");
                        
            _captionButtons = e.NameScope.Find<MinMaxCloseControl>("SystemCaptionButtons");
            _defaultTitleBar = e.NameScope.Find<Panel>("DefaultTitleBar");

            // This will set all our TemplateSettings properties
            // OnTitleBarHeightChanged(_titleBar.Height);

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
        if (!this.IsWindows || change.Property != WindowStateProperty)  // Lazy base call OnPropertyChanged
        {
            base.OnPropertyChanged(change);
        }

        if (change.Property == IconProperty)
        {
            base.Icon = new WindowIcon(change.NewValue as Bitmap);
            PseudoClasses.Set(SharedPseudoclasses.s_pcIcon, change.NewValue != null);
        }
        else if (change.Property == ActualThemeVariantProperty)
        {
            SetTitleBarColors();
        }

        // if (change.Property == TitleBarHeightProperty)
        // {
            // OnExtendsContentIntoTitleBarChanged(IsExtendedIntoWindowDecorations);
        // }

        // if (change.Property == ExtendClientAreaToDecorationsHintProperty)
        // {
            // Console.WriteLine($"Pro Ext Chd: {change.GetNewValue<bool>()}");
            // OnExtendsContentIntoTitleBarChanged(change.GetNewValue<bool>());
        // }

        if (IsWindows)
        {
            if (change.Property == WindowStateProperty)
            {
                HandleFullScreenTransition(change.GetNewValue<WindowState>());
                // OnExtendsContentIntoTitleBarChanged(TitleBar.ExtendsContentIntoTitleBar);
                base.OnPropertyChanged(change);  // Enable window size modifications before Avalonia's own logic
            }

            if (!_hasShown)
            {
                // HACK: Because of the frame adjustments made to AppWindow, setting the window
                // size before it is shown will result in an incorrect window size
                // To determine this, we check if WM_SIZE has been sent, if it hasn't, this is a
                // user requested size and we store it and apply it in Opened
                // Changing the size while the window is active works correctly
                if (change.Property == WidthProperty)
                {
                    var newV = change.GetNewValue<double>();
                    if (double.IsInfinity(_win32Manager.LastWMSizeSize.Width))
                    {
                        _win32Manager.LastUserWidth = newV;
                    }
                }
                else if (change.Property == HeightProperty)
                {
                    var newV = change.GetNewValue<double>();
                    if (double.IsInfinity(_win32Manager.LastWMSizeSize.Height))
                    {
                        _win32Manager.LastUserHeight = newV;
                    }
                }
            }
        }
    }

    protected override async void OnOpened(EventArgs e)
    {
        if (IsWindows)
        {
            _hasShown = true;

            if (!double.IsNaN(_win32Manager.LastUserHeight))
            {
                Height = _win32Manager.LastUserHeight;
            }

            if (!double.IsNaN(_win32Manager.LastUserWidth))
            {
                Width = _win32Manager.LastUserWidth;
            }
        }
        

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

        // Check the specified drag rectangles first - these override the default titlebar behavior
        // If that returns null, no drag rects were specified and we use the default logic
        var result = _titleBar.HitTestDragRects(p);

        if (result.HasValue)
        {
            if (result.Value)
                result = CheckExclusionList(p);

            return result.Value;
        }
        else
        {
            // DEFAULT LOGIC

            // We do a simple bounds check here first for the default title bar
            // which is always present, even w/ content extended into titlebar

            // What we know is that the default title bar will always be [0,0,WindowWidth,TitleBar.Height]
            // Therefore, we only need to do check the Y coordinate
            if (p.Y < _titleBar.Height)
            {
                if (TitleBar.TitleBarHitTestType == TitleBarHitTestType.Complex &&
                    !ComplexHitTest(p))
                {
                    return false;
                }
                // Now we know we're in the top part of the window designated as the titlebar
                // We need to see if we can actually drag
                // We need to check our exclusion list to see if we we should let the pointer event pass into
                // the client area or say we've grabbed the title bar

                return CheckExclusionList(p);
            }

            return false;
        }


        bool CheckExclusionList(Point p)
        {
            if (_excludeHitTestList != null && _excludeHitTestList.Count > 0)
            {
                // We don't have to read the AttachedProperty value here because we only add them to the list
                // IF the value is true, and remove them if it's set to false later - saving us that overhead

                // We iterate backwards as this is where we'll purge the list if any controls 
                // have been GC'd and the WeakReference no longer exists
                for (int i = _excludeHitTestList.Count - 1; i >= 0; i--)
                {
                    if (_excludeHitTestList[i].TryGetTarget(out var target))
                    {
                        // Skip invisible or disconnected controls
                        if (!target.IsVisible || !target.IsAttachedToVisualTree())
                            continue;

                        // If control was reparented into new window, matrix may be null, catch that case
                        var mat = target.TransformToVisual(this);
                        if (mat.HasValue)
                        {
                            if (new Rect(target.Bounds.Size).TransformToAABB(mat.Value).Contains(p))
                            {
                                // We've hit a control that's asked to be in the titlebar but allow interaction
                                // return false so NCHITTEST returns HTCLIENT
                                return false;
                            }
                        }
                    }
                    else
                    {
                        _excludeHitTestList.RemoveAt(i);
                    }
                }
            }

            return true;
        }
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

    internal void UpdateContentPosition(Thickness t)
    {        
        if (_templateRoot != null)
            _templateRoot.Margin = t;
    }

    // internal void UpdateFullScreenState(bool isFullScreen)
    // {
    //     if (isFullScreen)
    //     {
    //         ContentMargin = new Thickness();
    //     }
    //     else
    //     {
    //         OnExtendsContentIntoTitleBarChanged(_titleBar.ExtendsContentIntoTitleBar);
    //     }
    // }
    //
    // internal void OnWin32WindowStateChanged(WindowState state)
    // {
    //     if (state == WindowState.FullScreen)
    //     {
    //         ContentMargin = new Thickness();
    //     }
    //     else
    //     {
    //         OnExtendsContentIntoTitleBarChanged(_titleBar.ExtendsContentIntoTitleBar);
    //     }
    // }
    
    private void SetTitleBarColors()
    {
        if (_templateRoot == null)
            return;

        bool foundAccent = _templateRoot.TryFindResource(SYSTEM_ACCENT_COLOR, out var sysColor);
        Color? accentVariant = null;
        var themeVar = ActualThemeVariant;

        if (themeVar == ThemeVariant.Light)
        {
            if (_templateRoot.TryFindResource(SYSTEM_ACCENT_COLOR_DARK_1, out var v))
            {
                accentVariant = Unsafe.Unbox<Color>(v);
            }
        }
        else
        {
            if (_templateRoot.TryFindResource(SYSTEM_ACCENT_COLOR_LIGHT_1, out var v))
            {
                accentVariant = Unsafe.Unbox<Color>(v);
            }
        }

        Color textColor;
        if (_templateRoot.TryFindResource(TEXT_FILL_COLOR_PRIMARY, themeVar, out var value))
        {
            textColor = Unsafe.Unbox<Color>(value);
        }
        else
        {
            if (ActualThemeVariant == ThemeVariant.Dark)
            {
                textColor = Colors.White;
            }
            else
            {
                textColor = Colors.Black;
            }
        }

        SetResource(TITLE_BAR_BACKGROUND, _titleBar.BackgroundColor ?? Colors.Transparent);
        SetResource(TITLE_BAR_FOREGROUND, _titleBar.ForegroundColor ?? textColor);

        SetResource(TITLE_BAR_INACTIVE_BACKGROUND, _titleBar.InactiveBackgroundColor ?? Colors.Transparent);
        SetResource(TITLE_BAR_INACTIVE_FOREGROUND, _titleBar.InactiveForegroundColor ?? Colors.Gray);

        SetResource(SYSTEM_CAPTION_BACKGROUND, _titleBar.ButtonBackgroundColor ?? Colors.Transparent);
        SetResource(SYSTEM_CAPTION_FOREGROUND, _titleBar.ButtonForegroundColor ?? textColor);

        // SetResource(SYSTEM_CAPTION_BACKGROUND_HOVER, _titleBar.ButtonHoverBackgroundColor ?? (foundAccent ? Unsafe.Unbox<Color>(sysColor) : Color.FromArgb(23, 0, 0, 0)));
        SetResource(SYSTEM_CAPTION_FOREGROUND_HOVER, _titleBar.ButtonHoverForegroundColor ?? textColor);

        SetResource(SYSTEM_CAPTION_BACKGROUND_PRESSED, _titleBar.ButtonPressedBackgroundColor ?? (foundAccent ? Unsafe.Unbox<Color>(sysColor) : Color.FromArgb(52, 0, 0, 0)));
        SetResource(SYSTEM_CAPTION_FOREGROUND_PRESSED, _titleBar.ButtonPressedForegroundColor ?? GetPressedColor(textColor));

        SetResource(SYSTEM_CAPTION_BACKGROUND_INACTIVE, _titleBar.ButtonInactiveBackgroundColor ?? Colors.Transparent);
        SetResource(SYSTEM_CAPTION_FOREGROUND_INACTIVE, _titleBar.ButtonInactiveForegroundColor ?? (accentVariant ?? Colors.Gray));


        void SetResource(string name, Color color)
        {
            _templateRoot.Resources[name] = color;
        }

        Color GetPressedColor(Color c)
        {
            Color2 c2 = (Color2)c;
            c2.GetHSLf(out _, out _, out var l, out _);

            if (l < 0.5f)
            {
                return c2.LightenPercent(0.15f);
            }
            else
            {
                return c2.LightenPercent(-0.15f);
            }
        }
    }

    private static void OnAllowInteractionInTitleBarChanged(Control control, AvaloniaPropertyChangedEventArgs propChangeArgs)
    {
        if (control is TopLevel tl || control is Popup)
            return; //throw new InvalidOperationException("AllowTitleBarHitTest cannot be set on TopLevels or Popups");

        
        if (propChangeArgs.GetNewValue<bool>())
        {
            // Control likely isn't attached to the visual tree yet so we have no way of attaching this to the 
            // AppWindow hosting it, defer now, but we'll check first just in case
            if (control.GetVisualRoot() is AppWindow aw)
            {
                aw.AddExcludeHitTestItem(control);
            }
            else if (!Design.IsDesignMode) // Don't attach in Design mode
            {
                // Defer until attached to visual tree
                control.AttachedToVisualTree += AwaitControlAttachedToVisualTree;
            }
        }
        else
        {
            // If we change the value to false while still connected, we'll remove it from the list
            // Otherwise, we'll have to wait for the ref to be GC'd then we'll remove it later
            if (control.GetVisualRoot() is AppWindow aw)
            {
                aw.RemoveExcludeHitTestItem(control);
            }
        }
        
        void AwaitControlAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs args)
        {
            var control = sender as Control;
            control.AttachedToVisualTree -= AwaitControlAttachedToVisualTree;

            if (control.GetVisualRoot() is AppWindow aw)
            {
                aw.AddExcludeHitTestItem(control);
            }
        }
    }

    private void AddExcludeHitTestItem(Control c)
    {
        if (_excludeHitTestList == null)
            _excludeHitTestList = new List<WeakReference<Control>>();

        for (int i = 0; i < _excludeHitTestList.Count; i++)
        {
            // Control was already added - can happen if removed and re-added to the visual tree and
            // the control ref was never GC'd (like page navigation w/ cache)
            if (_excludeHitTestList[i].TryGetTarget(out var target) && target == c)
                return;
        }

        _excludeHitTestList.Add(new WeakReference<Control>(c));
    }

    private void RemoveExcludeHitTestItem(Control c)
    {
        if (_excludeHitTestList == null)
            return;
            
        for (int i = _excludeHitTestList.Count - 1; i >= 0; i--)
        {
            if (_excludeHitTestList[i].TryGetTarget(out var target) && target == c)
            {
                _excludeHitTestList.RemoveAt(i);
                return;
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

    private bool _hasShown;
}
