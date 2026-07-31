using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AvaloniaFluentUI.Controls;

public class SegmentedToggleView : SegmentedView
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<SegmentedToggleView, Orientation>(nameof(Orientation));

    /// <summary>
    /// 设置或获取当前的显示方向
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is SegmentedItem segmentedItem)
        {
            segmentedItem.Classes.Add("Toggle");
            if (segmentedItem.Content == null)
            {
                segmentedItem.Content = item;
            }
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

     protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        UpdateSelectedIndicatorPosition();
        return size;
    }

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
            Duration = TimeSpan.FromMilliseconds(150),
            FillMode = FillMode.Forward,
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
