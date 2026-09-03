using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Icons;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Styling;
using AvaloniaFluentUI.Windowing;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages;
using Gallery.Messages.MainWindowMessages;
using Gallery.Models;
using Gallery.Services;
using Gallery.ViewModels;

namespace Gallery.Views;

public class MainWindowSplashScreen : IApplicationSplashScreen
{
    public object SplashScreenContent => new Image
    {
        Source = new Bitmap(AssetLoader.Open(new Uri("avares://Gallery/Assets/app.ico"))),
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 128,
        Height = 128
    };
    public async Task RunTasks(CancellationToken cancellationToken)
    {
        await Task.Delay(1500, cancellationToken);
        Action.Invoke();
    }
    
    public Action Action { get; }

    public MainWindowSplashScreen(Action action)
    {
        Action = action;
    }

    public int MinimumShowTime => 1500;
}

public partial class MainWindow : FluentWindow
{
    private Bitmap? _backgroundImage;
    
    public MainWindow()
    {
        Application.Current?.Resources["NavigationViewContentMargin"] = new Thickness(0, 55, 0, 0);
        TitleBarIsVisible = false;
        SplashScreen = new MainWindowSplashScreen(() => TitleBarIsVisible = true);
        InitializeComponent();
        
        RegisterMessages();
        Loaded += OnLoaded;
        
        KeyBindings.Add(
            new KeyBinding
            {
                Gesture = new KeyGesture(Key.F, KeyModifiers.Control),
                Command = new RelayCommand(() => AutoCompleteBox.Focus())
            }
            );
        
        ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("Pin"));
        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (PinButton.Tag!.ToString() == "isTopmost")
        {
            ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("UnPin"));
        }
        else
        {
            ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("Pin"));
        }
    }


    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<JumpToControlMessage>(this, OnJumpToControl);
        WeakReferenceMessenger.Default.Register<EnabledWindowEffectMessage>(this, OnEnabledWindowEffect);
        WeakReferenceMessenger.Default.Register<EnabledBackgroundImageMessage>(this, OnEnabledBackgroundImage);
    }

    private Bitmap LoadImageResource()
    {
        return Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/bg.jpg")), 1024);
    }

    private void OnEnabledBackgroundImage(object recipient, EnabledBackgroundImageMessage message)
    {
        if (message.IsVisible)
        {
            if (message.Path != null)
            {
                using var stream = File.OpenRead(message.Path);
                _backgroundImage = Bitmap.DecodeToHeight(stream, 1024);
                BackgroundImage.Source = _backgroundImage;
                
                EnabledAcrylicBlue(false);
                EnabledMica(false);
            }
            else if (_backgroundImage == null)
            {
                _backgroundImage = LoadImageResource();
                BackgroundImage.Source = _backgroundImage;
                
                EnabledAcrylicBlue(false); 
                EnabledMica(false);
            }
        }
        else
        {
            BackgroundImage.Source = null;
            _backgroundImage?.Dispose();
            _backgroundImage = null;
        }
        
        BackgroundImage.IsVisible = message.IsVisible;
    }

    private void OnEnabledWindowEffect(object recipient, EnabledWindowEffectMessage message)
    {
        if (message.IsEnabled)
        {
            switch (message.type)
            {
                case "Mica":
                    EnabledMica(true);
                    break;
                case "Acrylic":
                    EnabledAcrylicBlue(true);
                    break;
            }
            return;
        }
        EnabledAcrylicBlue(false);
        EnabledMica(false);
    }

    private NavigationViewItem? FindNavigationItem(IList<object> items, string tag)
    {
        foreach (var item in items)
        {
            if (item is NavigationViewItem nvi)
            {
                if (nvi.Tag?.ToString() == tag)
                    return nvi;

                if (nvi.MenuItems?.Count > 0)
                {
                    var found = FindNavigationItem(nvi.MenuItems, tag);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
        }
        return null;
    }

    private void OnJumpToControl(object recipient, JumpToControlMessage message)
    {
        var nvi = FindNavigationItem(NavigationView.MenuItems, message.Page);
        if (nvi != null)
        {
            NavigationView.SelectedItem = nvi;
            nvi.BringIntoView();
        }
    }

    private void SaveConfig()
    {
        try
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                var svm = viewModel.SettingsViewModel;
                var config = new AppConfig
                {
                    IsCustomAccentColor = svm.IsCustomColor,
                    IsFollowSystemAccentColor = svm.IsFollowSystemAccentColor,
                    Theme = AvaloniaFluentTheme.Instance.CurrentTheme.ToString(),
                    IsWindowEffectEnabled = svm.IsEnabledWindowEffect,
                    WindowEffect = svm.CurrentEffect,
                    IsEnabledBackgroundImage = svm.IsEnabledBackgroundImage,
                    Language = svm.CurrentLanguage,
                    BackgroundImagePath = svm.BackgroundImagePath
                };
                if (svm.IsCustomColor)
                {
                    config.CustomAccentColor = svm.SelectedAccentColor.ToString();
                }
                ConfigService.SaveConfig(config);
                
#if DEBUG
                Debug.WriteLine("Save Config Success");
#endif
            }
        }
        catch (Exception e)
        {
#if DEBUG 
            Debug.WriteLine(e);
#endif
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        SaveConfig();
        base.OnClosing(e);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            bool visible = viewModel.SettingsViewModel.IsEnabledBackgroundImage;
            BackgroundImage.IsVisible = visible;
            
            if (visible)
            {
                var path = viewModel.SettingsViewModel.BackgroundImagePath;
                if (path != null)
                {
                    using var stream = File.OpenRead(path);
                    _backgroundImage = Bitmap.DecodeToHeight(stream, 1080);
                }
                else
                {
                    _backgroundImage = LoadImageResource();
                }

                BackgroundImage.Source = _backgroundImage;
            }

            SettingButton.IsEnabled = !IsWindows11;
            if (!IsWindows11)
            {
                Bind(BorderThicknessProperty, new Binding(nameof(viewModel.BorderWidth)));

                AvaloniaFluentTheme.Instance.ThemeChanged += (_, theme) =>
                {
                    if (BorderBrush == Brushes.Transparent)
                    {
                        BorderBrush = Brush.Parse(AvaloniaFluentTheme.Instance.IsDarkTheme ? "#484848" : "#D6D6D6");
                    }
                };
            }
        }
        
        InitializeInfoBarHost();
    }

    private void InitializeInfoBarHost()
    {
        InfoBarHost.RegisterManager<PopupInfoBarManager>();
        InfoBarHost.RegisterManager<ToastInfoBarManager>();
        InfoBarService.PopupInfoBarManager = InfoBarHost.GetManager<PopupInfoBarManager>();
        InfoBarService.ToastInfoBarManager = InfoBarHost.GetManager<ToastInfoBarManager>();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (Topmost && change.Property == WindowStateProperty)
        {
            Topmost = false;
            Topmost = true;
        }
    }

    private void OnToggleTopmost(object? sender, RoutedEventArgs e)
    {
        if (sender is ToolButton btn)
        {
            if (btn.Tag!.ToString() == "isTopmost")
            {
                btn.Tag = "noTopmost";
                btn.Content= FluentIcon.Pin;
                this.Topmost = false;
                ToolTip.SetTip(btn, LocalizationService.Instance.GetString("Pin"));
            }
            else
            {
                btn.Tag = "isTopmost";
                btn.Content = FluentIcon.Unpin;
                this.Topmost = true;
                ToolTip.SetTip(btn, LocalizationService.Instance.GetString("UnPin"));
            }
        }
    }
    
    private void OnPopupAvatarFlyout(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Avatar ct)
        {
            FlyoutBase.ShowAttachedFlyout(ct);
        }
    }

    private void OnPopupContextMenu(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Panel panel)
        {
            panel.ContextMenu?.Open();
        }
    }

    private void OnHideFlyout(object? sender, RoutedEventArgs routedEventArgs)
    {
        SettingButton.Flyout?.IsOpen = false;
    }

    private void OnBorderColorChanged(object? sender, Color c)
    {
        if (!IsWindows11)
        {
            BorderBrush = new SolidColorBrush(c);
        }
    }

    private void OnAutoCompleteBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel mv && e.AddedItems.Count > 0)
        {
            var page = e.AddedItems[0];
            mv.TogglePageCommand?.Execute(page);
            OnJumpToControl(null, new JumpToControlMessage(page.ToString(), null));
        }
    }
}
