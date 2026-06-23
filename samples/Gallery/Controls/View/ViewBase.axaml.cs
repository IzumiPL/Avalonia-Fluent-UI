using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Styling;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Helpers;
using Gallery.Messages;

namespace Gallery.Controls;

public class ViewBase : ContentControl 
{
    protected  Dictionary<string, CodeCard> CodeCards { get; set; }
    private string Page { get; }
    
    public ViewBase(string page)
    {
        Page = page;
        
        WeakReferenceMessenger.Default.Register<JumpToControlMessage>(this, OnJumpToControl);
    }

    public ViewBase() { }

    protected override Type StyleKeyOverride => typeof(ViewBase);

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ViewBase, string?>(nameof(Title));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private Button? _toggleThemeButton;
    private Button? _documentButton;
    private Button? _sourceCodeButton;
    private SmoothScrollViewer? _scrollViewer;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _toggleThemeButton?.Click -= OnToggleThemeClicked;
        _documentButton?.Click -= OnDocumentButtonClicked;
        _sourceCodeButton?.Click -= OnSourceCodeButtonClicked;
        
        _toggleThemeButton = e.NameScope.Find<Button>("ToggleThemeButton");
        _scrollViewer = e.NameScope.Find<SmoothScrollViewer>("SmoothScrollViewer");
        _documentButton = e.NameScope.Find<Button>("DocumentButton")!;
        _sourceCodeButton = e.NameScope.Find<Button>("SourceCodeButton")!;

        _toggleThemeButton?.Click += OnToggleThemeClicked;
       _documentButton?.Click += OnDocumentButtonClicked;
       _sourceCodeButton?.Click += OnSourceCodeButtonClicked;
        
        base.OnApplyTemplate(e);
    }

    private void OnDocumentButtonClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl("https://docs.mikuas.top/");
    }

    private void OnSourceCodeButtonClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl("https://github.com/HiyorinI/AvaloniaFluentUI.git");
    }

    private void OnToggleThemeClicked(object? sender, RoutedEventArgs e) => AvaloniaFluentTheme.Instance.ToggleTheme();
    
    protected async void ScrollTo(string name)
    {
        if (CodeCards.TryGetValue(name, out var codeCard))
        {
            if (codeCard.IsAttachedToVisualTree())
            {
                _scrollViewer?.Offset = GetVector(codeCard);
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _scrollViewer?.Offset = GetVector(codeCard);
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
            ScrollTo(message.Name);
#if DEBUG
            Debug.WriteLine($"Scroll of name: {message.Name}");
#endif
        }
    }

    protected Vector GetVector(Control? control)
    {
        var visual = _scrollViewer?.Content as Visual;
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
