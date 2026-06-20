using System.ComponentModel;
using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class ListPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("List");

    public string[] ItemSource =>
    [
        "Lost in the Wind", "Shining Stars", "Dream of Tomorrow", "Ocean Whisper", "Lonely Road", "Dancing Shadows",
        "Moonlight Journey", "Silent Tears", "Endless Summer", "Midnight Echo", "Wings of Freedom", "Crystal Sky",
        "Burning Heart", "Falling Snow", "Golden Horizon", "Echoes of Time", "Rising Flame", "Secret Garden",
        "Stormy Night", "Peaceful Dawn"
    ];
}
