using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Views;

public partial class MediaView : ViewBase 
{
    public MediaView()
    {
        InitializeComponent();
        
        
    }

    private async void OnSelectedImage(object? sender, RoutedEventArgs e)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        if (toplevel != null)
        {
            var values = await toplevel.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "选择图片",
                    AllowMultiple = false,
                    FileTypeFilter = [
                    new FilePickerFileType("图片文件") { Patterns = ["*.png", "*.jpg", "*.webp"] },
                    new FilePickerFileType("所有文件") { Patterns = ["*.*"] }
                    ]
                });

            if (values.Count > 0)
            {
                var path = values[0].TryGetLocalPath();
                if (path != null)
                {
                    ImageLabel.Source = path;
                }
            }
        }
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        ImageLabel.MaxWidth = e.NewSize.Width - 400;
    }
}
