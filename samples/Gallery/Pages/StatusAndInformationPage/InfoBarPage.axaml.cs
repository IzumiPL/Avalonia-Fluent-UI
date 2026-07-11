using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class InfoBarPage : InfoBarHostViewBase 
{
    public InfoBarPage() : base("InfoBar")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"InfoBar", InfoBarCard}
        };
        
        PopupInfoBarPositionComboBox.SelectedItem = InfoBarPosition.TopRight;
        ToastInfoBarPositionComboBox.SelectedItem =  InfoBarPosition.TopRight;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        InfoBarHost.RegisterManager<PopupInfoBarManager>();
        InfoBarHost.RegisterManager<ToastInfoBarManager>();
    }

    public PopupInfoBarManager PopupInfoBarManager => InfoBarHost.GetManager<PopupInfoBarManager>();
    public ToastInfoBarManager ToastInfoBarManager => InfoBarHost.GetManager<ToastInfoBarManager>();

    public InfoBarPosition GetPopupInfoBarPosition() => (InfoBarPosition)PopupInfoBarPositionComboBox.SelectedItem;
    public string GetTitle() => LocalizationService.Instance.GetString("Im_Title");
    public int GetPopupInfoBarDuration() => (int)InfoBarDurationNumberBox.Value;
    public bool GetPopupInfoBarIsClosable() => InfoBarIsClosableCheckBox.IsChecked ?? false;

    public InfoBarPosition GetToastInfoBarPosition() => (InfoBarPosition)ToastInfoBarPositionComboBox.SelectedItem;
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
            InfoBarSeverity.Custom,
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
}

