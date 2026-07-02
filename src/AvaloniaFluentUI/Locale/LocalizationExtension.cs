using System;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace AvaloniaFluentUI.Locale;

public class LocalizationExtension(string key) : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding { Source = LocalizationService.Instance, Path = $"[{key}]" };
    }
}
