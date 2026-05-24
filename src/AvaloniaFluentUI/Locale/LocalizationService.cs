using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace AvaloniaFluentUI.Locale;

/// <summary>
/// Provides localized string resources from embedded RESX files and supports
/// runtime language switching.
/// </summary>
/// <remarks>
/// To add a new language, create a new RESX satellite assembly. To extend
/// strings, add entries to Strings.resx and provide translated values in the
/// satellite .resx files.
/// </remarks>
public class LocalizationService : INotifyPropertyChanged
{
    private static readonly ResourceManager s_resourceManager =
        new("AvaloniaFluentUI.Locale.Strings",
            typeof(LocalizationService).Assembly);

    private LocalizationService() { }

    public static LocalizationService Instance { get; } = new();

    /// <summary>
    /// Raised when the UI culture changes. Bindings should re-read their
    /// localized string properties in response.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets a localized string for the specified resource key using the
    /// current UI culture.
    /// </summary>
    public string GetString(string key)
    {
        return GetString(key, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Gets a localized string for the specified resource key using the
    /// given culture.
    /// </summary>
    public string GetString(string key, CultureInfo culture)
    {
        var value = s_resourceManager.GetString(key, culture);
        return value ?? string.Empty;
    }

    /// <summary>
    /// Switches the app's UI culture at runtime. Raises
    /// <see cref="PropertyChanged"/> so that bound UI elements refresh.
    /// </summary>
    /// <param name="cultureName">
    /// A valid culture name, e.g. "en-US", "zh-CN", "ja-JP".
    /// </param>
    public void SetCulture(string cultureName)
    {
        var culture = new CultureInfo(cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    /// <summary>
    /// Switches the app's UI culture using the <see cref="Language"/> enum.
    /// </summary>
    public void SetLanguage(Language language)
    {
        var cultureName = language switch
        {
            Language.ZH_CN => "zh-CN",
            Language.JA_JP => "ja-JP",
            _ => "en-US",
        };
        SetCulture(cultureName);
    }
}
