using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaFluentUI.UI.Controls;

public class MultiSelectionView : SelectingItemsControl
{
    public static readonly StyledProperty<IList?> SelectedItemsProperty =
        AvaloniaProperty.Register<MultiSelectionView, IList?>(nameof(SelectedItems));

    public IList? SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public MultiSelectionView()
    {
        SelectionMode = SelectionMode.Multiple;
    }

    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey)
        => NeedsContainer<MultiSelectionComboBoxItem>(item, out recycleKey);

    protected override Control CreateContainerForItemOverride(object item, int index, object recycleKey)
        => new MultiSelectionComboBoxItem();

    protected override void PrepareContainerForItemOverride(Control container, object item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is MultiSelectionComboBoxItem multiItem)
        {
            multiItem.Content = item;
            multiItem.PropertyChanged -= OnContainerPropertyChanged;
            multiItem.PropertyChanged += OnContainerPropertyChanged;
            multiItem.IsSelected = SelectedItems?.Contains(item) ?? false;
        }
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        if (container is MultiSelectionComboBoxItem item)
        {
            item.PropertyChanged -= OnContainerPropertyChanged;
        }
        base.ClearContainerForItemOverride(container);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedItemsProperty)
        {
            if (change.OldValue is INotifyCollectionChanged oldColl)
                oldColl.CollectionChanged -= OnSelectedItemsCollectionChanged;
            if (change.NewValue is INotifyCollectionChanged newColl)
                newColl.CollectionChanged += OnSelectedItemsCollectionChanged;

            SyncSelectionStates();
        }
    }

    private void OnContainerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ListBoxItem.IsSelectedProperty && sender is Control container)
        {
            var item = ItemFromContainer(container);
            var selectedItems = SelectedItems;
            if (item == null || selectedItems == null)
                return;

            bool isSelected = (bool)e.NewValue!;
            if (isSelected && !selectedItems.Contains(item))
            {
                selectedItems.Add(item);
            }
            else if (!isSelected && selectedItems.Contains(item))
            {
                selectedItems.Remove(item);
            }
        }
    }

    private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncSelectionStates();
    }

    private void SyncSelectionStates()
    {
        foreach (var item in Items)
        {
            if (ContainerFromItem(item) is MultiSelectionComboBoxItem container)
            {
                container.IsSelected = SelectedItems?.Contains(item) ?? false;
            }
        }
    }
}
