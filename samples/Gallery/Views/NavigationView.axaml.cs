using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaFluentUI.UI.Controls;
using AvaloniaFluentUI.UI.Media.Animation;
using Gallery.Controls;
using Gallery.Models;
using Gallery.Pages;
using Gallery.ViewModels;

namespace Gallery.Views;

public partial class NavigationView : ViewBase
{
    public NavigationView() : base("Navigation")
    {
        InitializeComponent();
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"NavigationView", NavigationViewCard},
            {"PageTransition", PageTransitionCard},
            // {"TabView", TabViewCard},
            {"BreadcrumbBar", BreadcrumbBarCard},
            {"Segmented", SegmentedCard}
        };

        AddHandler(Frame.RequestBringIntoViewEvent, (_, e) => { e.Handled = true; });
    }

    private void OnListSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = sender as ListBox;
        if (listBox == null) return;
        
        switch (listBox.SelectedIndex)
        {
            case 0:
                Frame.Navigate(typeof(FramePage1));
                break;

            case 1:
                Frame.Navigate(typeof(FramePage2), null, new SlideNavigationTransitionInfo());
                break;

            case 2:
                Frame.Navigate(typeof(FramePage3), null, new DrillInNavigationTransitionInfo());
                break;

            case 3:
                Frame.Navigate(typeof(FramePage4), null, new SuppressNavigationTransitionInfo());
                break;
        }
    }

    private void OnTabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        if (DataContext is NavigationViewModel viewModel)
        {
            if (args.Item is TabViewItem item)
            {
                viewModel.TabItemSource.Remove(item);
            }
        }
    }
}
