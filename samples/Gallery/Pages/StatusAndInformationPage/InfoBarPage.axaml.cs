using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;
using Gallery.Extensions;

namespace Gallery.Pages;

public partial class InfoBarPage : InfoBarHostViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/StatusAndInformationPage/InfoBarPage.axaml"); 
    
    public InfoBarPage() : base("InfoBar")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard> { { "InfoBar", InfoBarCard } };

        PopupInfoBarPositionComboBox.SelectedItem = InfoBarPosition.TopRight;
        ToastInfoBarPositionComboBox.SelectedItem = InfoBarPosition.TopRight;
        
        
        InfoBarDurationEdit.ItemsSource = new int[] {-1, 500, 1000, 1500, 2000, 2500, 3000, 3500, 5000, 10000};
        InfoBarRadiusEdit.ItemsSource = new int[] {0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24};
    }

    public PopupInfoBarManager PopupInfoBarManager
    {
        get => InfoBarHost.GetManager<PopupInfoBarManager>();
    }

    public ToastInfoBarManager ToastInfoBarManager
    {
        get => InfoBarHost.GetManager<ToastInfoBarManager>();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        InfoBarHost.RegisterManager<PopupInfoBarManager>();
        InfoBarHost.RegisterManager<ToastInfoBarManager>();
    }

    public InfoBarPosition GetPopupInfoBarPosition()
    {
        return (InfoBarPosition)PopupInfoBarPositionComboBox.SelectedItem;
    }

    public string GetTitle()
    {
        return LocalizationService.Instance.GetString("Im_Title");
    }

    public int GetPopupInfoBarDuration()
    {
        return (int)InfoBarDurationNumberBox.Value;
    }

    public bool GetPopupInfoBarIsClosable()
    {
        return InfoBarIsClosableCheckBox.IsChecked ?? false;
    }

    public InfoBarPosition GetToastInfoBarPosition()
    {
        return (InfoBarPosition)ToastInfoBarPositionComboBox.SelectedItem;
    }

    public int GetToastInfoBarDuration()
    {
        return (int)ToastDurationNumberBox.Value;
    }

    public bool GetToastInfoBarIsClosable()
    {
        return ToastIsClosableCheckBox.IsChecked ?? false;
    }

    private void OnShowInformationInfoBar(object? sender, RoutedEventArgs e)
    {
        PopupInfoBarManager.Information(
            GetTitle(),
            LocalizationService.Instance.GetString("Information_Title_Bar_Content"),
            GetPopupInfoBarPosition(),
            GetPopupInfoBarIsClosable(),
            GetPopupInfoBarDuration()
        );
    }

    private void OnShowSuccessInfoBar(object? sender, RoutedEventArgs e)
    {
        PopupInfoBarManager.Success(
            GetTitle(),
            LocalizationService.Instance.GetString("Success_Title_Bar_Content"),
            GetPopupInfoBarPosition(),
            GetPopupInfoBarIsClosable(),
            GetPopupInfoBarDuration()
        );
    }

    private void OnShowWarningInfoBar(object? sender, RoutedEventArgs e)
    {
        PopupInfoBarManager.Warning(
            GetTitle(),
            LocalizationService.Instance.GetString("Warning_Title_Bar_Content"),
            GetPopupInfoBarPosition(),
            GetPopupInfoBarIsClosable(),
            GetPopupInfoBarDuration()
        );
    }

    private void OnShowErrorInfoBar(object? sender, RoutedEventArgs e)
    {
        PopupInfoBarManager.Error(
            GetTitle(),
            LocalizationService.Instance.GetString("Error_Title_Bar_Content"),
            GetPopupInfoBarPosition(),
            GetPopupInfoBarIsClosable(),
            GetPopupInfoBarDuration()
        );
    }

    private void OnShowCustomInfoBar(object? sender, RoutedEventArgs e)
    {
        PopupInfoBarManager.New(
            new PopupInfoBar
            {
                CornerRadius = new CornerRadius(InfoBarRadiusEdit.Text.ToIntOrDefault(6)),
                Title = InfoBarTitleEdit.Text,
                Content = new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = InfoBarContentEdit.Text
                        },
                        new Button
                        {
                            Content = "Action",
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Width = 128
                        }
                    }
                },
                MaxWidth = PopupInfoBarManager.InfoBarMaxWidth,
                Background = new SolidColorBrush(InfoBarBackgroundEdit.Color),
                Foreground = new SolidColorBrush(InfoBarForegroundEdit.Color),
                Position = GetPopupInfoBarPosition(),
                Severity = InfoBarSeverity.Custom,
                IsClosable = GetPopupInfoBarIsClosable(),
                Duration = InfoBarDurationEdit.Text.ToIntOrDefault(3000)
            }
        );
    }

    private void OnShowCustomToastInfoBar(object? sender, RoutedEventArgs e)
    {
        ToastInfoBarManager.New(
            GetTitle(),
            new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = LocalizationService.Instance.GetString("Custom_Title_Bar_Content") },
                    new Button
                    {
                        Content = "Action", HorizontalAlignment = HorizontalAlignment.Right, Width = 128
                    }
                }
            },
            GetToastInfoBarPosition(),
            InfoBarSeverity.Error,
            GetPopupInfoBarIsClosable(),
            GetPopupInfoBarDuration()
        );
    }

    private void OnShowErrorToastInfoBar(object? sender, RoutedEventArgs e)
    {
        ToastInfoBarManager.Error(
            GetTitle(),
            LocalizationService.Instance.GetString("Information_Title_Bar_Content"),
            GetToastInfoBarPosition(),
            GetToastInfoBarIsClosable(),
            GetToastInfoBarDuration()
        );
    }

    private void OnShowWarningToastInfoBar(object? sender, RoutedEventArgs e)
    {
        ToastInfoBarManager.Warning(
            GetTitle(),
            LocalizationService.Instance.GetString("Information_Title_Bar_Content"),
            GetToastInfoBarPosition(),
            GetToastInfoBarIsClosable(),
            GetToastInfoBarDuration()
        );
    }

    private void OnShowSuccessToastInfoBar(object? sender, RoutedEventArgs e)
    {
        ToastInfoBarManager.Success(
            GetTitle(),
            LocalizationService.Instance.GetString("Information_Title_Bar_Content"),
            GetToastInfoBarPosition(),
            GetToastInfoBarIsClosable(),
            GetToastInfoBarDuration()
        );
    }

    private void OnShowInformationToastInfoBar(object? sender, RoutedEventArgs e)
    {
        ToastInfoBarManager.Information(
            GetTitle(),
            LocalizationService.Instance.GetString("Information_Title_Bar_Content"),
            GetToastInfoBarPosition(),
            GetToastInfoBarIsClosable(),
            GetToastInfoBarDuration()
        );
    }

    private void OnShowInfoBarEditDialog(object? sender, RoutedEventArgs e)
    {
        PopupInfoBarEditDialog.ShowAsync(TopLevel.GetTopLevel(this));
    }
}
