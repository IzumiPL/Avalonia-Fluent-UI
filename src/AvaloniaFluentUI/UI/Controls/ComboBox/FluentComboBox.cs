using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace AvaloniaFluentUI.UI.Controls;

public class FluentComboBox : ComboBox
{
    private Animation? _animation;
    private Border? _border;
    private Popup? _popup;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_popup != null) _popup.Opened -= OnPopup;

        _popup = e.NameScope.Get<Popup>("PART_Popup");
        _popup.Opened += OnPopup;
        _border = _popup.Child as Border;
        InitTransform();

    }

    private void InitTransform()
    {
        if (_border == null) return;
        _border.RenderTransformOrigin = new RelativePoint(0.5, 0, RelativeUnit.Relative);

        var scale = new ScaleTransform(1.0, 1.0);
        var translate = new TranslateTransform(0, 0);
        _border.RenderTransform = new TransformGroup { Children = { scale, translate } };
        
        _animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(150),
            Easing = new CircularEaseOut(),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0.0),
                    Setters =
                    {
                        new Setter(OpacityProperty, 0.0),
                        new Setter(ScaleTransform.ScaleYProperty, 0.4),
                        new Setter(TranslateTransform.YProperty, -Bounds.Height)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters =
                    {
                        new Setter(OpacityProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0),
                        new Setter(TranslateTransform.YProperty, 0)
                    }
                }
            }
        };
    }

    private void OnPopup(object? sender, EventArgs e)
    {
        if (_border != null) _animation?.RunAsync(_border);
    }
}
