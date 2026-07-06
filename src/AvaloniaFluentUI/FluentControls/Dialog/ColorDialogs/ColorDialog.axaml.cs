using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Media;
using AvaloniaFluentUI.Locale;

namespace AvaloniaFluentUI.Controls;

/// <summary>
///     Colour picker dialog. Inherits <see cref="ContentDialog" /> so it can be
///     shown directly. The body hosts a <see cref="FluentColorView" />.
/// </summary>
public class ColorDialog : ContentDialog
{
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<ColorDialog, Color>(nameof(Color));

    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    
    public ColorDialog()
    {
        ColorView = new FluentColorView();

        DefaultButton = ContentDialogButton.Primary;
        Content = ColorView;

        SetText();
        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object sender, PropertyChangedEventArgs e)
    {
        SetText();
    }

    private void SetText()
    {
        Title = LocalizationService.Instance.GetString("ChooseColor");
        PrimaryButtonText = LocalizationService.Instance.GetString("Confirm");
        CloseButtonText = LocalizationService.Instance.GetString("Cancel");
    }

    protected override Type StyleKeyOverride => typeof(ContentDialog);

    /// <summary>
    ///     Raised when the user clicks Confirm, providing the final picked colour.
    /// </summary>
    public event EventHandler<Color>? ColorChanged;

    public FluentColorView ColorView { get; }
    
    protected override void OnPrimaryButtonClick(ContentDialogButtonClickEventArgs args)
    {
        base.OnPrimaryButtonClick(args);

        if (Color != ColorView.Color)
        {
            Color = ColorView.Color;
            ColorChanged?.Invoke(this, Color);
        }
    }

    protected override void OnOpening()
    {
        base.OnOpening();
        
        ColorView.Color = Color;
        ColorView.OriginalColor = new SolidColorBrush(Color);
        ColorView.NewColor = new SolidColorBrush(Color);
    }
}
