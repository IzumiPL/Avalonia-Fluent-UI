using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_HEADERS_AREA,         Type = typeof(Panel))]
[TemplatePart(Name = PART_SELECTED_INDICATOR,   Type = typeof(SelectionIndicator))]
public class SegmentedView : SelectingItemsControl
{
    /// <summary>
    ///     Defines the <see cref="SelectionIndicatorBackground" /> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SelectionIndicatorBackgroundProperty =
        AvaloniaProperty.Register<SegmentedView, IBrush?>(nameof(SelectionIndicatorBackground));

    /// <summary>
    ///     Defines the <see cref="SelectionIndicatorBrush" /> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SelectionIndicatorBrushProperty =
        AvaloniaProperty.Register<SegmentedView, IBrush?>(nameof(SelectionIndicatorBrush));

    /// <summary>
    ///     Defines the <see cref="SelectionIndicatorWidthIsFixed" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> SelectionIndicatorWidthIsFixedProperty =
        AvaloniaProperty.Register<SegmentedView, bool>(nameof(SelectionIndicatorWidthIsFixed));

    public bool SelectionIndicatorWidthIsFixed
    {
        get => GetValue(SelectionIndicatorWidthIsFixedProperty);
        set => SetValue(SelectionIndicatorWidthIsFixedProperty, value);
    }

    public IBrush? SelectionIndicatorBrush
    {
        get => GetValue(SelectionIndicatorBrushProperty);
        set => SetValue(SelectionIndicatorBrushProperty, value);
    }
    
    public IBrush? SelectionIndicatorBackground
    {
        get => GetValue(SelectionIndicatorBackgroundProperty);
        set => SetValue(SelectionIndicatorBackgroundProperty, value);
    }
    
    protected SelectionIndicator? _selectedIndicator;
    protected Panel? _headersArea;

    protected CancellationTokenSource? _cts;
    protected TranslateTransform _transform = new TranslateTransform();
    
    private const string PART_SELECTED_INDICATOR = "PART_SelectedIndicator";
    private const string PART_HEADERS_AREA = "PART_HeadersArea";

    public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromMilliseconds(150);
    public Easing AnimationEasing { get; set; } = new CubicEaseOut();

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        => NeedsContainer<SegmentedItem>(item, out recycleKey);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? ecycleKey)
        => new SegmentedItem();

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is SegmentedItem segItem && segItem.Content == null)
        {
            segItem.Content = item;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        _selectedIndicator = e.NameScope.Find<SelectionIndicator>(PART_SELECTED_INDICATOR);
        _headersArea = e.NameScope.Find<Panel>(PART_HEADERS_AREA);

        if (e.NameScope.Find<SingleDirectionScrollViewer>("PART_ScrollViewer") is {} scroller)
        {
            scroller.RemoveHandler(RequestBringIntoViewEvent, OnScrollViewerRequestBringIntoViewEventChanged);
            scroller.AddHandler(RequestBringIntoViewEvent, OnScrollViewerRequestBringIntoViewEventChanged);
        }
    }

    private void OnScrollViewerRequestBringIntoViewEventChanged(object? sender, RequestBringIntoViewEventArgs e) => e.Handled = true;

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        UpdateSelectedIndicatorPosition();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedItemProperty)
        {
            UpdateSelectedIndicatorPosition();
        }
    }

    protected virtual void SyncContainerSelection(object? selectedItem)
    {
        foreach (var item in Items)
        {
            if (ContainerFromItem(item) is SegmentedItem container)
            {
                container.IsSelected = selectedItem != null && Equals(item, selectedItem);
            }
        }
    }
    
    protected async virtual void RunSliderAnimation(Point position)
    {
        if (_selectedIndicator == null) { return; }

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _selectedIndicator.RenderTransform = _transform;

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
                        new Setter(TranslateTransform.XProperty, _transform.X)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, position.X)
                    }
                }
            }
        };

        await animation.RunAsync(_selectedIndicator, _cts.Token);
    }

    protected virtual void UpdateSelectedIndicatorPosition()
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
            var width = container.Bounds.Width;
            var height = container.Bounds.Height;
            
            RunSliderAnimation(transform.Value.Transform(new Point(0, 0)));
            _selectedIndicator.Width = width;
            _selectedIndicator.Height = height;
            if (!SelectionIndicatorWidthIsFixed)
            {
                _selectedIndicator.IndicatorWidth = width / 3;
            }
        }
    }
}

public class SegmentedItem : ContentControl
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<SegmentedItem>();

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
            if (Parent is SegmentedView segmented)
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
