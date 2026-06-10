using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml;

namespace AvaloniaFluentUI.Locale;

/// <summary>
/// Provides localized string resources from embedded RESX files and supports
/// runtime language switching.
/// </summary>
public class LocalizationService : INotifyPropertyChanged
{
    private static readonly ResourceManager resourceManager =
        new("AvaloniaFluentUI.Locale.Strings",
            typeof(LocalizationService).Assembly);

    public static CultureInfo DefaultCultureInfo { get; } = new("en-US");

    private LocalizationService() { }

    public static LocalizationService Instance { get; } = new();

    /// <summary>
    /// 当前有的翻译语言, 添加自定义语言时可把他添加到此列表
    /// </summary>
    public static HashSet<string> Languages = new HashSet<string>() { "en-US", "zh-CN", "ja-JP" };

    /// <summary>
    /// Raised when the UI culture changes. Bindings should re-read their
    /// localized string properties in response.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the current culture name, e.g. "en-US", "zh-CN", "ja-JP".
    /// </summary>
    public string CurrentLanguage => CultureInfo.CurrentCulture.Name;

    public CultureInfo CurrentCultureInfo => throw new NotImplementedException();

    /// <summary>
    /// Custom string overrides. Keys take the form <c>"ResourceName"</c>
    /// (applies to all cultures) or <c>"cultureCode:ResourceName"</c>
    /// (applies only to that culture, e.g. <c>"fr-FR:SearchText"</c>).
    /// Checked before the embedded RESX resources.
    /// </summary>
    public ConcurrentDictionary<string, string> CustomStrings { get; private set; } = new();

    /// <summary>
    /// 索引器允许通过 <c>Path=[key] 进行 XAML 绑定, 但最好还是切换语言后重启应用</c>.
    /// </summary>
    public string this[string resourceKey] => GetString(resourceKey);

    /// <summary>
    /// Gets a localized string for the specified resource key using the
    /// current UI culture.
    /// </summary>
    public string GetString(string key)
    {
        return GetString(key, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// 添加不同的语言 通过 language:key 添加
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public LocalizationService AddValue(string key, string value)
    {
        CustomStrings[key] = value;
        
        return this;
    }

    /// <summary>
    /// 添加不同的语言 通过 language:key 添加
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="values"></param>
    public void AddValues(IEnumerable<string> keys, IEnumerable<string> values)
    {
        foreach (var (key, value) in keys.Zip(values))
        {
            CustomStrings[key] = value;
        }
    }

    /// <summary>
    /// Gets a localized string for the specified resource key using the
    /// given culture.
    /// </summary>
    public string GetString(string key, CultureInfo culture)
    {
        // 1. Culture-specific custom override (e.g. "fr-FR:SearchText")
        var cultureKey = $"{culture.Name}:{key}";
        if (CustomStrings.TryGetValue(cultureKey, out var val))
            return val;

        // 2. Culture-neutral custom override
        if (CustomStrings.TryGetValue(key, out val))
            return val;

        // 3. Built-in embedded RESX
        string? value;
        try
        {
            value = resourceManager.GetString(key, culture);
        }
        catch (MissingManifestResourceException)
        {
            value = null;
        }
        if (value != null)
            return value;

        // 4. Fallback to default culture
        try
        {
            value = resourceManager.GetString(key, DefaultCultureInfo);
        }
        catch (MissingManifestResourceException)
        {
            value = null;
        }
        if (value != null)
            return value;

        return string.Empty;
    }

    /// <summary>
    /// Loads string entries from a <c>.resx</c> file at runtime and adds them
    /// to <see cref="CustomStrings"/>. If the file name contains a culture
    /// suffix (e.g. <c>Strings.fr-FR.resx</c>), the entries are scoped to that
    /// culture; otherwise they apply to all cultures.
    /// </summary>
    /// <param name="filePath">Path to the <c>.resx</c> file on disk.</param>
    public void LoadResxFile(string filePath)
    {
        var culture = ParseCultureFromFileName(filePath);
        var doc = new XmlDocument();
        doc.Load(filePath);

        foreach (XmlElement dataNode in doc.SelectNodes("/root/data"))
        {
            var name = dataNode.GetAttribute("name");
            if (string.IsNullOrEmpty(name)) continue;

            var value = dataNode.SelectSingleNode("value")?.InnerText;
            if (value == null) continue;

            var key = culture != null ? $"{culture}:{name}" : name;
            CustomStrings[key] = value;
        }
    }

    /// <summary>
    /// Loads all <c>.resx</c> files from the specified directory. See
    /// <see cref="LoadResxFile"/> for the culture-detection convention.
    /// </summary>
    /// <param name="directoryPath">Directory containing <c>.resx</c> files.</param>
    public void LoadResxDirectory(string directoryPath)
    {
        foreach (var file in Directory.GetFiles(directoryPath, "*.resx"))
        {
            LoadResxFile(file);
        }
    }

    /// <summary>
    /// Switches the app's UI culture at runtime. Raises
    /// <see cref="PropertyChanged"/> so that bound UI elements refresh.
    /// </summary>
    /// <param name="language">
    /// A valid culture name, e.g. "en-US", "zh-CN", "ja-JP".
    /// </param>
    public void SetCulture(string language)
    {
        var culture = new CultureInfo(language);
        
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    private static string? ParseCultureFromFileName(string filePath)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);
        var dotIndex = name.LastIndexOf('.');
        if (dotIndex < 0) return null;

        var suffix = name[(dotIndex + 1)..];
        try
        {
            var culture = CultureInfo.GetCultureInfo(suffix);
            if (culture.Name == suffix)
                return suffix;
        }
        catch (CultureNotFoundException)
        {
        }
        return null;
    }
}
