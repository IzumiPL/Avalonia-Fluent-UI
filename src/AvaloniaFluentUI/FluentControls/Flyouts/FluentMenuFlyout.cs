using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaFluentUI.Animations;

namespace AvaloniaFluentUI.Controls;

public class FluentMenuFlyout : MenuFlyout
{
    protected override Control CreatePresenter()
    {
        var presenter =  base.CreatePresenter();
        presenter.Margin = new Thickness(0, 5, 0, 0);
        return presenter;
    }

    // protected override void OnOpening(CancelEventArgs args)
    // {
    //     if (Popup.Child is {} presenter)
    //     {
    //         presenter.Opacity = 0;
    //         presenter.RenderTransform = null;
    //     }
    //
    //     base.OnOpening(args);
    // }

    protected override void OnOpened()
    {
        base.OnOpened();
        
        if (Popup.Child is {} presenter)
        {
            _=FluentAnimation.SlideInAsync(presenter, -24d, TranslateTransform.YProperty);
        }
    }
}
