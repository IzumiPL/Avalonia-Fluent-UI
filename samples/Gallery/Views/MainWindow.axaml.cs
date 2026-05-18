using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaFluentUI.UI.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages;
using Gallery.Models;
using Gallery.Services;
using Gallery.ViewModels;

namespace Gallery.Views;

public partial class MainWindow : Window
{
    private IPageTransition? _currentPageTransition;

    private bool _isEnabledWindowEffect;
    // private IPageTransition? _currentPageTransition = new PageSlide { Orientation = PageSlide.SlideAxis.Horizontal, Duration = TimeSpan.FromMilliseconds(250) };
    private readonly MainWindowMessageHandler _messageHandler;

    public MainWindow()
    {
#if DEBUG
        Debug.WriteLine("MainWindow Init");
#endif

        InitializeComponent();
        
         _messageHandler = new MainWindowMessageHandler(this);
         RegisterAllMessage();
         // Set Background Image
         BackgroundImage.Source = Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/bg.jpg")), 1024);
         
        // Set Default Page Toggled Animation
        SetPageTransition(null);

        Loaded += OnLoaded;
        ThemeService.ThemeChanged += _ => { EnableWindowEffect(_isEnabledWindowEffect); };
        
        WeakReferenceMessenger.Default.Register<JumpToControlMessage>(this, OnJumpToControl);
    }

    private void OnJumpToControl(object recipient, JumpToControlMessage message)
    {
        foreach (var item in NavigationView.MenuItems)
        {
            if (item is NavigationViewItem nvi)
            {
                if (nvi.Tag!.ToString() == message.Page)
                {
                    NavigationView.SelectedItem = nvi;
                }
            }
        } 
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            var svm = viewModel.SettingsViewModel;
            try
            {
                ThemeService.SaveConfig(new AppConfig
                {
                    AccentColor = svm.IsDefaultAccentColor ? ThemeService.FluentTheme?.CustomAccentColor.ToString() : svm.SelectedAccentColor.ToString(),
                    Theme = Application.Current?.RequestedThemeVariant.ToString(),
                    IsWindowEffectEnabled = svm.IsEnabledWindowEffect,
                    IsEnabledBackgroundImage = svm.IsEnabledBackgroundImage
                });
                Console.WriteLine("Save Config Success");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        else { Console.WriteLine("Save Config Error");}
        base.OnClosing(e);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        NavigationView.SettingsItem.Tag = "Settings";

        if (DataContext is MainWindowViewModel viewModel)
        {
            var svm = viewModel.SettingsViewModel;
            EnableWindowEffect(svm.IsEnabledWindowEffect);
            BackgroundImage.IsVisible = svm.IsEnabledBackgroundImage;
            // viewModel.ViewModelChangedEvent += OnViewModelChanged;
            // TransitioningContentControlPanel.PageTransition = new PageSlide
            // {
                // Orientation = svm.AnimationSlideAxis, 
                // SlideInEasing = new CubicEaseIn(),
                // Duration = TimeSpan.FromMilliseconds(svm.AnimationDuration)
            // };
        }
    }

    private void OnViewModelChanged(string page, string name)
    {
        foreach (var item in NavigationView.MenuItems)
        {
            if (item is NavigationViewItem nvi)
            {
                if ((string)nvi.Tag! == page) NavigationView.SelectedItem = nvi;

                WeakReferenceMessenger.Default.Send(new JumpToControlMessage(page, name));
            }
        }
    }

    private void RegisterAllMessage() => _messageHandler.RegisterAllMessage();
    
    private void InitControls()
    {
        // Apply Icon
        // ReturnButton.PathData = StreamGeometry.Parse(FluentIcon.Return);

        // Set Default Navigation Item
        // NavigationViewPanel.SetCurrentItemByContent("Home");
    }

    public void SetPageTransition(IPageTransition? pageTransition)
    {
        // TransitioningContentControlPanel.PageTransition = pageTransition;
        _currentPageTransition = pageTransition;
    }

    public IPageTransition? CurrentPageTransition => _currentPageTransition;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        InitControls();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (Topmost && change.Property == WindowStateProperty)
        {
            Topmost = false;
            Topmost = true;

            if (change.GetNewValue<WindowState>() == WindowState.Maximized)
            {
                NavigationView.Margin = new Thickness(12, 8, 8, 6);
            }
            else
            {
                NavigationView.Margin = new Thickness(6, 0, 0, 0);
            }
        }
    }

    public void SetBackgroundImageIsVisible(bool visible)
    {
        BackgroundImage.IsVisible = visible;
    }
    
    /// <summary>
    ///     Enable Window Effect
    /// </summary>
    public void EnableWindowEffect(bool enable = true)
    {
        if (enable)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (Environment.OSVersion.Version.Build >= 22000)
                {
                    Background = Brushes.Transparent;
                    TransparencyLevelHint = [WindowTransparencyLevel.Mica];
                }
                else
                {
                    Background = Brush.Parse(ThemeService.IsDarkTheme() ? "#A1000000" : "#C1FFFFFF");
                    TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                }
            }
        }
        else
        {
#if DEBUG
            Debug.WriteLine("Apply Default Background Style");
#endif
            Background = Brush.Parse(ThemeService.IsDarkTheme() ? "#202020" : "#F0F4F9");
            TransparencyLevelHint = [WindowTransparencyLevel.None];
        }

        _isEnabledWindowEffect = enable;
    }
}
