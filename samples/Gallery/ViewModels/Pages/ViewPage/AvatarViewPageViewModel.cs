using System.ComponentModel;
using Avalonia;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using Gallery.Extensions;

namespace Gallery.ViewModels;

public partial class AvatarViewPageViewModel : ViewModelBase
{ 
    public override string Title => LocalizationService.Instance.GetString("Avatar");

    public double[] AvatarHeights => [16, 18, 24, 32, 48, 64, 72, 96, 128];
    public double[] AvatarWidths => [16, 18, 24, 32, 48, 64, 72, 96, 128];

    public CornerRadius AvatarRadius => new CornerRadius(AvatarRadiusText.ToDoubleOrZero());

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvatarRadius))]
    public string _avatarRadiusText;

    [ObservableProperty]
    private double _avatarHeight = 64.0;

    [ObservableProperty]
    private double _avatarWidth = 64.0; 

    [ObservableProperty]
    private bool _avatarIsCircular = true;

    public AvatarViewPageViewModel()
    {
        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Title));
    }
}
