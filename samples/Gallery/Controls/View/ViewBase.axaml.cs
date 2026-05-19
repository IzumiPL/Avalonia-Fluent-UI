using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaFluentUI.UI.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Helpers;
using Gallery.Messages;
using Gallery.Services;

namespace Gallery.Controls;

public class ViewBase : ContentControl 
{
    protected SmoothScrollViewer? ScrollViewer { get; private set; }
    protected Dictionary<string, CodeCard> CodeCards { get; set; }
    private string Page { get; }
    
    public ViewBase(string page = "")
    {
        Page = page;
        // Console.WriteLine("Init View Base");
        
        WeakReferenceMessenger.Default.Register<JumpToControlMessage>(this, OnJumpToControl);
    }

    protected override Type StyleKeyOverride => typeof(ViewBase);

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ViewBase, string?>(nameof(Title));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var button = e.NameScope.Find<Button>("ToggleThemeButton");
        if (button != null)
        {
            button.Click += OnToggleThemeClicked;
        }

        ScrollViewer = e.NameScope.Find<SmoothScrollViewer>("SmoothScrollViewer");
        
        //////
        var db = e.NameScope.Find<Button>("DocumentButton")!;
        var sb = e.NameScope.Find<Button>("SourceCodeButton")!;

        db.Click -= OnClicked;
        sb.Click -= OnClicked;
        
        db.Click += OnClicked;
        sb.Click += OnClicked;
        
        base.OnApplyTemplate(e);
    }

    private void OnClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl("https://github.com/HiyorinI/AvaloniaFluentUI.git");
    }

    private void OnToggleThemeClicked(object? sender, RoutedEventArgs e)
    { 
        ThemeService.ToggleTheme();
    }
    
    protected async Task ScrollTo(string name)
    {
        if (CodeCards.TryGetValue(name, out var codeCard))
        {
            // await ScrollViewer?.Presenter?.ScrollTo(SmoothScrollDirection.Y, GetVector(codeCard).Y);
            if (codeCard.IsAttachedToVisualTree())
            {
                ScrollViewer?.Offset = GetVector(codeCard);
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ScrollViewer?.Offset = GetVector(codeCard);
                }, DispatcherPriority.Loaded);
            }
#if DEBUG
            Debug.WriteLine($"Scroll to: {GetVector(codeCard).Y}");
#endif
        }
    }

    protected void OnJumpToControl(object recipient, JumpToControlMessage message)
    {
        if (message.Page == this.Page && message.Name != null)
        {
            _=ScrollTo(message.Name);
#if DEBUG
            Debug.WriteLine($"Scroll of name: {message.Name}");
#endif
        }
    }

    protected Vector GetVector(Control? control)
    {
        var visual = ScrollViewer?.Content as Visual;
        if (visual != null)
        {
            var point = control?.TranslatePoint(new Point(0, 0), visual);
            if (point.HasValue)
            {
                return new Vector(0, point.Value.Y);
            }
        }

        return new Vector();
    }
}
