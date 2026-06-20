using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace AvaloniaFluentUI.Controls;

[PseudoClasses(PC_HAS_PLACEHOLDER, PC_PRESSED)]
[TemplatePart(Name = PART_MULTI_SELECTION_POPUP, Type = typeof(Popup))]
[TemplatePart(Name = PART_MULTI_SELECTION_VIEW, Type = typeof(MultiSelectionView))]
public class MultiSelectionComboBox : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IEnumerable?>(nameof(ItemsSource));

    public static readonly StyledProperty<IList?> SelectedItemsProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, IList?>(nameof(SelectedItems));

    public static readonly StyledProperty<string> PlaceholderTextProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, string>(nameof(PlaceholderText));

    public static readonly StyledProperty<double> ViewPortMaxHeightProperty =
        AvaloniaProperty.Register<MultiSelectionComboBox, double>(nameof(ViewPortMaxHeight));
    
    public double ViewPortMaxHeight
    {
        get => GetValue(ViewPortMaxHeightProperty);
        set => SetValue(ViewPortMaxHeightProperty, value);
    }

    public string PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

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
    private MultiSelectionView _multiSelectionView;

    private readonly AvaloniaList<MultiSelectionComboBoxItem> _items = new ();
    
    [Content]
    public AvaloniaList<MultiSelectionComboBoxItem> Items => _items;

    private const string PC_PRESSED = ":pressed";
    private const string PC_HAS_PLACEHOLDER = ":hasplaceholder";
    
    private const string PART_MULTI_SELECTION_POPUP = "PART_MultiSelectionPopup";
    private const string PART_MULTI_SELECTION_VIEW = "PART_MultiSelectionView";

    public MultiSelectionComboBox()
    {
        AddHandler(MultiSelectionDisplayItem.RemoveClickEvent, OnDisplayItemRemoveClick);
        PseudoClasses.Add(PC_HAS_PLACEHOLDER);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _multiSelectionView?.SelectionChanged -= OnSelectionChanged;
        
        _multiSelectionPopup = e.NameScope.Find<Popup>(PART_MULTI_SELECTION_POPUP);
        _multiSelectionView = e.NameScope.Find<MultiSelectionView>(PART_MULTI_SELECTION_VIEW);
        
        if (SelectedItems == null)
        {
            SetCurrentValue(SelectedItemsProperty, new ObservableCollection<object>());
        }
        
        if (ItemsSource == null && _items.Count > 0)
        {
            var data = new List<object>(_items.Count);
            foreach (var item in _items)
            {
                var value = item.Content ?? item;
                data.Add(value);
                if (item.IsSelected && !SelectedItems.Contains(value)) 
                {
                    SelectedItems.Add(value);
                }
            }
            ItemsSource = data;
        }

        _multiSelectionView?.SelectionChanged += OnSelectionChanged;
        UpdatePlaceholderStatus();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdatePlaceholderStatus();
    }

    private async void UpdatePlaceholderStatus()
    {
        await Task.Yield();
        PseudoClasses.Set(PC_HAS_PLACEHOLDER, SelectedItems?.Count == 0);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedItemsProperty)
        {
            UpdatePlaceholderStatus();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        PseudoClasses.Add(PC_PRESSED);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        PseudoClasses.Remove(PC_PRESSED);
        if (_multiSelectionPopup != null)
        {
            _multiSelectionPopup.Width = Bounds.Width;
            _multiSelectionPopup.IsOpen = true;
        }
    }

    private void OnDisplayItemRemoveClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is MultiSelectionDisplayItem displayItem && SelectedItems != null)
        {
            SelectedItems.Remove(displayItem.Content);
            UpdatePlaceholderStatus();
        }
    }
}
