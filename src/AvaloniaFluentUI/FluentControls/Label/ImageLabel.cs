using System;
using System.IO;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;
using Avalonia.Platform;

namespace AvaloniaFluentUI.Controls;

public class ImageLabel : Control
{
    /// <summary>
    ///     Defines the <see cref="Source" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> SourceProperty =
        AvaloniaProperty.Register<ImageLabel, object?>(nameof(Source));

    /// <summary>
    ///     Defines the <see cref="BlendMode" /> property.
    /// </summary>
    public static readonly StyledProperty<BitmapBlendingMode> BlendModeProperty =
        AvaloniaProperty.Register<ImageLabel, BitmapBlendingMode>(nameof(BlendMode));

    /// <summary>
    ///     Defines the <see cref="Stretch" /> property.
    /// </summary>
    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<ImageLabel, Stretch>(nameof(Stretch), Stretch.Uniform);

    /// <summary>
    ///     Defines the <see cref="StretchDirection" /> property.
    /// </summary>
    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<ImageLabel, StretchDirection>(
            nameof(StretchDirection),
            StretchDirection.Both);

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<ImageLabel, CornerRadius>(nameof(CornerRadius), new CornerRadius(8));

    public static readonly StyledProperty<BitmapInterpolationMode> InterpolationModeProperty =
        AvaloniaProperty.Register<ImageLabel, BitmapInterpolationMode>(nameof(InterpolationMode), BitmapInterpolationMode.HighQuality);

    public static readonly StyledProperty<string?> DelegateTextProperty =
        AvaloniaProperty.Register<ImageLabel, string?>(nameof(DelegateText));

    public static readonly StyledProperty<int> DecodePixelWidthProperty =
        AvaloniaProperty.Register<ImageLabel, int>(nameof(DecodePixelWidth));

    public static readonly StyledProperty<int> DecodePixelHeightProperty =
        AvaloniaProperty.Register<ImageLabel, int>(nameof(DecodePixelHeight));

    public BitmapInterpolationMode InterpolationMode
    {
        get => GetValue(InterpolationModeProperty);
        set => SetValue(InterpolationModeProperty, value);
    }

    public string? DelegateText
    {
        get => GetValue(DelegateTextProperty);
        set => SetValue(DelegateTextProperty, value);
    }

    public int DecodePixelHeight
    {
        get => GetValue(DecodePixelHeightProperty);
        set => SetValue(DecodePixelHeightProperty, value);
    }

    public int DecodePixelWidth
    {
        get => GetValue(DecodePixelWidthProperty);
        set => SetValue(DecodePixelWidthProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    ///     Gets or sets the image that will be displayed.
    /// </summary>
    [Content]
    public object? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    ///     Gets or sets the blend mode for the image.
    /// </summary>
    public BitmapBlendingMode BlendMode
    {
        get => GetValue(BlendModeProperty);
        set => SetValue(BlendModeProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value controlling how the image will be stretched.
    /// </summary>
    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value controlling in what direction the image will be stretched.
    /// </summary>
    public StretchDirection StretchDirection
    {
        get => GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }

    /// <inheritdoc />
    protected override bool BypassFlowDirectionPolicies
    {
        get => true;
    }
    
    private IImage? Image { get; set; }
    
    static ImageLabel()
    {
        AffectsRender<ImageLabel>(StretchProperty, StretchDirectionProperty, BlendModeProperty, CornerRadiusProperty, InterpolationModeProperty, DelegateTextProperty);
        AffectsMeasure<ImageLabel>(SourceProperty, StretchProperty, StretchDirectionProperty);
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<ImageLabel>(AutomationControlType.Image);
    }
    
    public ImageLabel()
    {
        TextElement.SetFontSize(this, 24);
    }

    private IImage? LoadImage()
    {
        if (Source == null)
        {
            return null;
        }

        if (Source is IImage image)
        {
            return image;
        }
        else if (Source is string source)
        {
            if (source.StartsWith("avares://", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = AssetLoader.Open(new Uri(source));
                return DecodeBitmap(stream);
            }

            if (File.Exists(source))
            {
                using var stream = File.OpenRead(source);
                return DecodeBitmap(stream);
            }
        }

        return null;
    }
 
    private Bitmap DecodeBitmap(Stream stream)
    {
        if (DecodePixelWidth > 0)
            return Bitmap.DecodeToWidth(stream, DecodePixelWidth, InterpolationMode);

        if (DecodePixelHeight > 0)
            return Bitmap.DecodeToHeight(stream, DecodePixelHeight, InterpolationMode);

        return new Bitmap(stream);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty)
        {
            if (Source is string && Image is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            Image = LoadImage();
            InvalidateVisual();
        }
    }

    /// <summary>
    ///     Renders the control.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    public sealed override void Render(DrawingContext context)
    {
        var source = Image;

        if (Bounds.Width <= 0 || Bounds.Height <= 0) { return; }

        if (source != null)
        {
            var viewPort = new Rect(Bounds.Size);
            var sourceSize = source.Size;

            var scale = Stretch.CalculateScaling(Bounds.Size, sourceSize, StretchDirection);
            var scaledSize = sourceSize * scale;
            var destRect = viewPort
                .CenterRect(new Rect(scaledSize))
                .Intersect(viewPort);

            var sourceRect = new Rect(sourceSize)
                .CenterRect(new Rect(destRect.Size / scale));

            using (context.PushClip(new RoundedRect(viewPort, CornerRadius)))
            using (context.PushRenderOptions(new RenderOptions { BitmapBlendingMode = BlendMode, BitmapInterpolationMode = InterpolationMode }))
            {
                context.DrawImage(source, sourceRect, destRect);
            }
        }

        if (!String.IsNullOrWhiteSpace(DelegateText))
        {
            // Delegate
           DrawDelegate(context); 
        }
    }

    private void DrawDelegate(DrawingContext context)
    {
        // Delegate
        var rect = new Rect(0, 0, Bounds.Width / 2, Bounds.Height);
        var destRect = new RoundedRect(rect, CornerRadius.TopLeft, 0, 0, CornerRadius.BottomLeft); 
        context.DrawRectangle(new SolidColorBrush(Color.FromArgb(80, 255, 255, 255)), null, destRect);
        
        var layout = new TextLayout(
            text: DelegateText,
            typeface: Typeface.Default,
            fontSize: TextElement.GetFontSize(this),
            foreground: Brushes.Black,  
            textWrapping: TextWrapping.WrapWithOverflow,
            textTrimming: TextTrimming.CharacterEllipsis,
            textAlignment: TextAlignment.Center,
            maxWidth: rect.Width - 24);
        
        // 居中绘制 
        var point = new Point(0, (Bounds.Height - layout.Height) / 2);
        layout.Draw(context, point);
    }

    /// <summary>
    ///     Measures the control.
    /// </summary>
    /// <param name="availableSize">The available size.</param>
    /// <returns>The desired size of the control.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        var source = Image;
        var result = new Size();

        if (source != null)
        {
            result = Stretch.CalculateSize(availableSize, source.Size, StretchDirection);
        }

        return result;
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var source = Image;

        if (source != null)
        {
            var sourceSize = source.Size;
            var result = Stretch.CalculateSize(finalSize, sourceSize);
            return result;
        }

        return new Size();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new ImageAutomationPeer(this);
    }
}
