using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Windowing;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class WizardPage : InfoBarHostViewBase 
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/ViewPage/WizardPage.axaml"); 
    
    private PopupInfoBarManager PopupInfoBarManager => InfoBarHost.GetManager<PopupInfoBarManager>();
    
    public WizardPage()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        InfoBarHost.RegisterManager<PopupInfoBarManager>();
    }

    private void OnNexting(object? sender, WizardNextingEventArgs e)
    {
        var page = e.CurrentIndex;

        switch (page)
        {
            case 1:
                if ((P2.TokenEdit.Text != "tk_1") || (P2.ProxyEdit.Text != "locale"))
                {
                    e.Cancel = true;
                    PopupInfoBarManager.Error(LocalizationService.Instance.GetString("Error"), LocalizationService.Instance.GetString("TkError"), InfoBarPosition.TopRight, duration: 3500);
                }
                break;
            case 2:               
                if ((P3.FatherEdit.Text != "1") || (P3.MotherEdit.Text != "2"))
                {
                    e.Cancel = true;
                    PopupInfoBarManager.Error(LocalizationService.Instance.GetString("Error"), LocalizationService.Instance.GetString("NameError"), InfoBarPosition.TopRight, duration: 3500);
                }
                break;
            case 3:                
                if ((P4.UserNameEdit.Text != "1") || (P4.PasswordEdit.Text != "1"))
                {
                    e.Cancel = true;
                    PopupInfoBarManager.Error(LocalizationService.Instance.GetString("Error"), LocalizationService.Instance.GetString("UnPsError"), InfoBarPosition.TopRight, duration: 3500);
                }
                break;
        }
    }

    private void OnFinished(object? sender, System.EventArgs e)
    {
        // WizardView.IsVisible = false;
    }

    private void OnFinishing(object? sender, WizardFinishingEventArgs e)
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
                        ww.InfoBarHost.GetManager<PopupInfoBarManager>().Error(LocalizationService.Instance.GetString("Error"), LocalizationService.Instance.GetString("TkError"), InfoBarPosition.TopRight, duration: 3500);
                    }
                    break;
                case 2:               
                    if ((p3.FatherEdit.Text != "1") || (p3.MotherEdit.Text != "2"))
                    {
                        e.Cancel = true;
                        ww.InfoBarHost.GetManager<PopupInfoBarManager>().Error(LocalizationService.Instance.GetString("Error"), LocalizationService.Instance.GetString("NameError"), InfoBarPosition.TopRight, duration: 3500);
                    }
                    break;
                case 3:                
                    if ((p4.UserNameEdit.Text != "1") || (p4.PasswordEdit.Text != "1"))
                    {
                        e.Cancel = true;
                        ww.InfoBarHost.GetManager<PopupInfoBarManager>().Error(LocalizationService.Instance.GetString("Error"), LocalizationService.Instance.GetString("UnPsError"), InfoBarPosition.TopRight, duration: 3500);
                    }
                    break;
            }
        };
        ww.WizardView.Finished += (_, _) => ww.Close();
        ww.TitleBarMargin = new Thickness(-20, 0, 0, 0);

        if (TopLevel.GetTopLevel(this) is Window w)
        {
            ww.ShowDialog(w);
        }
    }
}

public class WizardWindow : FluentWindow 
{
    public WizardView WizardView { get; }

    public WizardWindow()
    {
        WizardView = new WizardView
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            CornerRadius = new CornerRadius(0),
            BorderThickness =  new Thickness(0),
            BorderBrush = Brushes.Transparent,
        };

        Content = WizardView;

        MaxButtonIsVisible = false;
        MinButtonIsVisible = false;
        CanResize = false;
        EnabledMica(true);
    }

    public WizardWindow(IEnumerable<Control> pages) : this()
    {
        AddPages(pages);
    }

    public WizardWindow(params Control[] pages) : this((IEnumerable<Control>)pages) { }

    public void AddPage(Control page)
    {
        WizardView.Items.Add(page);
    }

    public void AddPages(IEnumerable<Control> pages)
    {
        foreach (var page in pages)
        {
            WizardView.Items.Add(page);
        }
    }
}
