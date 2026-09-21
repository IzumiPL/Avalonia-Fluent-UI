using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Controls;
using Gallery.Helpers;

namespace Gallery.Views;

public partial class HomeView : UserControl
{
    // private readonly ContentDialog _confirmDialog;
    // private Thumb? _thumb;
    // private readonly Popup? _popup;
    // private readonly Slider _slider;

    public HomeView()
    {
        InitializeComponent();

        // OverlayLayer.GetOverlayLayer(this).Children.Add();
        // _slider = new Slider { Maximum = 100, Minimum = 0, Width = 256 };
        // _popup = new Popup
        // {
        //     Width = 128, 
        //     Height = 64,
        //     Child = new TextBlock { TextAlignment = TextAlignment.Center,  FontSize = 24},
        //     IsOpen = true,
        //     IsVisible = false,
        //     PlacementTarget = _slider,
        //     Placement = PlacementMode.Top
        // };
        //
        // _slider.ValueChanged += (sender, e) =>
        // {
        //     _popup?.IsOpen = true;
        // };
        // OverlayLayer.GetOverlayLayer(this)?.Children.Add(_slider);
        //
        // StackPanel.Children.Add(_slider);
        //
        // _confirmDialog = new ContentDialog
        // {
        //     PrimaryButtonText = "确定",
        //     CloseButtonText = "取消",
        //     DefaultButton = ContentDialogButton.Primary
        // };
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var window = TopLevel.GetTopLevel(this) as Window;
        if (window != null)
        {
            window.PropertyChanged += (_, e) =>
            {
                if (e.Property == Window.WindowStateProperty)
                {
                    bool isMax = window.WindowState == WindowState.Maximized || window.WindowState == WindowState.FullScreen;
                    Thickness value = isMax ? new Thickness(0, 0, 10, 20) : new Thickness(0, 0, 0, 20);
                    SmoothScrollViewer.Margin = value;
                }
            };
        }
    }

    // protected async override void OnApplyTemplate(TemplateAppliedEventArgs e)
    // {
        // base.OnApplyTemplate(e);
    // }

    // private async void OnOpenDialogClick(object? sender, RoutedEventArgs e)
    // {
        // var owner = TopLevel.GetTopLevel(this) as Window;
        // if (owner == null)
            // return;

        // var dialog = new ContentDialog
        // {
            // Title = "我是标题",
            // Content = new TextBlock { Text = "你确定要删除吗？" },
            // PrimaryButtonText = "确定",
            // CloseButtonText = "取消",
            // DefaultButton =  ContentDialogButton.Primary
        // };

        // await dialog.ShowAsync(owner);
    // }
//     public HomeView()
//     {
// #if DEBUG
//         Debug.WriteLine("HomeView Init");
// #endif
//         InitializeComponent();
//     }
//
//     private async void OnOpenDialogClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
//     {
//         var result = await new ContentDialog
//         {
//             Title = "我是标题",
//             Content = "你确定要删除吗？",
//             PrimaryButtonText = "确定",
//             CloseButtonText = "取消",
//         }.ShowAsync(TopLevel.GetTopLevel(this));
//         
//         if (result == ContentDialogResult.Primary)
//         {
//             ConsoleTest.WriteLine("你点了确定");
//         }
//         else
//         {
//             ConsoleTest.WriteLine("你点了取消");
//         }
//     }
    private void OnGettingStartedClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl(new Uri("https://github.com/HiyorinI/AvaloniaFluentUI.git"), TopLevel.GetTopLevel(this));
    }

    private void OnGitHubRepoClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl(new Uri("https://github.com/HiyorinI/AvaloniaFluentUI.git"), TopLevel.GetTopLevel(this));
    }

    private void OnCodeSamplesClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl(new Uri("https://github.com/HiyorinI/AvaloniaFluentUI/tree/master/samples/Gallery"), TopLevel.GetTopLevel(this));
    }

    private void OnSendFeedBackClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl(new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/issues/new"), TopLevel.GetTopLevel(this));
    }

    private void ScrollTo(object? sender, RoutedEventArgs e)
    {
        var y = Math.Max(0, SmoothScrollViewer.Extent.Height - SmoothScrollViewer.Viewport.Height);
        if (((Button)sender).CommandParameter == "Center")
        {
            y /= 2;
        }
        
        _ = SmoothScrollViewer.GetVisualDescendants().OfType<SmoothScrollContentPresenter>().FirstOrDefault()?.ScrollToAsync(new Vector(SmoothScrollViewer.Offset.X, y));
    }
}
