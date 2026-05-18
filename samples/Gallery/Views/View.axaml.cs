using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using AvaloniaFluentUI.UI.Controls;
using Gallery.Controls;

namespace Gallery.Views;

public partial class View : ViewBase 
{
    public View() : base("View")
    {
        InitializeComponent();
        Carousel.AddHandler(
            PointerWheelChangedEvent,
            (_, e) =>
            {
                if (e.Delta.Y > 0)
                {
                    Carousel.Previous();
                }
                else
                {
                    Carousel.Next();
                } 
            },
            RoutingStrategies.Tunnel,
        true);

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"ListBox", ListBoxCard},
            {"TreeView", TreeViewCard},
            {"FlipView", FlipViewCard}
        };

        // Carousel.Loaded += (_, e) =>
        // {
        //     Carousel.PointerWheelChanged += (_, e) =>
        //     {
        //         e.Handled = true;
        //         
        //         Console.WriteLine(true);
        //         Console.WriteLine(e.Delta.X);
        //         Console.WriteLine(e.Delta.Y);
        //     };
        //     
        //     var scroll = Carousel.GetVisualDescendants().OfType<SmoothScrollViewer>().FirstOrDefault();
        //
        //     if (scroll != null)
        //     {
        //         Console.WriteLine("Not Null");
        //         scroll.PointerWheelChanged += (_, e) =>
        //         {
        //             Console.WriteLine(true);
        //             Console.WriteLine(e.Delta.X);
        //             Console.WriteLine(e.Delta.Y);
        //             
        //             if (e.Delta.Y > 0)
        //             {
        //                 Console.WriteLine(true);
        //             }
        //             else
        //             {
        //                 Console.WriteLine(false);
        //             }
        //         };
        //     }
        // };
    }

    protected override Type StyleKeyOverride => typeof(ViewBase);
}
