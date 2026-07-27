using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// 带前后标签的输入框
/// </summary>
public class LabelTextBox : TextBox 
{
    public static readonly StyledProperty<string?> PrefixProperty =
        AvaloniaProperty.Register<LabelTextBox, string?>(nameof(Prefix));

    public static readonly StyledProperty<string?> SuffixProperty =
        AvaloniaProperty.Register<LabelTextBox, string?>(nameof(Suffix));
    
    /// <summary>
    /// 设置或获取前缀
    /// </summary>
    public string? Prefix
    {
        get => GetValue(PrefixProperty);
        set => SetValue(PrefixProperty, value);
    }

    /// <summary>
    /// 设置或获取后缀
    /// </summary>
    public string? Suffix
    {
        get => GetValue(SuffixProperty);
        set => SetValue(SuffixProperty, value);
    }
}

