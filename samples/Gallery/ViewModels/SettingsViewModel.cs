using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages.MainWindowMessages;
using Gallery.Services;

namespace Gallery.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel()
    {
#if DEBUG
        Debug.WriteLine("SettingsViewModel Init");
#endif
        ThemeService.ThemeChanged += OnThemeChanged;
        _=InitAsync();
    }

    private async Task InitAsync()
    {
        var config = await ThemeService.LoadConfig();
        if (config != null)
        {
            // await Task.Delay(200);
            IsEnabledWindowEffect = config.IsWindowEffectEnabled;
            IsEnabledBackgroundImage = config.IsEnabledBackgroundImage;
        }
    }

    private void OnThemeChanged(ThemeVariant variant)
    {
        OnPropertyChanged(nameof(IsDarkTheme));
        OnPropertyChanged(nameof(IsAutoTheme));
    }
    
    [ObservableProperty] 
    private string[] _animationTypes = ["Null", "Crossfade", "PageSlide", "Rotate3DTransition"];

    [ObservableProperty]
    private long[] _animationDurations = [50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000];

    [ObservableProperty]
    private PageSlide.SlideAxis[] _animationSlideAxisItems = [PageSlide.SlideAxis.Horizontal, PageSlide.SlideAxis.Vertical];

    [ObservableProperty]
    private bool _isEnabledTogglePageAnimation = true;

    [ObservableProperty] 
    private bool _isTopmost;

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(AnimationSlideAxisBoxIsVisible))]
    private string _animationType = "PageSlide";

    [ObservableProperty]
    private long _animationDuration = 200;

    public bool AnimationSlideAxisBoxIsVisible => AnimationType == "PageSlide" || AnimationType == "Rotate3DTransition";

    [ObservableProperty]
    private bool _isDefaultAccentColor = true;

    partial void OnIsDefaultAccentColorChanged(bool value)
    {
        if (value)
        {
            ThemeService.SetAccentColor(Colors.DeepPink);
        }
    }

    [ObservableProperty]
    private Color _selectedAccentColor = Colors.Transparent;

    partial void OnSelectedAccentColorChanged(Color value)
    {
        ThemeService.SetAccentColor(value);
    }

    [RelayCommand]
    private void ToggleTheme(string value)
    {
        switch (value)
        {
            case "Light": 
                ThemeService.SetTheme(ThemeVariant.Light);
                break;
            case "Dark": 
                ThemeService.SetTheme(ThemeVariant.Dark);
                break;
            case "Auto": 
                ThemeService.SetTheme(ThemeVariant.Default);
                break;
        }
    }

    // public bool IsLightTheme => Application.Current?.RequestedThemeVariant == ThemeVariant.Light;
    public bool IsDarkTheme => ThemeService.IsDarkTheme();
    public bool IsAutoTheme => Application.Current?.RequestedThemeVariant == ThemeVariant.Default;
    
    [ObservableProperty]
    private PageSlide.SlideAxis _animationSlideAxis = PageSlide.SlideAxis.Horizontal;
    
    [ObservableProperty]
    private bool _isEnabledWindowEffect = true;

    public bool WindowEffectCardIsEnabled => !IsEnabledBackgroundImage && RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    [ObservableProperty]
    private bool _isCustomColor;

    partial void OnIsCustomColorChanged(bool value)
    {
        if (value)
        {
            ThemeService.SetAccentColor(SelectedAccentColor);
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WindowEffectCardIsEnabled))]
    private bool _isEnabledBackgroundImage;

    partial void OnIsEnabledTogglePageAnimationChanged(bool value) => WeakReferenceMessenger.Default.Send(new PageAnimationStatusChangedMessage(value));

    partial void OnIsTopmostChanged(bool value) => WeakReferenceMessenger.Default.Send(new WindowTopmostStatusMessage(value));

    partial void OnAnimationTypeChanged(string value) => SendAnimationMessage();
    partial void OnAnimationDurationChanged(long value) => SendAnimationMessage();
    partial void OnAnimationSlideAxisChanged(PageSlide.SlideAxis value) => SendAnimationMessage();

    private void SendAnimationMessage()
    {
        WeakReferenceMessenger.Default.Send(new PageAnimationTypeChangedMessage(AnimationType, AnimationDuration, AnimationSlideAxis));
    }

    partial void OnIsEnabledWindowEffectChanged(bool value) => WeakReferenceMessenger.Default.Send(new EnabledWindowEffectMessage(value));

    partial void OnIsEnabledBackgroundImageChanged(bool value)
    {
        if (value) { IsEnabledWindowEffect = false;}
        WeakReferenceMessenger.Default.Send(new EnabledBackgroundImageMessage(value));
    }
}
