using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Styling;
using AvaloniaFluentUI.Windowing;

namespace InfoBarTest.Views;

public partial class MainWindow : FluentWindow 
{
    private PopupInfoBarManager PopupManager { get; } = new();
    private ToastInfoBarManager ToastManager { get; } = new();

    private readonly Bitmap _bitmap = Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://InfoBarTest/Assets/mc.jpg")), 720);

    public MainWindow()
    {
        InitializeComponent();
        Application.Current.Resources["NavigationViewContentGridCornerRadius"] = new CornerRadius(0);
        TitleBarMargin = new Thickness(50, 0, 0, 0);

        // 注册 manager 到 InfoBarHost
        InfoBarHost.RegisterManager(PopupManager);
        InfoBarHost.RegisterManager(ToastManager);
        
        // InfoBarHost.GetManager<>()
        
        NavigationView.PropertyChanged += (_, e) =>
        {
            if (e.Property == NavigationView.IsPaneOpenProperty)
            {
                if (NavigationView.DisplayMode == NavigationViewDisplayMode.Minimal)
                {
                    TitleBarMargin = new Thickness(76, 0, 0, 0);
                }
                else if (NavigationView.DisplayMode == NavigationViewDisplayMode.Compact)
                {
                    TitleBarMargin = new Thickness(50, 0, 0, 0);
                }
                else
                {
                    TitleBarMargin = new Thickness(NavigationView.IsPaneOpen ? NavigationView.OpenPaneLength + 12 : 50, 0, 0, 0);
                }
            }

            if (e.Property == NavigationView.DisplayModeProperty)
            {
                if (NavigationView.DisplayMode == NavigationViewDisplayMode.Minimal)
                {
                    TitleBarMargin = new Thickness(76, 0, 0, 0);
                }
            }
        };
    }

    // 辅助方法：从 UI 读取通用选项
    private int GetDuration() => (int)(DurationBox.Value ?? 3000);
    private bool GetIsClosable() => ClosableCheckBox.IsChecked == true;

    // 枚举索引与 ComboBox 顺序一致: TopLeft=0, Top=1, TopRight=2, BottomLeft=3, Bottom=4, BottomRight=5
    private PopupInfoBarPosition GetPopupPosition() => (PopupInfoBarPosition)(PositionComboBox.SelectedIndex);
    private ToastInfoBarPosition GetToastPosition() => (ToastInfoBarPosition)(PositionComboBox.SelectedIndex);

    // 辅助方法：读取自定义输入
    private string GetCustomTitle() => CustomTitleBox.Text ?? "自定义标题";
    private string GetCustomContent() => CustomContentBox.Text ?? "自定义内容";

    // 枚举索引与 ComboBox 顺序一致: Informational=0, Success=1, Warning=2, Error=3, Custom=4
    private PopupInfoBarSeverity GetCustomPopupSeverity() => (PopupInfoBarSeverity)(CustomSeverityBox.SelectedIndex);
    private ToastSeverity GetCustomToastSeverity() => (ToastSeverity)(CustomSeverityBox.SelectedIndex);

    // ========================
    //  PopupInfoBar 按钮事件
    // ========================

