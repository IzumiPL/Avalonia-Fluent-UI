using System;
using System.Collections;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.UI.Controls;

[TemplatePart(Name = "PART_DropDownButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_MultiSelectionPopup", Type = typeof(Popup))]
[TemplatePart(Name = "PART_MultiSelectionView", Type = typeof(MultiSelectionView))]
public class MultiSelectionComboBox : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IEnumerable?>(nameof(ItemsSource));

    public static readonly StyledProperty<IList?> SelectedItemsProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IList?>(nameof(SelectedItems));

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IList? SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    private Popup? _multiSelectionPopup;
    private Button? _dropDownButton;
    private MultiSelectionView? _multiSelectionView;

    public MultiSelectionComboBox()
    {
        AddHandler(MultiSelectionDisplayItem.RemoveClickEvent, OnDisplayItemRemoveClick);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_dropDownButton != null)
        {
            _dropDownButton.Click -= OnDropDownButtonClick;
        }

        _dropDownButton = e.NameScope.Find<Button>("PART_DropDownButton");
        _multiSelectionPopup = e.NameScope.Find<Popup>("PART_MultiSelectionPopup");
        _multiSelectionView = e.NameScope.Find<MultiSelectionView>("PART_MultiSelectionView");

        if (_multiSelectionPopup != null)
        {
            _multiSelectionPopup.PlacementTarget = this;
            _multiSelectionPopup.IsLightDismissEnabled = true;
        }

        if (_dropDownButton != null)
        {
            _dropDownButton.Click += OnDropDownButtonClick;
        }

        if (_multiSelectionView != null)
        {
            if (SelectedItems == null)
            {
                Console.WriteLine("等于 Null");
                SetCurrentValue(SelectedItemsProperty, new ObservableCollection<object>());
            }
            _multiSelectionView.ItemsSource = ItemsSource;
            _multiSelectionView.SelectedItems = SelectedItems;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty && _multiSelectionView != null)
        {
            _multiSelectionView.ItemsSource = ItemsSource;
        }
        else if (change.Property == SelectedItemsProperty && _multiSelectionView != null)
        {
            _multiSelectionView.SelectedItems = SelectedItems;
        }
    }

    private void OnDropDownButtonClick(object? sender, RoutedEventArgs e)
    {
        if (_multiSelectionPopup != null)
        {
            _multiSelectionPopup.Width = Bounds.Width;
            _multiSelectionPopup.IsOpen = !_multiSelectionPopup.IsOpen;
        }
    }

    private void OnDisplayItemRemoveClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is MultiSelectionDisplayItem displayItem && SelectedItems != null)
        {
            SelectedItems.Remove(displayItem.Content);
        }
    }
}
