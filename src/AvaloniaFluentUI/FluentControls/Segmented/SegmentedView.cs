using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_SELECTED_INDICATOR, Type = typeof(SelectionIndicator))]
[TemplatePart(Name = PART_HEADERS_ARES, Type = typeof(Panel))]
public class SegmentedView : SelectingItemsControl
{
    protected SelectionIndicator? _selectedIndicator;
    protected Panel? _headersArea;
    
    protected TranslateTransform _transform = new TranslateTransform();
    
    private const string PART_SELECTED_INDICATOR = "PART_SelectedIndicator";
    private const string PART_HEADERS_ARES = "PART_HeadersArea";

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
        _headersArea = e.NameScope.Find<Panel>(PART_HEADERS_ARES);
    }

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
            SyncContainerSelection(change.NewValue);
            UpdateSelectedIndicatorPosition();
        }
    }

    private void SyncContainerSelection(object selectedItem)
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

        _selectedIndicator.RenderTransform = _transform;

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

        await animation.RunAsync(_selectedIndicator);
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
            var indicatorWidth = width / 3;
            
            RunSliderAnimation(transform.Value.Transform(new Point(0, 0)));
            _selectedIndicator.Width = width;
            _selectedIndicator.Height = height;
            _selectedIndicator.IndicatorWidth = indicatorWidth;
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
