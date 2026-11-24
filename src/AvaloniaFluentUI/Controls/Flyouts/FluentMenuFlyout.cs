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
        return presenter;
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        
        if (Popup.Child is {} presenter)
        {
            Popup.VerticalOffset = -2;
            if (Target != null)
            {
                presenter.MinWidth = Target.Bounds.Width + 10;
            }

            FluentAnimation.SlideInAsync(presenter, -24d, TranslateTransform.YProperty);
        }
    }
}