    private void OnPopupInfoClick(object? sender, RoutedEventArgs e)
    {
        PopupManager.Information(
            "Information",
            "这是一条 PopupInfoBar 信息通知。",
            GetPopupPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnPopupSuccessClick(object? sender, RoutedEventArgs e)
    {
        PopupManager.Success(
            "Success",
            "操作成功完成！（PopupInfoBar）",
            GetPopupPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnPopupWarningClick(object? sender, RoutedEventArgs e)
    {
        PopupManager.Warning(
            "Warning",
            "请注意，这是一条警告信息。（PopupInfoBar）",
            GetPopupPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnPopupErrorClick(object? sender, RoutedEventArgs e)
    {
        PopupManager.Error(
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
        ToastManager.Information(
            "Information",
            "这是一条 ToastInfoBar 信息通知。",
            GetToastPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnToastSuccessClick(object? sender, RoutedEventArgs e)
    {
        ToastManager.Success(
            "Success",
            "操作成功完成！（ToastInfoBar）",
            GetToastPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnToastWarningClick(object? sender, RoutedEventArgs e)
    {
        ToastManager.Warning(
            "Warning",
            "请注意，这是一条警告信息。（ToastInfoBar）",
            GetToastPosition(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnToastErrorClick(object? sender, RoutedEventArgs e)
    {
        ToastManager.Error(
            "Error",
            "发生了一个错误！（ToastInfoBar）",
            GetToastPosition(),
            GetIsClosable(),
            GetDuration());
    }

    // ========================
    //  自定义 InfoBar 按钮事件
    // ========================

    // Popup New：使用用户输入的标题、内容和选择的 Severity
    private void OnPopupNewClick(object? sender, RoutedEventArgs e)
    {
        PopupManager.New(
            GetCustomTitle(),
            GetCustomContent(),
            GetPopupPosition(),
            GetCustomPopupSeverity(),
            GetIsClosable(),
            GetDuration());
    }

    // Toast New：使用用户输入的标题、内容和选择的 Severity
    private void OnToastNewClick(object? sender, RoutedEventArgs e)
    {
        ToastManager.New(
            GetCustomTitle(),
            GetCustomContent(),
            GetToastPosition(),
            GetCustomToastSeverity(),
            GetIsClosable(),
            GetDuration());
    }

    // 辅助方法：从 ColorPicker 读取颜色
    private IBrush GetBgBrush() => new SolidColorBrush(BgColorPicker.Color);
    private IBrush GetFgBrush() => new SolidColorBrush(FgColorPicker.Color);

    // Popup Custom：自定义背景/前景色
    private void OnPopupCustomColorClick(object? sender, RoutedEventArgs e)
    {
        PopupManager.Custom(
            GetCustomTitle(),
            GetCustomContent(),
            GetPopupPosition(),
            GetIsClosable(),
            GetDuration(),
            GetBgBrush(),
            GetFgBrush());
    }

    // Toast Custom：自定义背景/前景色
    private void OnToastCustomColorClick(object? sender, RoutedEventArgs e)
    {
        ToastManager.Custom(
            GetCustomTitle(),
            GetCustomContent(),
            GetToastPosition(),
            GetIsClosable(),
            GetDuration(),
            GetBgBrush(),
            GetFgBrush());
    }

    // Popup 带控件：Content 为一个包含按钮和复选框的 StackPanel
    private void OnPopupWithControlClick(object? sender, RoutedEventArgs e)
    {
        PopupManager.New(
            GetCustomTitle(),
            new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = GetCustomContent() },
                    new CheckBox { Content = "记住我的选择", IsChecked = false },
                    new Button { Content = "Action", HorizontalAlignment = HorizontalAlignment.Right }
                }
            },
            GetPopupPosition(),
            GetCustomPopupSeverity(),
            GetIsClosable(),
            GetDuration());
    }

    // Toast 带控件：Content 为一个包含进度条和链接的 StackPanel
    private void OnToastWithControlClick(object? sender, RoutedEventArgs e)
    {
        ToastManager.New(
            GetCustomTitle(),
            new StackPanel { 
                Spacing = 8,
                Children =
                { 
                    new Border { 
                        ClipToBounds = true, 
                        CornerRadius = new CornerRadius(8), 
                        Child = new Image 
                        {
                            Width = 456,
                            Height = 256,
                            Source = _bitmap,
                            Stretch = Stretch.UniformToFill 
                        } 
                    },
                    new Button
                    {
                        Content = "Action",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Classes = { "Accent" }
                    }
                }
            },
            GetToastPosition(),
            GetCustomToastSeverity(),
            GetIsClosable(),
            GetDuration());
    }

    private void OnToggleTheme(object? sender, RoutedEventArgs e)
    {
        AvaloniaFluentTheme.Instance.ToggleTheme();
    }
}
