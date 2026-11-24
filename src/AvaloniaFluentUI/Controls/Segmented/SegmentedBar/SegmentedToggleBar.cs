using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AvaloniaFluentUI.Controls;

public class SegmentedToggleBar : SegmentedBar 
{ 
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<SegmentedToggleBar, Orientation>(nameof(Orientation));

    /// <summary>
    /// 设置或获取当前的显示方向
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) 
        => NeedsContainer<SegmentedToggleBarItem>(item, out recycleKey);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? ecycleKey)
        => new SegmentedToggleBarItem();

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is SegmentedToggleBarItem segItem && segItem.Content == null)
        {
            segItem.Content = item;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OrientationProperty)
        {
            InvalidateMeasure(); 
            UpdateSelectedIndicatorPosition();
        }
    }

    // protected override Size ArrangeOverride(Size finalSize)
    // {
        // var size = base.ArrangeOverride(finalSize);
        // UpdateSelectedIndicatorPosition();
        // return size;
    // }

    protected async override void RunSliderAnimation(Point position)
    {
        if (_selectedIndicator == null) { return; }

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _selectedIndicator.RenderTransform = _transform;

        AvaloniaProperty property;
        double startValue;
        double endValue;
        if (Orientation == Orientation.Horizontal)
        {
            property = TranslateTransform.XProperty;
            startValue = _transform.X;
            endValue = position.X;
        }
        else
        {
            property = TranslateTransform.YProperty;
            startValue = _transform.Y;
            endValue = position.Y;
        }
        
        var animation = new Animation
        {
            Duration = SliderDuration,
            FillMode = FillMode.Forward,
            Easing = SliderEasing,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = 
                    {
                        new Setter(property, startValue)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(property, endValue)
                    }
                }
            }
        };

        await animation.RunAsync(_selectedIndicator, _cts.Token);
    }

    protected override void UpdateSelectedIndicatorPosition()
    {
        if (_selectedIndicator == null || _headersArea == null)
            return;

        var selectedItem = SelectedItem;
        if (selectedItem == null || Items.Count == 0)
        {
            _selectedIndicator.IsVisible = false;
            return;
        }

        var container = ContainerFromItem(selectedItem);
        if (container == null || container.Bounds.Width <= 0)
        {
            _selectedIndicator.IsVisible = false;
            return;
        }

        _selectedIndicator.IsVisible = true;

        var transform = container.TransformToVisual(_headersArea);
        if (transform.HasValue)
        {
            double width = container.Bounds.Width;
            double height = container.Bounds.Height;
            
            RunSliderAnimation(transform.Value.Transform(new Point(0, 0)));
            _selectedIndicator.Width = width;
            _selectedIndicator.Height = height;
        }
    }
}


public class SegmentedToggleBarItem : SegmentedBarItem 
{
    protected override void SyncSelectedItem(PointerReleasedEventArgs e)
    {
        var pointe = e.GetPosition(this);
        if (!e.Handled && IsEffectivelyEnabled && new Rect(Bounds.Size).Contains(pointe))
        {
            if (Parent is SegmentedToggleBar segmented)
            {
                var dataItem = segmented.ItemFromContainer(this);
                if (dataItem != null)
                {
                    segmented.SelectedItem = dataItem;
                }
            }
        }
    }
}
