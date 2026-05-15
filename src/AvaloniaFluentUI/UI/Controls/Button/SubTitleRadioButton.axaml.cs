using Avalonia;
using Avalonia.Controls;

namespace AvaloniaFluentUI.UI.Controls;

public class SubTitleRadioButton : RadioButton 
{
    public static readonly StyledProperty<string> SubTitleProperty =
        AvaloniaProperty.Register<SubTitleRadioButton, string>(nameof(SubTitle));

    public string SubTitle
    {
        get => GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }
}
