using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Controls;
using Gallery.Messages;

namespace Gallery.Views;

public partial class DialogBoxAndPopupView : ViewBase
{
    public DialogBoxAndPopupView() : base("DialogBoxAndPopup")
    {
        InitializeComponent();

        Title = LocalizationService.Instance.GetString("DialogAndPopup");

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"TaskDialog", TaskDialogCard},
            {"Flyout", FlyoutCard},
            {"ContentDialog", ContentDialogCard},
            {"TeachingTip", TeachingTipCard}
        };
    }
    
    private async void OnShowContentDialog(object? sender, RoutedEventArgs e)
    {
        await ContentDialog.ShowAsync();
    }

    private async void OnShowCustomDialog(object? sender, RoutedEventArgs e)
    {
        await CustomDialog.ShowAsync();
    }

    private async void OnShowTaskDialog(object? sender, RoutedEventArgs e)
    {
        var taskDialog = new TaskDialog
        {
            FooterVisibility = TaskDialogFooterVisibility.Always,
            IsVisible = false,
            ShowProgressBar = false,
            Header = "下载文件",
            SubHeader = "你准备好下载文件了吗?",
            Buttons = [TaskDialogButton.YesButton, TaskDialogButton.NoButton]
        };
        taskDialog.Closing += (_, e) =>
        {
            if ((TaskDialogStandardResult)e.Result == TaskDialogStandardResult.Yes)
            {
                var deferral = e.GetDeferral();
                taskDialog.ShowProgressBar = true;
                int value = 0;
                DispatcherTimer timer = null;

                void Tick(object? s, EventArgs e)
                {
                    taskDialog.SetProgressBarState(++value, TaskDialogProgressState.Normal);
                    if (value == 100) { timer.Stop(); }
                    deferral.Complete();
                }

                timer = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Normal, Tick);
                timer.Start();
            } 
        };

        taskDialog.XamlRoot = TopLevel.GetTopLevel(this); 
        _=await taskDialog.ShowAsync();
    }
}
