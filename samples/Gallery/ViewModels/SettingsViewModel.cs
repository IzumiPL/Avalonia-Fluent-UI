using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages.MainWindowMessages;
using Gallery.Models;
using Gallery.Services;

namespace Gallery.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel(AppConfig? config)
    {
#if DEBUG
        Debug.WriteLine("SettingsViewModel Init");
#endif
        ThemeService.ThemeChanged += OnThemeChanged;

        LoadSetting(config);
        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SettingsTitle));
        OnPropertyChanged(nameof(WindowEffect));
        OnPropertyChanged(nameof(WindowEffectDescription));
        OnPropertyChanged(nameof(AppearanceDescription));
        OnPropertyChanged(nameof(AppThemeDescription));
        OnPropertyChanged(nameof(ThemeColor));
        OnPropertyChanged(nameof(ThemeColorDescription));
        OnPropertyChanged(nameof(CustomColor));
        OnPropertyChanged(nameof(DefaultColor));
        OnPropertyChanged(nameof(SelectColor));
        OnPropertyChanged(nameof(PinToTop));
        OnPropertyChanged(nameof(PinToTopDescription));
        OnPropertyChanged(nameof(Language));
        OnPropertyChanged(nameof(LanguageDescription));
        OnPropertyChanged(nameof(BackgroundImage));
        OnPropertyChanged(nameof(EnableBackgroundImage));
        OnPropertyChanged(nameof(Light));
        OnPropertyChanged(nameof(Dark));
        OnPropertyChanged(nameof(FollowSystem));
    }

    private void LoadSetting (AppConfig? config)
    {
        if (config != null)
        {
            IsEnabledWindowEffect = config.IsWindowEffectEnabled;
            IsEnabledBackgroundImage = config.IsEnabledBackgroundImage;
            CurrentLanguage =  config.Language;
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
            ThemeService.SetAccentColor(Colors.DeepSkyBlue);
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

    public string[] Languages => ["en-US", "zh-CN", "ja-JP"];

    // Localized string properties
    public string SettingsTitle => LocalizationService.Instance.GetString("Settings");
    public string WindowEffect => LocalizationService.Instance.GetString("SV_WindowEffect");
    public string WindowEffectDescription => LocalizationService.Instance.GetString("SV_WindowEffectDescription");
    public string AppearanceDescription => LocalizationService.Instance.GetString("SV_Appearance");
    public string AppThemeDescription => LocalizationService.Instance.GetString("SV_AppTheme");
    public string ThemeColor => LocalizationService.Instance.GetString("SV_ThemeColor");
    public string ThemeColorDescription => LocalizationService.Instance.GetString("SV_ThemeColorDescription");
    public string CustomColor => LocalizationService.Instance.GetString("SV_CustomColor");
    public string DefaultColor => LocalizationService.Instance.GetString("SV_DefaultColor");
    public string SelectColor => LocalizationService.Instance.GetString("SV_SelectColor");
    public string PinToTop => LocalizationService.Instance.GetString("SV_PinToTop");
    public string PinToTopDescription => LocalizationService.Instance.GetString("SV_PinToTopDescription");
    public string Language => LocalizationService.Instance.GetString("SV_Language");
    public string LanguageDescription => LocalizationService.Instance.GetString("SV_LanguageDescription");
    public string BackgroundImage => LocalizationService.Instance.GetString("SV_BackgroundImage");
    public string EnableBackgroundImage => LocalizationService.Instance.GetString("SV_EnableBackgroundImage");
    public string Light => LocalizationService.Instance.GetString("LV_Light");
    public string Dark => LocalizationService.Instance.GetString("LV_Dark");
    public string FollowSystem => LocalizationService.Instance.GetString("LV_FollowSystem");

    [ObservableProperty]
    private string _currentLanguage;

    partial void OnCurrentLanguageChanged(string value)
    {
        LocalizationService.Instance.SetCulture(value);
    }

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
