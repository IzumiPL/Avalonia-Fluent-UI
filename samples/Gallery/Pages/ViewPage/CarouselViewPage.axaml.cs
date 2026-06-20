using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class CarouselViewPage : ViewBase
{
    public CarouselViewPage() : base("CarouselView")
    {
        InitializeComponent();
        
        Carousel.AddHandler(
            PointerWheelChangedEvent,
            (_, e) =>
            {
                if (e.Delta.Y > 0) { Carousel.Previous(); }
                else { Carousel.Next(); } 
            }, RoutingStrategies.Tunnel, true);
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"PageTransition", PageTransitionsCard},
            {"FlipView", FlipViewCard}
        };
    }
}
