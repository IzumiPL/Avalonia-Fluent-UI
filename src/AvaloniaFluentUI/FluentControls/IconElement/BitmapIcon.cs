using System;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents and icon that uses a bitmap as its content
/// </summary>
public class BitmapIcon : IconElement
{
    /// <summary>
    /// Defines the <see cref="UriSource"/> property
    /// </summary>
    public static readonly StyledProperty<Uri?> UriSourceProperty =
        AvaloniaProperty.Register<BitmapIcon, Uri?>(nameof(UriSource));

    /// <summary>
    /// Defines the <see cref="ShowAsMonochrome"/> property
    /// </summary>
    public static readonly StyledProperty<bool> ShowAsMonochromeProperty =
        AvaloniaProperty.Register<BitmapIcon, bool>(nameof(ShowAsMonochrome));

    /// <summary>
    /// Gets or sets the Uniform Resource Identifier (URI) of the bitmap to use as the icon content.
    /// </summary>
    public Uri? UriSource
    {
        get => GetValue(UriSourceProperty);
        set => SetValue(UriSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the bitmap is shown in a single color.
    /// </summary>
    public bool ShowAsMonochrome
    {
        get => GetValue(ShowAsMonochromeProperty);
        set => SetValue(ShowAsMonochromeProperty, value);
    }
    
    private BitmapIconSource? _bis;
    protected SKBitmap? _bitmap;
    private Size _originalSize;
    private Bitmap? _cachedAvBitmap;
    
    public BitmapIcon()
    {
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.HighQuality);
    }

    ~BitmapIcon()
    {
        Dispose();
        UnlinkFromBitmapIconSource();
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == UriSourceProperty)
        {
            if (_bis != null)
                throw new InvalidOperationException("Cannot edit properties of BitmapIcon if BitmapIconSource is linked");

            CreateBitmap(change.GetNewValue<Uri>());
            InvalidateCachedBitmap();
            InvalidateVisual();
        }
        else if (change.Property == ShowAsMonochromeProperty)
        {
            if (_bis != null)
                throw new InvalidOperationException("Cannot edit properties of BitmapIcon if BitmapIconSource is linked");

            InvalidateCachedBitmap();
            InvalidateVisual();
        }
        else if (change.Property == ForegroundProperty && ShowAsMonochrome)
        {
            InvalidateCachedBitmap();
        }
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        if (_bis != null)
            return _originalSize;

        if (_bitmap == null || UriSource == null)
            return base.MeasureOverride(availableSize);

        return _originalSize;
    }

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        if (_bitmap == null && _bis == null)
            return;

        var dst = new Rect(Bounds.Size);

        if (dst.Width < 1 || dst.Height < 1)
            return;

        var avBitmap = GetCachedAvaloniaBitmap();
        if (avBitmap == null)
            return;

        using (context.PushClip(dst))
        {
            context.DrawImage(avBitmap, new Rect(avBitmap.Size), dst);
        }
    }

    private Bitmap? GetCachedAvaloniaBitmap()
    {
        if (_cachedAvBitmap != null)
            return _cachedAvBitmap;

        if (_bitmap == null)
            return null;

        byte[] pngData;

        if (ShowAsMonochrome)
        {
            var avColor = Foreground is ISolidColorBrush sc ? sc.Color : Colors.White;
            var skColor = new SKColor(avColor.R, avColor.G, avColor.B, avColor.A);

            using var colorFilter = SKColorFilter.CreateBlendMode(skColor, SKBlendMode.SrcATop);
            using var paint = new SKPaint { ColorFilter = colorFilter };

            using var surface = SKSurface.Create(new SKImageInfo(_bitmap.Width, _bitmap.Height, SKColorType.Bgra8888));
            surface.Canvas.Clear(SKColors.Transparent);
            surface.Canvas.DrawBitmap(_bitmap, 0, 0, paint);
            using var snap = surface.Snapshot();
            using var data = snap.Encode(SKEncodedImageFormat.Png, 100);
            pngData = data.ToArray();
        }
        else
        {
            using var image = SKImage.FromBitmap(_bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            pngData = data.ToArray();
        }

        using var ms = new MemoryStream(pngData);
        _cachedAvBitmap = new Bitmap(ms);
        return _cachedAvBitmap;
    }

    private void InvalidateCachedBitmap()
    {
        _cachedAvBitmap?.Dispose();
        _cachedAvBitmap = null;
    }

    private void CreateBitmap(Uri? src)
    {
        if (_bis != null)
            return;

        Dispose();

        if (src == null)
            return;

        if (src.IsAbsoluteUri && src.IsFile)
        {
            _bitmap = SKBitmap.Decode(src.LocalPath);
        }
        else
        {
            _bitmap = SKBitmap.Decode(AssetLoader.Open(src));
        }
        _originalSize = new Size(_bitmap.Width, _bitmap.Height);
    }

    protected void Dispose()
    {
        InvalidateCachedBitmap();
        _bitmap?.Dispose();
        _bitmap = null;
        _originalSize = default;
    }

    internal void LinkToBitmapIconSource(BitmapIconSource bis)
    {
        if (bis == null)
            throw new ArgumentNullException("BitmapIconSource", "BitmapIconSource cannot be null");

        _bis = bis;
        OnLinkedBitmapIconSourceChanged(null, null);
        bis.OnBitmapChanged += OnLinkedBitmapIconSourceChanged;
    }

    internal void UnlinkFromBitmapIconSource()
    {
        if (_bis != null)
            _bis.OnBitmapChanged -= OnLinkedBitmapIconSourceChanged;

        _bis = null;
    }

    private void OnLinkedBitmapIconSourceChanged(object? sender, object? e)
    {
        Dispose();
        if (_bis != null)
        {
            _bitmap = _bis._bitmap;
            _originalSize = _bis.Size;
        }
        InvalidateCachedBitmap();
    }
}
