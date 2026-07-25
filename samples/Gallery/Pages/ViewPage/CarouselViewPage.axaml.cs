using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using AvaloniaFluentUI.Controls;
using Gallery.Controls;

namespace Gallery.Pages;

public class FlipViewImageDelegate(string text, string? content = null, IBrush? foreground = null) : IImageLabelDelegate
{
    private readonly string _text = text;
    private readonly string? _content = content;
    private readonly IBrush _foreground = foreground ?? Brushes.White;
    
    public void Render(DrawingContext context, Rect rect, CornerRadius radius)
    {
        var tl = new TextLayout(
            _text,
            Typeface.Default,
            32,
            _foreground
        );

        var x = 32;
        var y = 48;
        
        if (_content != null)
        {
            var cl = new TextLayout(
                _content,
                Typeface.Default,
                14,
                _foreground
            );
            cl.Draw(context, new Point(x, y + tl.Height + 6));
        }
        
        tl.Draw(context, new Point(x, y));
    }
}

public partial class CarouselViewPage : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/ViewPage/CarouselViewPage.axaml"); 
    
    private IImageLabelDelegate[] _flipViewImageDelegates;
    
    public CarouselViewPage() : base("CarouselView")
    {
        InitializeComponent();

        _flipViewImageDelegates = new IImageLabelDelegate[] 
        {
            new FlipViewImageDelegate("樱色约定", "在樱花飘落的季节，与命中注定的人许下永恒的约定。"),
            new FlipViewImageDelegate("星之瞳", "凝望遥远星空，寻找隐藏在宇宙深处的未知奇迹。"), 
            new FlipViewImageDelegate("月下幻想", "银色月光洒落之时，来自异世界的篇章悄然展开。"), 
            new FlipViewImageDelegate("时空旅人", "穿越漫长岁月，寻找被遗忘的故事与失落的记忆。"),
            new FlipViewImageDelegate("星海彼岸", "追逐群星的轨迹，向着未知的远方踏上冒险之旅。"), 
            new FlipViewImageDelegate("梦境回廊", "穿越现实与幻想的边界，开启一场只属于你的梦幻旅程。"), 
        };

        FlipView.MaxWidth = 580;
        FlipView.Height = 360;
        
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

    private void OnEnabledFlipViewImageDelegate(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb)
        {
            FlipView.SetImageDelegates(cb.IsChecked == true ?  _flipViewImageDelegates : null);
        }
    }
}
