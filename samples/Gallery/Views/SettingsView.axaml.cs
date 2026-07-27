using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages.MainWindowMessages;
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
}
