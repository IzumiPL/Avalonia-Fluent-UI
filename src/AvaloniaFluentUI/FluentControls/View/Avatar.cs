using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;

namespace AvaloniaFluentUI.Controls;

public class Avatar : Control
{
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<Avatar, IImage?>(nameof(Source));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<Avatar, string?>(nameof(Text));

    public static readonly StyledProperty<IBrush> BackgroundProperty =
        AvaloniaProperty.Register<Avatar, IBrush>(nameof(Background));

    public static readonly StyledProperty<IBrush> ForegroundProperty =
        AvaloniaProperty.Register<Avatar, IBrush>(nameof(Foreground));

    public static readonly StyledProperty<BitmapInterpolationMode> RenderModeProperty =
        AvaloniaProperty.Register<Avatar, BitmapInterpolationMode>(nameof(RenderMode), defaultValue: BitmapInterpolationMode.HighQuality);

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<Avatar, CornerRadius>(nameof(CornerRadius));

    public static readonly StyledProperty<bool> IsCircularProperty =
        AvaloniaProperty.Register<Avatar, bool>(nameof(IsCircular));

    public bool IsCircular
    {
        get => GetValue(IsCircularProperty);
        set => SetValue(IsCircularProperty, value);
    }

    static Avatar()
    {
        AffectsRender<Avatar>(SourceProperty, TextProperty, BackgroundProperty, ForegroundProperty, CornerRadiusProperty, RenderModeProperty, IsCircularProperty);
    }

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
    
    public IBrush Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }
    
    public IBrush Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public BitmapInterpolationMode RenderMode
    {
        get => GetValue(RenderModeProperty);
        set => SetValue(RenderModeProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        
        var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        
        if (!(rect.Width > 0) || !(rect.Height > 0)) { return; }

        var rc = IsCircular ? new RoundedRect(rect, rect.Width / 2, rect.Height / 2) : new RoundedRect(rect, CornerRadius);
        context.DrawRectangle(Background, null, rc);

        if (Source != null)
        {
            using (context.PushClip(rc))
            using (context.PushRenderOptions(new RenderOptions { BitmapInterpolationMode = RenderMode }))
            {
                var pixelSize = Source.Size;
                context.DrawImage(Source, new Rect(0, 0, pixelSize.Width, pixelSize.Height), rect);
            }

            return;
        }

        if (!string.IsNullOrWhiteSpace(Text))
        {
            string first = Text.Trim()[0].ToString();
            double fs = Math.Min(rect.Width, rect.Height) * 0.45;

            if (fs <= 0) { fs = 14; }

            var layout = new TextLayout(
                text: first,
                typeface: Typeface.Default,
                fontSize: fs,
                foreground: Foreground,
                maxWidth: rect.Width
                );
            
            layout.Draw(context, new Point(rect.X + (rect.Width - layout.Width) / 2, rect.Y + (rect.Height - layout.Height) / 2));
        }
    }}
