using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_CURRENT_PRESENTER,    Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_NEXT_PRESENTER,       Type = typeof(ContentPresenter))]
public class StackContent : ContentControl
{
    /// <summary>
    ///     Defines the <see cref="IsBack" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsBackProperty =
        AvaloniaProperty.Register<StackContent, bool>(nameof(IsBack));

    public bool IsBack
    {
        get => GetValue(IsBackProperty);
        set => SetValue(IsBackProperty, value);
    }

    public double EnterDuration { get; set; } = 600;
    public double ExitDuration { get; set; } = 250;
    
    private ContentPresenter? _currentPresenter;
    private ContentPresenter? _nextPresenter; 
    
    private const string PART_CURRENT_PRESENTER = "PART_CurrentPresenter";
    private const string PART_NEXT_PRESENTER = "PART_NextPresenter";
    
    private CancellationTokenSource? _cts;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        _currentPresenter = e.NameScope.Find<ContentPresenter>(PART_CURRENT_PRESENTER);
        _nextPresenter = e.NameScope.Find<ContentPresenter>(PART_NEXT_PRESENTER);

        _currentPresenter?.Content = Content;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            if (IsBack)
            {
                _ = RunExitContentAnimation(change.OldValue, change.NewValue);
            }
            else
            {
                _ = RunEnterContentAnimation(change.NewValue);
            }
        }
    }

    private CancellationToken ResetToken()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        
        return _cts.Token;
    }

    private async Task RunEnterContentAnimation(object? newContent)
    {
        if (_currentPresenter is null)
            return;

        _nextPresenter?.IsVisible = false;
        _currentPresenter.Content = newContent;
        var token = ResetToken();

        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(EnterDuration), 
            Easing = new SplineEasing(0.1, 0.9, 0.2, 1.0), 
            FillMode = FillMode.Forward, 
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d), 
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0d), 
                        new Setter(TranslateTransform.YProperty, 150d)
                    }
                }, 
                new KeyFrame
                {
                    Cue = new Cue(1d), 
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d), 
                        new Setter(TranslateTransform.YProperty, 0d)
                    }
                }
            }
        };

        await animation.RunAsync(_currentPresenter, token);
    }

    private async Task RunExitContentAnimation(object? oldContent, object? newContent)
    {
        if (_currentPresenter is null ||  _nextPresenter is null)
            return;
        
        var token = ResetToken();
        
        _currentPresenter.IsVisible = false;
        _currentPresenter.Content = null;
        _nextPresenter.IsVisible = true;
        _nextPresenter.Content = oldContent;

        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(ExitDuration),
            Easing = new CubicEaseIn(),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters =
                    {
                        new Setter(TranslateTransform.YProperty, 0d),
                        new Setter(Visual.OpacityProperty, 1d)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(TranslateTransform.YProperty, Bounds.Height / 2),
                        new Setter(Visual.OpacityProperty, 0d)
                    }
                }
            }
        };

        await animation.RunAsync(_nextPresenter, token);
        _nextPresenter.IsVisible = false;
        _nextPresenter.Content = null;
        
        _currentPresenter.IsVisible = true;
        _currentPresenter.Content = newContent;

        var opaAnimation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(120),
            Easing = new CubicEaseOut(),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d), 
                    Setters = { new Setter(Visual.OpacityProperty, 0d), }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d), 
                    Setters = { new Setter(Visual.OpacityProperty, 1d), }
                }
            }
        };
        
        await opaAnimation.RunAsync(_currentPresenter, token);
    }
}
