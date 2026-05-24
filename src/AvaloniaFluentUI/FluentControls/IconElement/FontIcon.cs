using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents an icon that uses a glyph from the specified font.
/// </summary>
public partial class FontIcon : FAIconElement
{
    /// <summary>
    /// Defines the <see cref="FontFamily"/> property
    /// </summary>
    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        TextElement.FontFamilyProperty.AddOwner<FontIcon>();

    /// <summary>
    /// Defines the <see cref="FontSize"/> property
    /// </summary>
    public static readonly StyledProperty<double> FontSizeProperty =
        TextElement.FontSizeProperty.AddOwner<FontIcon>();

    /// <summary>
    /// Defines the <see cref="FontWeight"/> property
    /// </summary>
    public static readonly StyledProperty<FontWeight> FontWeightProperty =
        TextElement.FontWeightProperty.AddOwner<FontIcon>();

    /// <summary>
    /// Defines the <see cref="FontStyle"/> property
    /// </summary>
    public static readonly StyledProperty<FontStyle> FontStyleProperty =
        TextElement.FontStyleProperty.AddOwner<FontIcon>();

    /// <summary>
    /// Defines the <see cref="Glyph"/> property
    /// </summary>
    public static readonly StyledProperty<string> GlyphProperty =
        AvaloniaProperty.Register<FontIcon, string>(nameof(Glyph));

    /// <summary>
    /// Gets or sets the <see cref="Avalonia.Media.FontFamily"/> to use when rendering
    /// the glyph
    /// </summary>
    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size to use when rendering the glyph
    /// </summary>
    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Avalonia.Media.FontWeight"/> to use 
    /// when rendering the glyph
    /// </summary>
    public FontWeight FontWeight
    {
        get => GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Avalonia.Media.FontStyle"/> to use 
    /// when rendering the glyph
    /// </summary>
    public FontStyle FontStyle
    {
        get => GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }

    /// <summary>
    /// Gets or sets the glyph this FontIcon renders
    /// </summary>
    public string Glyph
    {
        get => GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextElement.FontSizeProperty ||
            change.Property == TextElement.FontFamilyProperty ||
            change.Property == TextElement.FontWeightProperty ||
            change.Property == TextElement.FontStyleProperty ||
            change.Property == GlyphProperty)
        {
            _textLayout = null;
            InvalidateMeasure();
        }
        else if (change.Property == TextElement.ForegroundProperty)
        {
            _textLayout = null;
            // FAIconElement calls InvalidateVisual
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_textLayout == null)
        {
            GenerateText();
        }

        return new Size(_textLayout.Width, _textLayout.Height);
    }

    public override void Render(DrawingContext context)
    {
        if (_textLayout == null)
            GenerateText();

        var dstRect = new Rect(Bounds.Size);
        using (context.PushClip(dstRect))
        {
            var pt = new Point(dstRect.Center.X - _textLayout.Width * 0.5,
                               dstRect.Center.Y - _textLayout.Height * 0.5);
            _textLayout.Draw(context, pt);
        }
    }

    private void GenerateText()
    {
        _textLayout = new TextLayout(Glyph, new Typeface(FontFamily, FontStyle, FontWeight),
           FontSize, Foreground, TextAlignment.Left);
    }

    private TextLayout _textLayout;
}
