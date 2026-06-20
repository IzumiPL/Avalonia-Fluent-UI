using Avalonia;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;

namespace AvaloniaFluentUI.Controls;

public class ShortcutKeyPanel : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<string>> KeysProperty =
        AvaloniaProperty.Register<ShortcutKeyPanel, IEnumerable<string>>(nameof(Keys));

    public IEnumerable<string> Keys
    {
        get => GetValue(KeysProperty);
        set => SetValue(KeysProperty, value);
    }
}
