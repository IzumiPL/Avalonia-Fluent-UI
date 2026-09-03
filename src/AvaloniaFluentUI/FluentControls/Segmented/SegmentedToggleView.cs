using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
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

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) 
        => NeedsContainer<SegmentedToggleItem>(item, out recycleKey);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? ecycleKey)
        => new SegmentedToggleItem();

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is SegmentedToggleItem segItem && segItem.Content == null)
        {
            segItem.Content = item;
        }
    }

    protected override void SyncContainerSelection(object? selectedItem)
    {
        foreach (var item in Items)
        {
            if (ContainerFromItem(item) is SegmentedToggleItem container)
            {
                container.IsSelected = selectedItem != null && Equals(item, selectedItem);
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
            Duration = AnimationDuration,
            FillMode = FillMode.Forward,
            Easing = AnimationEasing,
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

public class SegmentedToggleItem : ContentControl
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<SegmentedToggleItem>();

    /// <summary>
    /// 设置或获取当前项是否选中
    /// </summary>
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsSelectedProperty)
        {
            PseudoClasses.Set(":selected", change.GetNewValue<bool>());
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        var pointe = e.GetPosition(this);
        if (!e.Handled && IsEffectivelyEnabled && new Rect(Bounds.Size).Contains(pointe))
        {
            if (Parent is SegmentedToggleView segmented)
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
