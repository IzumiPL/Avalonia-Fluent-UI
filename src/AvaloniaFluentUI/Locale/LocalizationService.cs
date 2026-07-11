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
    private readonly List<ResourceManager> _resourceManagers = new();

    public static CultureInfo DefaultCultureInfo { get; } = new("en-US");

    private LocalizationService()
    {
        // 添加默认资源管理
        _resourceManagers.Add(new ResourceManager("AvaloniaFluentUI.Locale.Strings", typeof(LocalizationService).Assembly));
    }
    
    /// <summary>
    /// 添加resx到资源管理
    /// </summary>
    public void AddResourceManager(ResourceManager resourceManager)
    {
        ArgumentNullException.ThrowIfNull(resourceManager);

        if (_resourceManagers.Contains(resourceManager))
            return;
        
        // 默认新添加的为第一个查找的
        _resourceManagers.Insert(0, resourceManager);

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    // 全局唯一实例
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
    public string CurrentLanguage => CultureInfo.CurrentUICulture.Name;

    public CultureInfo CurrentCultureInfo => CultureInfo.CurrentUICulture;

    /// <summary>
    /// 添加的自定义的值, 默认查找优先级最高
    /// </summary>
    private readonly ConcurrentDictionary<string, string> _customStrings  = new();

    /// <summary>
    /// Entries loaded from disk <c>.resx</c> files for the current culture only.
    /// Cleared and reloaded when the culture changes.
    /// </summary>
    private readonly ConcurrentDictionary<string, string> _resourceEntries = new();

    /// <summary>
    /// Directory path passed to <see cref="LoadResxDirectory"/> last time,
    /// so we can auto-reload on culture switch. <c>null</c> if never loaded.
    /// </summary>
    private string? _loadedResxDirectory;

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
        return GetString(key, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// 添加不同的语言 通过 language:key 添加
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public LocalizationService AddValue(string key, string value)
    {
        _customStrings[key] = value;
        
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
            _customStrings[key] = value;
        }
    }

    /// <summary>
    /// 添加同一种指定的语言
    /// </summary>
    /// <param name="language"></param>
    /// <param name="keys"></param>
    /// <param name="values"></param>
    public void AddValuesWithLanguage(string language, IEnumerable<string> keys, IEnumerable<string> values)
    {
        foreach (var (key, value) in keys.Zip(values))
        {
            _customStrings[$"{language}:{key}"] = value;
        }
    }

    /// <summary>
    /// Gets a localized string for the specified resource key using the
    /// given culture.
    /// </summary>
    public string GetString(string key, CultureInfo culture)
    {
        // 1.针对特定文化的自定义覆盖（例如“fr-FR：SearchText”）
        var cultureKey = $"{culture.Name}:{key}";
        if (_customStrings.TryGetValue(cultureKey, out var val))
            return val;

        if (_customStrings.TryGetValue(key, out val))
            return val;

        // 3.磁盘 .resx 文件中的条目（仅限当前区域性）
        if (_resourceEntries.TryGetValue(key, out val))
            return val;

        // 4. 嵌入式资源管理器
        foreach (var manager in _resourceManagers)
        {
            try
            {
                var value = manager.GetString(key, culture);

                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            catch (MissingManifestResourceException)
            {
            }
        }

        // 5.以上都未找到就查找默认语言的 
        foreach (var manager in _resourceManagers)
        {
            try
            {
                var value = manager.GetString(key, DefaultCultureInfo);

                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            catch (MissingManifestResourceException)
            {
            }
        }

        // 全都未找到返回空
        return string.Empty;
    }

    /// <summary>
    /// 在Android下使用LoadResxFile和LoadResxDirectory可能会加载不到
    /// 运行时从 <c>.resx</c> 文件加载字符串条目到
    /// <see cref="_resourceEntries"/>, 覆盖之前的任何条目
    /// </summary>
    /// <param name="filePath">Path to the <c>.resx</c> file on disk.</param>
    public void LoadResxFile(string filePath)
    {
        var culture = ParseCultureFromFileName(filePath);

        // Skip if this file targets a different culture than the current one
        if (culture != null &&
            !culture.Equals(CultureInfo.CurrentUICulture.Name, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var doc = new XmlDocument();
        doc.Load(filePath);

        foreach (XmlElement dataNode in doc.SelectNodes("/root/data"))
        {
            var name = dataNode.GetAttribute("name");
            if (string.IsNullOrEmpty(name)) continue;

            var value = dataNode.SelectSingleNode("value")?.InnerText;
            if (value == null) continue;

            _resourceEntries[name] = value;
        }
    }

    /// <summary>
    /// Loads <c>.resx</c> files from the specified directory that match the
    /// current UI culture. Called automatically by <see cref="SetCulture"/>
    /// when a directory was previously loaded.
    /// </summary>
    /// <param name="directoryPath">Directory containing <c>.resx</c> files.</param>
    public void LoadResxDirectory(string directoryPath)
    {
        _resourceEntries.Clear();
        _loadedResxDirectory = directoryPath;

        var currentCulture = CultureInfo.CurrentUICulture.Name;

        foreach (var file in Directory.GetFiles(directoryPath, "*.resx"))
        {
            var culture = ParseCultureFromFileName(file);
            // Load if: culture-neutral (e.g. Strings.resx) OR matches current culture
            if (culture == null || culture.Equals(currentCulture, StringComparison.OrdinalIgnoreCase))
            {
                LoadResxFile(file);
            }
        }
    }

    /// <summary>
    /// 设置当前的文化, 会触发属性更改, 默认不做更改判断
    /// </summary>
    /// <param name="language"></param>
    public void SetCulture(string language)
    {
        var culture = new CultureInfo(language);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        // 重新加载磁盘 .resx 文件以获取新文化（旧条目自动清除）
        if (_loadedResxDirectory != null)
        {
            LoadResxDirectory(_loadedResxDirectory);
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    public static string? ParseCultureFromFileName(string filePath)
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
