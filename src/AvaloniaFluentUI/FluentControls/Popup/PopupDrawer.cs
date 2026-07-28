using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_CONTENT_HOST, Type = typeof(Border))]
[TemplatePart(Name = PART_HIT_TEST_AREA, Type = typeof(Border))]
[TemplatePart(Name = PART_CLOSE_BUTTON, Type = typeof(ToolButton))]
[PseudoClasses(":open", ":left", ":right", ":top", ":bottom")]
public class PopupDrawer : ContentControl
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<PopupDrawer, bool>(nameof(IsOpen), coerce: CoerceIsOpen, defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<PopupDrawerPosition> PositionProperty =
        AvaloniaProperty.Register<PopupDrawer, PopupDrawerPosition>(nameof(Position));

    /// <summary>
    ///     Defines the <see cref="DrawerWidth" /> property.
    /// </summary>
    public static readonly StyledProperty<double> DrawerWidthProperty =
        AvaloniaProperty.Register<PopupDrawer, double>(nameof(DrawerWidth), 328);

    /// <summary>
    ///     Defines the <see cref="DrawerHeight" /> property.
    /// </summary>
    public static readonly StyledProperty<double> DrawerHeightProperty =
        AvaloniaProperty.Register<PopupDrawer, double>(nameof(DrawerHeight), Double.NaN);

    /// <summary>
    ///     Defines the <see cref="IsLightDismissEnabled" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        AvaloniaProperty.Register<PopupDrawer, bool>(nameof(IsLightDismissEnabled), true);

    /// <summary>
    /// <para>设置或获取点击外部是否关闭抽屉, 默认点击外部可关闭</para>
    /// <para>在<c>True</c>的状态下无法点击除Drawer区域的其他区域, 为<c>False</c>则可以点击外部区域</para>
    /// </summary>
    public bool IsLightDismissEnabled
    {
        get => GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }
    
    /// <summary>
    /// 设置或获取弹出抽屉的高度
    /// </summary>
    public double DrawerHeight
    {
        get => GetValue(DrawerHeightProperty);
        set => SetValue(DrawerHeightProperty, value);
    }

    /// <summary>
    /// 设置或获取弹出抽屉的宽度
    /// </summary>
    public double DrawerWidth
    {
        get => GetValue(DrawerWidthProperty);
        set => SetValue(DrawerWidthProperty, value);
    }

    /// <summary>
    /// 设置或获取弹出抽屉是否弹出
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty,value);
    }

    /// <summary>
    /// 设置或获取弹出抽屉的弹出位置
    /// </summary>
    public PopupDrawerPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty,value);
    }
    
    public bool IsRunning { get; private set; }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    private ToolButton? _closeButton;
    /// <summary>
    /// 内容载体
    /// </summary>
    private Border? _contentHost;
    /// <summary>
    /// 击中关闭区域
    /// </summary>
    private Border? _hitTestArea;

    // private CancellationTokenSource? _cts;
    private readonly TranslateTransform _transform = new();

    private const string PART_CONTENT_HOST = "PART_ContentHost";
    private const string PART_HIT_TEST_AREA = "PART_HitTestArea";
    private const string PART_CLOSE_BUTTON = "PART_CloseButton";

    /// <summary>
    /// 显示抽屉栏
    /// </summary>
    public void Show()
    {
        IsOpen = true;
    }

    /// <summary>
    /// 隐藏抽屉栏
    /// </summary>
    public void Hide()
    {
        IsOpen = false;
    }
    
    /// <summary>
    /// 切换显示抽屉栏
    /// </summary>
    public void Toggle()
    {
        IsOpen = !IsOpen;
    }
    
    /// <summary>
    /// 如果动画在运行就阻止<see cref="IsOpen"/>改变值
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool CoerceIsOpen(AvaloniaObject sender, bool value)
    {
        var drawer = (PopupDrawer)sender;

        if (drawer.IsRunning)
        {
            return drawer.IsOpen;
        }
        
        return value;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _closeButton?.Click -= OnCloseButtonClick;
        _hitTestArea?.PointerPressed -= OnHitTestAreaPointerPressed;
        
        _contentHost = e.NameScope.Find<Border>(PART_CONTENT_HOST);
        _closeButton = e.NameScope.Find<ToolButton>(PART_CLOSE_BUTTON);
        _hitTestArea = e.NameScope.Find<Border>(PART_HIT_TEST_AREA);

        _closeButton?.Click += OnCloseButtonClick;
        _hitTestArea?.PointerPressed += OnHitTestAreaPointerPressed;

        if (_contentHost != null)
        {
            _contentHost.RenderTransform = _transform;
            _contentHost.IsVisible = IsOpen;
        }

        _hitTestArea?.IsVisible = IsOpen;
        UpdatePseudoClasses();
    }

    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        Hide();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenProperty)
        {
            UpdatePseudoClasses();
            RunPopupDrawerAnimation();
        }
        
        if (change.Property == PositionProperty)
        {
            UpdatePseudoClasses();
            ResetPosition();
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":left", Position == PopupDrawerPosition.Left);
        PseudoClasses.Set(":right", Position == PopupDrawerPosition.Right);
        PseudoClasses.Set(":top", Position == PopupDrawerPosition.Top);
        PseudoClasses.Set(":bottom", Position == PopupDrawerPosition.Bottom);
    }

    private void ResetPosition()
    {
        if (_contentHost == null) { return; }
        
        switch(Position)
        {
            case PopupDrawerPosition.Left:
                _transform.X = -DrawerWidth;
                _transform.Y = 0;
                break;
            case PopupDrawerPosition.Right:
                _transform.X = DrawerWidth;
                _transform.Y = 0;
                break;
            case PopupDrawerPosition.Top:
                _transform.X = 0;
                _transform.Y = -DrawerHeight;
                break;
            case PopupDrawerPosition.Bottom:
                _transform.X = 0; 
                _transform.Y =  DrawerHeight;
                break;
        }
    }

    private (double from, double to) GetPopupPos(double distance)
    {
        double from, to;
        
        if(IsOpen)
        {

            from = Position switch
            {
                PopupDrawerPosition.Left   => -distance,
                PopupDrawerPosition.Right  => distance,
                PopupDrawerPosition.Top    => -distance,
                PopupDrawerPosition.Bottom => distance,
                _                          => 0
            };
            
            to = 0;
            _contentHost!.IsVisible = true;
            _hitTestArea!.IsVisible = true;
        }
        else
        {
            from = 0;
            
            to = Position switch
            {
                PopupDrawerPosition.Left   => -distance,
                PopupDrawerPosition.Right  => distance,
                PopupDrawerPosition.Top    => -distance,
                PopupDrawerPosition.Bottom => distance,
                _                          => 0
            };

        }
        
        return (from, to);
    }

    private async void RunPopupDrawerAnimation()
    {
        if (_contentHost == null)
            return;

        IsRunning = true;

        AvaloniaProperty property;
        double distance;
        switch (Position)
        {
            case PopupDrawerPosition.Top: 
            case PopupDrawerPosition.Bottom:
                property = TranslateTransform.YProperty;
                distance = DrawerHeight;
                break;
            case PopupDrawerPosition.Left: 
            case PopupDrawerPosition.Right:
                property = TranslateTransform.XProperty;
                distance = DrawerWidth;
                break;
            default:
                property = TranslateTransform.XProperty;
                distance = 0;
                break;
        }
        
        var (from, to) = GetPopupPos(distance);

        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(IsOpen ? 333 : 167),
            FillMode = FillMode.Forward,
            Easing = IsOpen ? new CubicEaseOut() : new CubicEaseIn(),
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters = { new Setter(property, from) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters = { new Setter(property, to) }
                }
            }
        };

        try
        {
            await animation.RunAsync(_contentHost);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        finally
        {
            IsRunning = false;
        }

        if(!IsOpen)
        {
            _contentHost.IsVisible = false;
            _hitTestArea?.IsVisible = false;
        }
    }

    /// <summary>
    /// 点击外部隐藏, 只有再<see cref="IsLightDismissEnabled"/>为True时才有效
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnHitTestAreaPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsLightDismissEnabled || !IsOpen)
        {
            return;
        }
        
        // 内部点击则不关闭
        if (e.Source is Visual visual && visual == _contentHost)
        {
            return;
        }
        
        Hide();         
        // e.Handled = true;
    }
}
