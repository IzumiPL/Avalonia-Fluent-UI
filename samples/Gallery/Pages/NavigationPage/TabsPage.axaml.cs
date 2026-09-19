using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Controls;
using Gallery.Controls;
using Gallery.ViewModels;

namespace Gallery.Pages;

public partial class TabsPage : PopupDrawerHostViewBase 
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/NavigationPage/TabsPage.axaml"); 
    
    public TabsPage() : base("Tabs")
    {
        InitializeComponent();
    }

    private void OnTabViewCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        if (DataContext is TabsPageViewModel vm)
        {
            vm.TabViewRequestClose(args.Item);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        PopupDrawer.Position = PopupDrawerPosition.Right;
        // PopupDrawer.IsLightDismissEnabled = false;
    }

    private void OpenPopupDrawer(object? sender, RoutedEventArgs e)
    {
        PopupDrawer.IsOpen = true;
    }
}
