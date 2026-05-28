using System;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages.MainWindowMessages;
using Gallery.Views;

namespace Gallery.Services;

public class MainWindowMessageHandler :
    IRecipient<PageAnimationStatusChangedMessage>, 
    IRecipient<PageAnimationTypeChangedMessage>,
    // IRecipient<WindowTopmostStatusMessage>,
    // IRecipient<NavigationGoBackMessage>,
    IRecipient<EnabledWindowEffectMessage>,
    IRecipient<EnabledBackgroundImageMessage>
{
    private readonly MainWindow _window;

    public MainWindowMessageHandler(MainWindow window)
    {
        _window = window;
    }

    /// <summary>
    ///     Register Message
    /// </summary>
    public void RegisterAllMessage()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }
    
    /// <summary>
    ///     On Page AnimationChanged Message
    /// </summary>
    /// <param name="message"></param>
    public void Receive(PageAnimationTypeChangedMessage message)
    {
        IPageTransition? pageTransition = message.Type switch
        {
            "Crossfade" => new CrossFade { Duration = TimeSpan.FromMilliseconds(message.Duration) },
            "PageSlide" => new PageSlide { Orientation = message.SlideAxis, SlideInEasing = new CubicEaseIn(), Duration = TimeSpan.FromMilliseconds(message.Duration) },
            "Rotate3DTransition" => new Rotate3DTransition { Orientation = message.SlideAxis, Duration = TimeSpan.FromMilliseconds(message.Duration) },
            _ => null
        };
        _window.SetPageTransition(pageTransition);
    }

    /// <summary>
    ///     Navigation GoBack Message
    /// </summary>
    /// <param name="message"></param>
    public void Receive(NavigationGoBackMessage message)
    {
        // _window.NavigationViewPanel.SetCurrentItemByContent(message.Value);
    }

    /// <summary>
    ///     On Page AnimationEnableChanged Message
    /// </summary>
    /// <param name="message"></param>
    public void Receive(PageAnimationStatusChangedMessage message)
    {
        _window.SetPageTransition(message.IsEnabled ? _window.CurrentPageTransition : null);
    }

    /// <summary>
    ///     On Window TopmostState IsEnableChanged Message
    /// </summary>
    /// <param name="message"></param>
    public void Receive(WindowTopmostStatusMessage message)
    {
        _window.Topmost = message.Value;
    }
    
    /// <summary>
    /// On Window Effect EnableChanged Message
    /// </summary>
    /// <param name="message"></param>
    public void Receive(EnabledWindowEffectMessage message)
    {
        _window.EnableWindowEffect(message.IsEnabled);
    }

    public void Receive(EnabledBackgroundImageMessage message)
    {
        _window.SetBackgroundImageIsVisible(message.IsVisible);
    }
}
