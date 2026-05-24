using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AvaloniaFluentUI.Controls;

public class SegmentedToggleView : SegmentedView
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<SegmentedToggleView, Orientation>(nameof(Orientation));

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected override void PrepareContainerForItemOverride(Control container, object item, int index)
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
        // container.BringIntoView();
        _selectedIndicator.IsVisible = true;

        var transform = container.TransformToVisual(_headersArea);
        if (transform.HasValue)
        {
            double width = container.Bounds.Width;
            double height = container.Bounds.Height;
            var position = transform.Value.Transform(new Point(0, 0));
            var value = Orientation == Orientation.Horizontal ? new Thickness(position.X, 0, 0, 0) : new Thickness(Margin.Left, position.Y, 0, 0);

            _selectedIndicator.Margin = value;
            _selectedIndicator.Width = width;
            _selectedIndicator.Height = height;
        }
    }
}
