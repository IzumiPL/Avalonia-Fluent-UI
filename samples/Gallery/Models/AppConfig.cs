using System.Text.Json.Serialization;

namespace Gallery.Models;

public class AppConfig
{
    public string Theme { get; set; } = "";
    public bool IsCustomAccentColor { get; set; }
    public string CustomAccentColor { get; set; } = "";
    public bool IsWindowEffectEnabled { get; set; }
    public string WindowEffect { get; set; } = "";
    public bool IsEnabledBackgroundImage { get; set; }
    public string Language { get; set; } = "";
}

[JsonSerializable(typeof(AppConfig))]
public partial class ConfigJsonContext : JsonSerializerContext
{
}
