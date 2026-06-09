using System;
using System.Collections.Generic;
using System.Diagnostics;
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

[TemplatePart(PART_CURRENT_IMAGE, typeof(Image))]
[TemplatePart(PART_NEXT_IMAGE, typeof(Image))]
[TemplatePart(PART_PREVIOUS_BUTTON, typeof(Button))]
[TemplatePart(PART_NEXT_BUTTON, typeof(Button))]
public class FlipView : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<string>?> ImageSourceProperty =
        AvaloniaProperty.Register<FlipView, IEnumerable<string>?>(nameof(ImageSource));

    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<FlipView, int>(nameof(SelectedIndex), defaultValue: -1);

    public static readonly StyledProperty<BitmapInterpolationMode> ImageInterpolationModeProperty =
        AvaloniaProperty.Register<FlipView, BitmapInterpolationMode>(nameof(ImageInterpolationMode));

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<FlipView, Stretch>(nameof(Stretch));

    public static readonly StyledProperty<int> DecodeToHeightProperty =
        AvaloniaProperty.Register<FlipView, int>(nameof(DecodeToHeight), defaultValue: 1080);

    public static readonly StyledProperty<int> DecodeToWidthProperty =
        AvaloniaProperty.Register<FlipView, int>(nameof(DecodeToWidth));
    
    public static readonly StyledProperty<double> IntervalProperty =
        AvaloniaProperty.Register<FlipView, double>(nameof(Interval), defaultValue: 1500, validate: value => value >= 600);

    public static readonly StyledProperty<bool> IsAutoPlayProperty =
        AvaloniaProperty.Register<FlipView, bool>(nameof(IsAutoPlay));
    
    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<FlipView, int>(nameof(ItemCount));

    public static readonly StyledProperty<FlipOrientation> OrientationProperty =
        AvaloniaProperty.Register<FlipView, FlipOrientation>(nameof(Orientation), defaultValue: FlipOrientation.Horizontal);

    public static readonly StyledProperty<int> MaxVisiblePipsProperty =
        AvaloniaProperty.Register<FlipView, int>(nameof(MaxVisiblePips), defaultValue: 8);

    public int MaxVisiblePips
    {
        get => GetValue(MaxVisiblePipsProperty);
        set => SetValue(MaxVisiblePipsProperty, value);
    }

    public FlipOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    
    /// <summary>
    /// 最小间隔 400
    /// </summary>
    public double Interval
    {
        get => GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

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
    /// 默认缩放到高度 1080, 小于0不缩放
    /// </summary>
    public int DecodeToHeight
    {
        get => GetValue(DecodeToHeightProperty);
        set => SetValue(DecodeToHeightProperty, value);
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }
    
    public BitmapInterpolationMode ImageInterpolationMode
    {
        get => GetValue(ImageInterpolationModeProperty);
        set => SetValue(ImageInterpolationModeProperty, value);
    }

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
    
    private bool _isRunning;
    private Image? _currentImage;
    private Image? _nextImage;
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _previousButton?.Click -= OnPreviousButtonClick;
        _nextButton?.Click -= OnNextButtonClick;

        _currentImage = e.NameScope.Find<Image>(PART_CURRENT_IMAGE);
        _nextImage = e.NameScope.Find<Image>(PART_NEXT_IMAGE);
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

    private void HandleAutoPlayChanged(bool value)
    {
        if (value && this.IsAttachedToVisualTree())
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

    private void UpdateButtonVisibility()
    {
        if (_previousButton != null) { _previousButton.IsVisible = ItemCount > 0 && SelectedIndex > 0; }
        if (_nextButton != null) { _nextButton.IsVisible = ItemCount > 0 && SelectedIndex < ItemCount - 1; }
    }

    private void HideButtons()
    {
        if (_previousButton != null) { _previousButton.IsVisible = false; }
        if (_nextButton != null) { _nextButton.IsVisible = false; }
    }

    public void Start()
    {
        if (ItemCount < -1 || !this.IsAttachedToVisualTree()) { return; }

        IsAutoPlay = true;
        _autoPlayTimer.Start();
    }

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
        _disposeCts?.Cancel();
        _disposeCts = new CancellationTokenSource();
        var token = _disposeCts.Token;
        
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
        // SelectedIndex = -1;
#if DEBUG
        Debug.WriteLine("Dispose Image");
#endif
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
        else if (change.Property == ImageInterpolationModeProperty)
        {
            HandleImageInterpolationModeChanged(change.GetNewValue<BitmapInterpolationMode>());
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

    private void HandleImageInterpolationModeChanged(BitmapInterpolationMode mode)
    {
        if (_currentImage != null && _nextImage != null)
        {
            RenderOptions.SetBitmapInterpolationMode(_currentImage, mode);
            RenderOptions.SetBitmapInterpolationMode(_nextImage, mode);
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

#if DEBUG
        Debug.WriteLine("Reload Image");
#endif

        ItemCount = imagePaths.Count;
        if (SelectedIndex >= ItemCount || SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }
        
        string path = imagePaths[SelectedIndex];
        imagePaths.RemoveAt(SelectedIndex);
        var cb = LoadBitMap(path, DecodeToHeight, DecodeToWidth);

        Dispatcher.UIThread.Post(() =>
        {
            _currentImage?.Source = cb;
            Debug.WriteLine(_currentImage);
        }, DispatcherPriority.Render);
        
        await foreach (var bitmap in LoadImagesAsync(imagePaths))
        {
            _items.Add(bitmap);
#if DEBUG
            Debug.WriteLine("Load Image: " + bitmap);
#endif
        }
        _items.Insert(SelectedIndex, cb);

#if DEBUG
        Debug.WriteLine("Current Index: " + SelectedIndex);
        Debug.WriteLine("Is Auto Play: " + IsAutoPlay);
#endif
        
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
            
            await Task.Run(() => { bitmap = LoadBitMap(path, dh, dw); });

            if (bitmap != null)
            {
                yield return bitmap;
            }
        }
    }

    private Bitmap LoadBitMap(string path, int dh = 0, int dw = 0)
    {
        try
        {
            if (path.StartsWith("avares://"))
            {
                using var stream = AssetLoader.Open(new Uri(path));
                if (dh > 0)
                {
                    return Bitmap.DecodeToHeight(stream, dh);
                }
                if (dw > 0)
                {
                    return Bitmap.DecodeToWidth(stream, dw); 
                }
                return new Bitmap(stream);
            }
            else
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                if (dh > 0)
                {
                    return Bitmap.DecodeToHeight(stream, dh);
                }
                if (dw > 0)
                {
                    return Bitmap.DecodeToWidth(stream, dw);
                }
                return new Bitmap(stream);
            }
        }
        catch (FileNotFoundException e)
        {
            return null;
        }
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

        _cancelAnimationCts?.Cancel();
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

    private void ResetTransform()
    {
        _currentTransform.X = 0;
        _currentTransform.Y = 0;

        _nextTransform.X = 0;
        _nextTransform.Y = 0;
    }

    public void Next()
    {
        if (_isRunning || SelectedIndex >= ItemCount - 1)
        {
            return;
        }
    
        SelectedIndex++;
    }

    public void Previous()
    {
        if (_isRunning || SelectedIndex <= 0) 
        {
            return;
        }

        SelectedIndex--;
    }
}


