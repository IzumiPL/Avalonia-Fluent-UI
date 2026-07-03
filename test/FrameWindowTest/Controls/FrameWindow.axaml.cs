using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaFluentUI.Styling;
using AvaloniaFluentUI.Windowing;

namespace FrameWindowTest.Controls;

[TemplatePart(Name = PART_MINIMIZE_BUTTON, Type = typeof(Button))]
[TemplatePart(Name = PART_MAXIMIZE_BUTTON, Type = typeof(Button))]
[TemplatePart(Name = PART_CLOSE_BUTTON, Type = typeof(Button))]
public class FrameWindow : Window
{
    private Button? _minimizeButton;
    private Button? _maximizeButton;
    private Button? _closeButton;
    
    private const string PART_MINIMIZE_BUTTON = "PART_MinimizeButton";
    private const string PART_MAXIMIZE_BUTTON = "PART_MaximizeButton";
    private const string PART_CLOSE_BUTTON = "PART_CloseButton";

    public static readonly StyledProperty<double> TitleBarHeightProperty =
        AvaloniaProperty.Register<FrameWindow, double>(nameof(TitleBarHeight), defaultValue: 45);

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<FrameWindow, double>(nameof(IconSize), defaultValue: 18);

    public static readonly StyledProperty<object?> TitleBarContentProperty =
        AvaloniaProperty.Register<FrameWindow, object?>(nameof(TitleBarContent));

    public static readonly StyledProperty<IDataTemplate?> TitleBarContentTemplateProperty =
        AvaloniaProperty.Register<FrameWindow, IDataTemplate?>(nameof(TitleBarContentTemplate));

    public static readonly StyledProperty<Thickness> TitleBarContentMarginProperty =
        AvaloniaProperty.Register<FrameWindow, Thickness>(nameof(TitleBarContentMargin), defaultValue: new Thickness(8, 0, 140, 0));

    public static readonly StyledProperty<bool> TitleBarContentIsVisibleProperty =
        AvaloniaProperty.Register<FrameWindow, bool>(nameof(TitleBarContentIsVisible), defaultValue: true);

    public bool TitleBarContentIsVisible
    {
        get => GetValue(TitleBarContentIsVisibleProperty);
        set => SetValue(TitleBarContentIsVisibleProperty, value);
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
    
    public double TitleBarHeight
    {
        get => GetValue(TitleBarHeightProperty);
        set => SetValue(TitleBarHeightProperty, value);
    }
    
    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    
    public FrameWindow()
    {
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
    }

    private void OnCloseButtonClicked(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void OnMaximizeButtonClicked(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void OnMinimizeButtonClicked(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
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
        if (enable)
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

    protected override Type StyleKeyOverride => typeof(FrameWindow);
}
