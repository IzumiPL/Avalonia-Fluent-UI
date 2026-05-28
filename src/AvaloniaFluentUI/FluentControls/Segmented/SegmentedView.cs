using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_SELECTED_INDICATOR, Type = typeof(Rectangle))]
[TemplatePart(Name = PART_HEADERS_ARES, Type = typeof(Control))]
public class SegmentedView : SelectingItemsControl
{
    protected Rectangle? _selectedIndicator;
    protected Control? _headersArea;
    
    private const string PART_SELECTED_INDICATOR = "PART_SelectedIndicator";
    private const string PART_HEADERS_ARES = "PART_HeadersArea";

    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey)
        => NeedsContainer<SegmentedItem>(item, out recycleKey);

    protected override Control CreateContainerForItemOverride(object item, int index, object recycleKey)
        => new SegmentedItem();

    protected override void PrepareContainerForItemOverride(Control container, object item, int index)
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
        
        _selectedIndicator = e.NameScope.Find<Rectangle>(PART_SELECTED_INDICATOR);
        _headersArea = e.NameScope.Find<Control>(PART_HEADERS_ARES);
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
        // container.BringIntoView();
        _selectedIndicator.IsVisible = true;

        var transform = container.TransformToVisual(_headersArea);
        if (transform.HasValue)
        {
            var width = container.Bounds.Width;
            var indicatorWidth = width / 3;
            var position = transform.Value.Transform(new Point(0, 0));
            _selectedIndicator.Margin = new Thickness(position.X + (width - indicatorWidth) / 2, 0, 0, 0);
            _selectedIndicator.Width = indicatorWidth;
        }
    }
}

public class SegmentedItem : ContentControl
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<SegmentedItem>();

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

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.Handled && IsEffectivelyEnabled)
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
