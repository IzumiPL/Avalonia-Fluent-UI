using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Media.Animation;
using Gallery.Controls;
using Gallery.Models;
using Gallery.Pages;
using Gallery.ViewModels;

namespace Gallery.Views;

public partial class NavigationView : UserControl 
{
    public NavigationView()// : base("Navigation")
    {
        InitializeComponent();
        
        // CodeCards = new Dictionary<string, CodeCard>()
        // {
            // {"NavigationView", NavigationViewCard},
            // {"PageTransition", PageTransitionCard},
            // {"TabView", TabViewCard},
            // {"BreadcrumbBar", BreadcrumbBarCard},
            // {"Segmented", SegmentedCard}
        // };
    }
}
