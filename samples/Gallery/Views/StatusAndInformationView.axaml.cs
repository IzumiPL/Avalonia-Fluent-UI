using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Views;

public partial class StatusAndInformationView : InfoBarHostViewBase 
{
    public StatusAndInformationView() : base("StatusAndInformation")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>
        {
            { "ToolTip", ToolTipCard },
            { "InfoBadge", InfoBadgeCard },
            { "InfoBar", InfoBarCard },
            { "ProgressBar", ProgressBarCard },
            { "ProgressRing", ProgressRingCard }
        };
        
        PopupInfoBarPositionComboBox.SelectedItem = PopupInfoBarPosition.TopRight;
        ToastInfoBarPositionComboBox.SelectedItem =  ToastInfoBarPosition.TopRight;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        InfoBarHost.RegisterManager(PopupInfoBarManager);
        InfoBarHost.RegisterManager(ToastInfoBarManager);
    }

    public PopupInfoBarManager PopupInfoBarManager { get; } = new();
    public ToastInfoBarManager ToastInfoBarManager { get; } = new();

    public PopupInfoBarPosition GetPopupInfoBarPosition() => (PopupInfoBarPosition)PopupInfoBarPositionComboBox.SelectedItem;
    public string GetTitle() => LocalizationService.Instance.GetString("Im_Title");
    public int GetPopupInfoBarDuration() => (int)InfoBarDurationNumberBox.Value;
    public bool GetPopupInfoBarIsClosable() => InfoBarIsClosableCheckBox.IsChecked ?? false;

    public ToastInfoBarPosition GetToastInfoBarPosition() => (ToastInfoBarPosition)ToastInfoBarPositionComboBox.SelectedItem;
    public int GetToastInfoBarDuration() => (int)ToastDurationNumberBox.Value;
    public bool GetToastInfoBarIsClosable() => ToastIsClosableCheckBox.IsChecked ?? false;
    
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
            GetTitle(),
            new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = LocalizationService.Instance.GetString("Custom_Title_Bar_Content") },
                    new Button { Content = "Action", HorizontalAlignment = HorizontalAlignment.Right, Width = 128 }
                }
            },
            GetPopupInfoBarPosition(),
            PopupInfoBarSeverity.Custom,
            GetPopupInfoBarIsClosable(),
            GetPopupInfoBarDuration()
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
                    new Button { Content = "Action", HorizontalAlignment = HorizontalAlignment.Right, Width = 128 }
                }
            },
            GetToastInfoBarPosition(),
            ToastSeverity.Custom,
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
}
