using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Controls.Enums;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// 图片轮播视图
/// </summary>
[TemplatePart(Name = PART_NEXT_IMAGE,       Type = typeof(ImageLabel))]
[TemplatePart(Name = PART_NEXT_BUTTON,      Type = typeof(Button))]
[TemplatePart(Name = PART_CURRENT_IMAGE,    Type = typeof(ImageLabel))]
[TemplatePart(Name = PART_PREVIOUS_BUTTON,  Type = typeof(Button))]
public class FlipView : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<string>?> ImageSourceProperty =
        AvaloniaProperty.Register<FlipView, IEnumerable<string>?>(nameof(ImageSource));

    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<FlipView, int>(nameof(SelectedIndex), -1);

    public static readonly StyledProperty<BitmapInterpolationMode> InterpolationModeProperty =
        ImageLabel.InterpolationModeProperty.AddOwner<FlipView>();

    public static readonly StyledProperty<Stretch> StretchProperty =
        ImageLabel.StretchProperty.AddOwner<FlipView>();

    public static readonly StyledProperty<int> DecodeToHeightProperty =
        ImageLabel.DecodePixelHeightProperty.AddOwner<FlipView>();

    public static readonly StyledProperty<int> DecodeToWidthProperty =
        ImageLabel.DecodePixelWidthProperty.AddOwner<FlipView>();

    public static readonly StyledProperty<double> IntervalProperty =
        AvaloniaProperty.Register<FlipView, double>(nameof(Interval), 1500, validate: value => value >= 600);

    public static readonly StyledProperty<bool> IsAutoPlayProperty =
        AvaloniaProperty.Register<FlipView, bool>(nameof(IsAutoPlay));

    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<FlipView, int>(nameof(ItemCount));

    public static readonly StyledProperty<FlipOrientation> OrientationProperty =
        AvaloniaProperty.Register<FlipView, FlipOrientation>(nameof(Orientation));

    public static readonly StyledProperty<int> MaxVisiblePipsProperty =
        AvaloniaProperty.Register<FlipView, int>(nameof(MaxVisiblePips), 8);

    public static readonly StyledProperty<bool> PipsPagerIsVisibleProperty =
        AvaloniaProperty.Register<FlipView, bool>(nameof(PipsPagerIsVisible), true);

    /// <summary>
    /// 获取或设置底部圆点是否显示
    /// </summary>
    public bool PipsPagerIsVisible
    {
        get => GetValue(PipsPagerIsVisibleProperty);
        set => SetValue(PipsPagerIsVisibleProperty, value);
    }

    /// <summary>
    /// 获取底部圆点最大显示数量,默认最大显示<c>8</c>个
    /// </summary>
    public int MaxVisiblePips
    {
        get => GetValue(MaxVisiblePipsProperty);
        set => SetValue(MaxVisiblePipsProperty, value);
    }

    /// <summary>
    /// 获取或设置当前轮播方向
    /// </summary>
    public FlipOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// 获取或设置自动播放间隔(ms) 最小间隔 <c>600</c>, 默认间隔<c>1500</c>
    /// </summary>
    public double Interval
    {
        get => GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    /// <summary>
    /// 获取或设置当前是否是自动播放
    /// </summary>
    public bool IsAutoPlay
    {
        get => GetValue(IsAutoPlayProperty);
        set => SetValue(IsAutoPlayProperty, value);
    }

    public int DecodeToWidth
    {
        get => GetValue(DecodeToWidthProperty);
        set => SetValue(DecodeToWidthProperty, value);
    }

    /// <summary>
    /// 默认缩放到高度 800, 小于0不缩放
    /// </summary>
    public int DecodeToHeight
    {
        get => GetValue(DecodeToHeightProperty);
        set => SetValue(DecodeToHeightProperty, value);
    }

    /// <summary>
    /// 获取或设置图片拉伸方式
    /// </summary>
    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }
    
    public BitmapInterpolationMode InterpolationMode
    {
        get => GetValue(InterpolationModeProperty);
        set => SetValue(InterpolationModeProperty, value);
    }

    /// <summary>
    /// 图片路径
    /// </summary>
    public IEnumerable<string>? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public int ItemCount
    {
        get => GetValue(ItemCountProperty);
        private set => SetValue(ItemCountProperty, value);
    }

    static FlipView()
    {
        StretchProperty.OverrideDefaultValue<FlipView>(Stretch.UniformToFill);
        DecodeToHeightProperty.OverrideDefaultValue<FlipView>(800);
    }
    
    private bool _isRunning;
    private ImageLabel? _currentImage;
    private ImageLabel? _nextImage;
    private Button? _previousButton;
    private Button? _nextButton;

    private readonly DispatcherTimer _autoPlayTimer;
    private readonly TranslateTransform _currentTransform = new();
    private readonly TranslateTransform _nextTransform = new();

    private CancellationTokenSource? _disposeCts;
    private CancellationTokenSource? _cancelAnimationCts;

    private const string PART_CURRENT_IMAGE = "PART_CurrentImage";
    private const string PART_NEXT_IMAGE = "PART_NextImage";
    private const string PART_PREVIOUS_BUTTON = "PART_PreviousButton";
    private const string PART_NEXT_BUTTON = "PART_NextButton";

    private List<Bitmap> _items = new List<Bitmap>();
    public List<Bitmap> Items => _items;
    public TimeSpan ForwardDuration { get; set; } = TimeSpan.FromMilliseconds(400);
    public TimeSpan BackwardDuration { get; set; } = TimeSpan.FromMilliseconds(360);
    public Easing SlideInEasing { get; set; } = new CubicEaseOut();
    public Easing SlideOutEasing { get; set; } = new LinearEasing();
    
    private IList<IImageLabelDelegate?>? Delegates { get; set; }

    private bool HasDelegate() => Delegates != null && Delegates.Count == ItemCount;
    
    /// <summary>
    /// 设置当前图片的绘制代理
    /// </summary>
    /// <param name="delegates"></param>
    public void SetImageDelegates(IList<IImageLabelDelegate?>? delegates)
    {
        Delegates = delegates;
        if (Delegates == null)
        {
            // 代理不为为Null则清空当前代理状态
            _currentImage?.SetImageLabelDelegate(null);
            _nextImage?.SetImageLabelDelegate(null);
        }
        else if (HasDelegate())
        {
            // 不为空就给当前图片设置代理
            _currentImage?.SetImageLabelDelegate(Delegates[SelectedIndex]);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _previousButton?.Click -= OnPreviousButtonClick;
        _nextButton?.Click -= OnNextButtonClick;

        _currentImage = e.NameScope.Find<ImageLabel>(PART_CURRENT_IMAGE);
        _nextImage = e.NameScope.Find<ImageLabel>(PART_NEXT_IMAGE);
        _previousButton = e.NameScope.Find<Button>(PART_PREVIOUS_BUTTON);
        _nextButton = e.NameScope.Find<Button>(PART_NEXT_BUTTON);

        _previousButton?.Click += OnPreviousButtonClick;
        _nextButton?.Click += OnNextButtonClick;

        _currentImage?.RenderTransform = _currentTransform;
        _nextImage?.RenderTransform = _nextTransform;
    }

    public FlipView()
    {
        _autoPlayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Interval) };
        _autoPlayTimer.Tick += OnAutoPlay;
        AddHandler(RequestBringIntoViewEvent, OnRequestBringIntoView);
    }

    private void OnRequestBringIntoView(object? sender, RequestBringIntoViewEventArgs e) => e.Handled = true;

    private void OnAutoPlay(object? sender, EventArgs e)
    {
        if (ItemCount <= 1)
        {
            Stop(); 
            return;
        }

        // 自动播放到最后一个则返回到第一个
        if (SelectedIndex >= ItemCount -1)
        {
            SelectedIndex = 0;
        }
        Next();
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        UpdateButtonVisibility();
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        HideButtons();
    }

    /// <summary>
    /// 只有附加到了视觉树上才启用自动播放
    /// </summary>
    /// <param name="value"></param>
    private void HandleAutoPlayChanged(bool value)
    {
        if (value && this.IsAttachedToVisualTree() && IsEnabled)
        {
            Start();
        }
        else
        {
            _autoPlayTimer.Stop();
        }
    }

    private void HandleIntervalChanged()
    {
        if (IsAutoPlay) { _autoPlayTimer.Stop(); }

        _autoPlayTimer.Interval = TimeSpan.FromMilliseconds(Interval);
        
        if (IsAutoPlay) { Start(); }
    }

    /// <summary>
    /// 更新上一张,下一张按钮显示状态
    /// </summary>
    private void UpdateButtonVisibility()
    { 
        _previousButton?.IsVisible = ItemCount > 0 && SelectedIndex > 0;
        _nextButton?.IsVisible = ItemCount > 0 && SelectedIndex < ItemCount - 1;
    }

    private void HideButtons()
    {
        _previousButton?.IsVisible = false; 
        _nextButton?.IsVisible = false;
    }

    /// <summary>
    /// 只有在图片数量大于 1, 且附加到视觉书上才会自动播放
    /// </summary>
    public void Start()
    {
        if (ItemCount < 1 || !this.IsAttachedToVisualTree()) { return; }

        IsAutoPlay = true;
        _autoPlayTimer.Start();
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void Stop()
    {
        IsAutoPlay = false;
        _autoPlayTimer.Stop();
    }

    private void OnPreviousButtonClick(object? sender, RoutedEventArgs e) => Previous();

    private void OnNextButtonClick(object? sender, RoutedEventArgs e) => Next();

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (_items.Count <= 0 && ItemCount > 0)
        {
            ReloadImages();
            _disposeCts?.Cancel();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        _autoPlayTimer.Stop();

        _cancelAnimationCts?.Cancel();
        _cancelAnimationCts?.Dispose();
        _disposeCts?.Cancel();
        _disposeCts?.Dispose();
        _disposeCts = new CancellationTokenSource();
        var token = _disposeCts.Token;
        
        // 延迟释放,防止快速切换页面导致崩溃
        Dispatcher.UIThread.Post(() =>
        {
            if (token.IsCancellationRequested) { return; }
            
            DisposeImage();
        },
        DispatcherPriority.Background);
    }

    private void DisposeImage()
    { 
        _currentImage?.Source = null; 
        _nextImage?.Source = null; 

        foreach (var bitmap in _items) 
        { 
            bitmap.Dispose();
        } 
        
        _items.Clear();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ImageSourceProperty)
        {
            _autoPlayTimer.Stop();
            DisposeImage();
            
            if (this.IsAttachedToVisualTree())
            {
                ReloadImages();
            }
            else
            {
                var newValue = ImageSource?.ToList();
                if (newValue != null)
                {
                    ItemCount = newValue.Count;
                }
            }
        }
        else if (change.Property == SelectedIndexProperty)
        {
            int ov = change.GetOldValue<int>();
            int nv = change.GetNewValue<int>();

            if (ov == -1 || nv < 0 || nv >= _items.Count)
            {
                return;
            }
            if (IsPointerOver) { UpdateButtonVisibility(); }
            RunSliderAnimationAsync(_items[nv], nv, nv > ov);
        }
        else if (change.Property == IsAutoPlayProperty)
        {
            HandleAutoPlayChanged(change.GetNewValue<bool>());
        }
        else if (change.Property == IntervalProperty)
        {
            HandleIntervalChanged();
        }
    }

    private async void ReloadImages()
    {
        DisposeImage();
        var imagePaths = ImageSource?.ToList();
        if (imagePaths == null || !imagePaths.Any())
        {
            ItemCount = -1;
            return;
        }

        ItemCount = imagePaths.Count;
        if (SelectedIndex >= ItemCount || SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }
        
        string path = imagePaths[SelectedIndex];
        imagePaths.RemoveAt(SelectedIndex);
        var cb = LoadBitMap(path, DecodeToHeight, DecodeToWidth);

        IImageLabelDelegate? dg = null;
        if (HasDelegate())
        {
            dg = Delegates![SelectedIndex];
        }
        
        Dispatcher.UIThread.Post(() =>
        {
            _currentImage?.Source = cb;
            if (dg != null)
            {
                _currentImage?.SetImageLabelDelegate(dg);
            }
        }, DispatcherPriority.Render);
        
        await foreach (var bitmap in LoadImagesAsync(imagePaths))
        {
            _items.Add(bitmap);
        }
        
        _items.Insert(SelectedIndex, cb);
        
        ResetTransform();
        if (IsAutoPlay) Start();
    }

    private async IAsyncEnumerable<Bitmap> LoadImagesAsync(IEnumerable<string> imagePaths)
    {
        foreach (var path in imagePaths)
        {
            int dh = DecodeToHeight;
            int dw = DecodeToWidth;
            Bitmap? bitmap = null;
            
            await Task.Run(() => { bitmap = LoadBitMap(path, dw, dh); });

            if (bitmap != null)
            {
                yield return bitmap;
            }
        }
    }

    private Bitmap? LoadBitMap(string path, int dh = 0, int dw = 0)
    {
        try
        {
            if (path.StartsWith("avares://"))
            {
                using var stream = AssetLoader.Open(new Uri(path));
                return DecodeBitmap(stream, dw, dh);
            }
            else
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                return DecodeBitmap(stream, dw, dh);
            }
        }
        catch (FileNotFoundException e)
        {
            return null;
        }
    }
    
    private Bitmap DecodeBitmap(Stream stream, int dw, int dh)
    {
        if (dw > 0)
            return Bitmap.DecodeToWidth(stream, dw);

        if (dh > 0)
            return Bitmap.DecodeToHeight(stream, dh);

        return new Bitmap(stream);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        if (ItemCount == 0) { return; }

        if (e.Delta.Y < 0)
        {
            Next();
        }
        else if (e.Delta.Y > 0)
        {
            Previous();
        }

        e.Handled = true;
        base.OnPointerWheelChanged(e);
    }

    private async void RunSliderAnimationAsync(IImage image, int targetIndex, bool forward)
    {
        double distance;
        StyledProperty<double> property;
        if (Orientation == FlipOrientation.Horizontal)
        {
            distance = Bounds.Width;
            property = TranslateTransform.XProperty;
        }
        else
        {
            distance = Bounds.Height;
            property = TranslateTransform.YProperty;
        }

        if (_currentImage == null || _nextImage == null) { return; }

        IImageLabelDelegate? nd = null;
        if (HasDelegate())
        {
            nd = Delegates![targetIndex];
            if (nd != null)
            {
                _nextImage.SetImageLabelDelegate(nd);
            }
        }

        _cancelAnimationCts?.Cancel();
        _cancelAnimationCts?.Dispose();
        _cancelAnimationCts = new CancellationTokenSource();
        var token = _cancelAnimationCts.Token;

        _isRunning = true;
        _nextImage.Source = image;
        _nextImage.IsVisible = true;

        var duration = forward ? ForwardDuration : BackwardDuration;

        var currentAnimation = new Animation
        {
            Duration = duration,
            FillMode = FillMode.Forward,
            Easing = SlideOutEasing,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = { new Setter(property, 0d) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(property, forward ? -distance : distance) }
                }
            }
        };

        var nextAnimation = new Animation
        {
            Duration = duration,
            FillMode = FillMode.Forward,
            Easing = SlideInEasing,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = { new Setter(property, forward ? distance : -distance) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(property, 0d) }
                }
            }
        };

        try
        {
            await Task.WhenAll(
                currentAnimation.RunAsync(_currentImage, token),
                nextAnimation.RunAsync(_nextImage, token));

            if (HasDelegate())
            {
                _nextImage.SetImageLabelDelegate(null);
                _currentImage.SetImageLabelDelegate(nd);
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                SelectedIndex = targetIndex;
                _currentImage.Source = image;
            }

            _nextImage.Source = null;
            _nextImage.IsVisible = false;
            ResetTransform();
            _isRunning = false;
        }
    }

    /// <summary>
    /// 重置坐标位置
    /// </summary>
    private void ResetTransform()
    {
        _currentTransform.X = 0;
        _currentTransform.Y = 0;

        _nextTransform.X = 0;
        _nextTransform.Y = 0;
    }

    /// <summary>
    /// 下一张图片
    /// </summary>
    public void Next()
    {
        if (_isRunning || SelectedIndex >= ItemCount - 1)
        {
            return;
        }
    
        SelectedIndex++;
    }

    /// <summary>
    /// 上一张图片
    /// </summary>
    public void Previous()
    {
        if (_isRunning || SelectedIndex <= 0) 
        {
            return;
        }

        SelectedIndex--;
    }
}


