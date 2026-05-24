using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaFluentUI.Media.Animation;

namespace AvaloniaFluentUI.Controls;

public class FluentMenuFlyout : MenuFlyout
{
    protected override Control CreatePresenter()
    {
        var presenter =  base.CreatePresenter();
        presenter.Margin = new Thickness(0, 5, 0, 0);
        return presenter;
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        
        if (Popup.Child is {} presenter)
        {
            _=FluentAnimation.SlideInAsync(presenter, -24d, TranslateTransform.YProperty);
        }
    }
}
