using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaFluentUI.Controls;

public class LabelTextBox : TextBox 
{
    public static readonly StyledProperty<string?> PrefixProperty =
        AvaloniaProperty.Register<LabelTextBox, string?>(nameof(Prefix));

    public string? Prefix
    {
        get => GetValue(PrefixProperty);
        set => SetValue(PrefixProperty, value);
    }

    public static readonly StyledProperty<string?> SuffixProperty =
        AvaloniaProperty.Register<LabelTextBox, string?>(nameof(Suffix));

    public string? Suffix
    {
        get => GetValue(SuffixProperty);
        set => SetValue(SuffixProperty, value);
    }
}

