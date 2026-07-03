using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Styling;

namespace InfoBarTest.Views;

public partial class MainWindow : Window
{
    private readonly PopUpInfoBarManager _popupManager = new();
    private readonly ToastInfoBarManager _toastManager = new();

    public MainWindow()
    {
        InitializeComponent();

        // 注册 manager 到 InfoBarHost
        InfoBarHost.RegisterManager(_popupManager);
        InfoBarHost.RegisterManager(_toastManager);
    }

    // 辅助方法：从 UI 读取通用选项
    private int GetDuration() => (int)(DurationBox.Value ?? 3000);
    private bool GetIsClosable() => ClosableCheckBox.IsChecked == true;

    // 枚举索引与 ComboBox 顺序一致: TopLeft=0, Top=1, TopRight=2, BottomLeft=3, Bottom=4, BottomRight=5
    private PopupInfoBarPosition GetPopupPosition() => (PopupInfoBarPosition)(PositionComboBox.SelectedIndex);
    private ToastInfoBarPosition GetToastPosition() => (ToastInfoBarPosition)(PositionComboBox.SelectedIndex);

    // ========================
    //  PopupInfoBar 按钮事件
    // ========================

    private void OnPopupInfoClick(object? sender, RoutedEventArgs e)
    {
        _popupManager.Information(
            "Information",
            "这是一条 PopupInfoBar 信息通知。",
            GetPopupPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnPopupSuccessClick(object? sender, RoutedEventArgs e)
    {
        _popupManager.Success(
            "Success",
            "操作成功完成！（PopupInfoBar）",
            GetPopupPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnPopupWarningClick(object? sender, RoutedEventArgs e)
    {
        _popupManager.Warning(
            "Warning",
            "请注意，这是一条警告信息。（PopupInfoBar）",
            GetPopupPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnPopupErrorClick(object? sender, RoutedEventArgs e)
    {
        _popupManager.Error(
            "Error",
            "发生了一个错误！（PopupInfoBar）",
            GetPopupPosition(),
            GetIsClosable(),
            GetDuration());
    }

    // ========================
    //  ToastInfoBar 按钮事件
    // ========================

    private void OnToastInfoClick(object? sender, RoutedEventArgs e)
    {
        _toastManager.Information(
            "Information",
            "这是一条 ToastInfoBar 信息通知。",
            GetToastPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnToastSuccessClick(object? sender, RoutedEventArgs e)
    {
        _toastManager.Success(
            "Success",
            "操作成功完成！（ToastInfoBar）",
            GetToastPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnToastWarningClick(object? sender, RoutedEventArgs e)
    {
        _toastManager.Warning(
            "Warning",
            "请注意，这是一条警告信息。（ToastInfoBar）",
            GetToastPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnToastErrorClick(object? sender, RoutedEventArgs e)
    {
        _toastManager.Error(
            "Error",
            "发生了一个错误！（ToastInfoBar）",
            GetToastPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnToggleTheme(object? sender, RoutedEventArgs e)
    {
        AvaloniaFluentTheme.Instance.ToggleTheme();
    }
}
