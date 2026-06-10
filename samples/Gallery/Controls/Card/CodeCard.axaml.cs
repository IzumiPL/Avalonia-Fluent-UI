using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Metadata;
using Gallery.Helpers;

namespace Gallery.Controls;

public class CodeCard : ContentControl
{
    public static readonly StyledProperty<object?> CodeContentProperty =
        AvaloniaProperty.Register<CodeCard, object?>(nameof(CodeContent));

    public static readonly StyledProperty<IDataTemplate?> CodeContentTemplateProperty =
        AvaloniaProperty.Register<CodeCard, IDataTemplate?>(nameof(CodeContentTemplate));

    public static readonly StyledProperty<double> CodeContentHeightProperty =
        AvaloniaProperty.Register<CodeCard, double>(nameof(CodeContentHeight));

    public static readonly StyledProperty<ICommand?> CodeContentCommandProperty =
        AvaloniaProperty.Register<CodeCard, ICommand?>(nameof(CodeContentCommand));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CodeCard, string?>(nameof(Title));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<string> UrlProperty =
        AvaloniaProperty.Register<CodeCard, string>(nameof(Url), defaultValue: "https://github.com/HiyorinI/AvaloniaFluentUI.git");

    public string Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    private Border? _border;
    private const string PART_BORDER = "PART_CodeContentBorder";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _border?.PointerReleased -= OnSourceCodeContentPointerReleased;
        
        _border = e.NameScope.Get<Border>(PART_BORDER);
        
        _border?.PointerReleased += OnSourceCodeContentPointerReleased;
    }

    private void OnSourceCodeContentPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        UrlHelpers.OpenUrl(Url);
    }

    public double CodeContentHeight
    {
        get => GetValue(CodeContentHeightProperty);
        set => SetValue(CodeContentHeightProperty, value);
    }

    [DependsOn(nameof(CodeContentTemplate))]
    public object? CodeContent
    {
        get => GetValue(CodeContentProperty);
        set => SetValue(CodeContentProperty, value);
    }

    public IDataTemplate? CodeContentTemplate
    {
        get => GetValue(CodeContentTemplateProperty);
        set => SetValue(CodeContentTemplateProperty, value);
    }

    public ICommand? CodeContentCommand
    {
        get => GetValue(CodeContentCommandProperty);
        set => SetValue(CodeContentCommandProperty, value);
    }
}
