using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaFluentUI.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Helpers;
using Gallery.Messages.MainWindowMessages;
using Gallery.Services;
using Gallery.ViewModels;

namespace Gallery.Views;

public partial class SettingsView : UserControl 
{
    public SettingsView()
    {
#if DEBUG
        Debug.WriteLine("SettingsView Init");
#endif
        InitializeComponent();
    }

    private async void OnShowSelectBgImageDialog(object? sender, RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        if (toplevel != null)
        {
            var result = await toplevel.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "选择自定义背景图",
                    AllowMultiple = false,
                    FileTypeFilter =
                    [
                        new FilePickerFileType("图片文件") { Patterns = ["*.png", "*.jpg", "*.webp"] }
                    ]
                }
            );
            if (result.Count > 0 && DataContext is SettingsViewModel vm)
            {
                var path = result[0].Path.LocalPath;
                
                vm.IsEnabledWindowEffect = false;
                vm.CurrentEffect = "Null";
                vm.BackgroundImagePath = path;
                WeakReferenceMessenger.Default.Send(new EnabledBackgroundImageMessage(true, path));
            }
        }
    }

    private void OnHelpClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl(new Uri("https://docs.mikuas.top"), TopLevel.GetTopLevel(this));
    }

    private void OnSendFeedbackClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl(new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/issues/new"), TopLevel.GetTopLevel(this));
    }

    private void OnCheckForUpdateClicked(object? sender, RoutedEventArgs e)
    {
        // InfoBarService.PopupInfoBarManager.Information("检查更新", "暂时没有可用的更新!", InfoBarPosition.TopRight, true);
    }
}
