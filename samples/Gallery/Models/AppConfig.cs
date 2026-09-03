using System.Text.Json.Serialization;

namespace Gallery.Models;

public class AppConfig
{
    public string Theme { get; set; } = string.Empty;
    public bool IsCustomAccentColor { get; set; }
    public bool IsFollowSystemAccentColor { get; set; }
    public string CustomAccentColor { get; set; } = string.Empty;
    public bool IsWindowEffectEnabled { get; set; }
    public string WindowEffect { get; set; } = string.Empty;
    public bool IsEnabledBackgroundImage { get; set; }
    public string Language { get; set; } = string.Empty;
    public string? BackgroundImagePath { get; set; }
}

[JsonSerializable(typeof(AppConfig))]
public partial class ConfigJsonContext : JsonSerializerContext
{
}
