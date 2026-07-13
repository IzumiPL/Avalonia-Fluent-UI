using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Styling;
using AvaloniaFluentUI.Windowing;
using WizardTest.Controls;
using WizardTest.Pages;

namespace WizardTest;

public partial class MainWindow : FluentWindow 
{
    private PopupInfoBarManager PopupInfoBarManager => InfoBarHost.GetManager<PopupInfoBarManager>();
    
    public MainWindow()
    {
        InitializeComponent();

        var host = new InfoBarHost();
        
        host.RegisterManager<PopupInfoBarManager>();
        host.RegisterManager<ToastInfoBarManager>();

        host.GetManager<PopupInfoBarManager>();
        host.GetManager<ToastInfoBarManager>()
            .Information("Title", "Content", InfoBarPosition.TopRight, true);
        
        InfoBarHost.RegisterManager<PopupInfoBarManager>();
        
        EnabledMica(true);
    }

    private void OnToggleTheme(object? sender, RoutedEventArgs e)
    {
        AvaloniaFluentTheme.Instance.ToggleTheme();
    }

    private void OnNexting(object? sender, WizardTest.EventArgs.WizardNextingEventArgs e)
    {
        var page = e.CurrentIndex;

        switch (page)
        {
            case 1:
                if ((P2.TokenEdit.Text != "tk_1") || (P2.ProxyEdit.Text != "locale"))
                {
                    e.Cancel = true;
                    PopupInfoBarManager.Error("错误", "访问令牌或代理服务器地址输入有误,请从新输入!", InfoBarPosition.TopRight, duration: 3500);
                }
                break;
            case 2:               
                if ((P3.FatherEdit.Text != "1") || (P3.MotherEdit.Text != "2"))
                {
                    e.Cancel = true;
                    PopupInfoBarManager.Error("错误", "名称输入有误,请从新输入!", InfoBarPosition.TopRight, duration: 3500);
                }
                break;
            case 3:                
                if ((P4.UserNameEdit.Text != "1") || (P4.PasswordEdit.Text != "1"))
                {
                    e.Cancel = true;
                    PopupInfoBarManager.Error("错误", "用户名或密码入有误,请从新输入!", InfoBarPosition.TopRight, duration: 3500);
                }
                break;
        }
    }

    private void OnFinished(object? sender, System.EventArgs e)
    {
        WizardView.IsVisible = false;
    }

    private void OnFinishing(object? sender, WizardTest.EventArgs.WizardFinishingEventArgs e)
    {
    }

    private void OnNexted(object? sender, System.EventArgs e)
    {
    }

    private void OnShowWizardWindow(object? sender, RoutedEventArgs _)
    {
        var p2 = new P2();
        var p3 = new P3();
        var p4 = new P4();
        var ww = new WizardWindow(new P1(), p2, p3, p4, new P5());
        ww.ShowInTaskbar = false;
        ww.MinWidth = 580;
        ww.MinHeight = 400;
        ww.Width = 720;
        ww.Height = 500;
        ww.MaxWidth = 880;
        ww.MaxHeight = 620;
        ww.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        ww.Loaded += (_, _) => { ww.InfoBarHost.RegisterManager<PopupInfoBarManager>(); };
        
        ww.WizardView.Nexting += (_, e) =>
        {
            var page = e.CurrentIndex;

            switch (page)
            {
                case 1:
                    if ((p2.TokenEdit.Text != "tk_1") || (p2.ProxyEdit.Text != "locale"))
                    {
                        e.Cancel = true;
                        ww.InfoBarHost.GetManager<PopupInfoBarManager>().Error("错误", "访问令牌或代理服务器地址输入有误,请从新输入!", InfoBarPosition.TopRight, duration: 3500);
                    }
                    break;
                case 2:               
                    if ((p3.FatherEdit.Text != "1") || (p3.MotherEdit.Text != "2"))
                    {
                        e.Cancel = true;
                        ww.InfoBarHost.GetManager<PopupInfoBarManager>().Error("错误", "名称输入有误,请从新输入!", InfoBarPosition.TopRight, duration: 3500);
                    }
                    break;
                case 3:                
                    if ((p4.UserNameEdit.Text != "1") || (p4.PasswordEdit.Text != "1"))
                    {
                        e.Cancel = true;
                        ww.InfoBarHost.GetManager<PopupInfoBarManager>().Error("错误", "用户名或密码入有误,请从新输入!", InfoBarPosition.TopRight, duration: 3500);
                    }
                    break;
            }
        };
        ww.WizardView.Finished += (_, _) => ww.Close();
        ww.TitleBarMargin = new Thickness(-20, 0, 0, 0);
        
        ww.ShowDialog(this);
    }
}
