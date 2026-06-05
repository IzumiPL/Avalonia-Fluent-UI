using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_PLACEHOLDER, Type = typeof(TextBlock))]
[TemplatePart(Name = PART_DROP_DOWN_BUTTON, Type = typeof(Button))]
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

    private TextBlock? _watermark;
    private Popup? _multiSelectionPopup;
    private Button? _dropDownButton;
    private MultiSelectionView? _multiSelectionView;
    
    private const string PART_PLACEHOLDER = "PART_Placeholder";
    private const string PART_DROP_DOWN_BUTTON = "PART_DropDownButton";
    private const string PART_MULTI_SELECTION_POPUP = "PART_MultiSelectionPopup";
    private const string PART_MULTI_SELECTION_VIEW = "PART_MultiSelectionView";

    public MultiSelectionComboBox()
    {
        AddHandler(MultiSelectionDisplayItem.RemoveClickEvent, OnDisplayItemRemoveClick);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _dropDownButton?.Click -= OnDropDownButtonClick;
        _multiSelectionView?.SelectionChanged -= OnSelectedItemsChanged;

        _watermark = e.NameScope.Find<TextBlock>(PART_PLACEHOLDER);
        _dropDownButton = e.NameScope.Find<Button>(PART_DROP_DOWN_BUTTON);
        _multiSelectionPopup = e.NameScope.Find<Popup>(PART_MULTI_SELECTION_POPUP);
        _multiSelectionView = e.NameScope.Find<MultiSelectionView>(PART_MULTI_SELECTION_VIEW);

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
                SetCurrentValue(SelectedItemsProperty, new ObservableCollection<object>());
            }
            _multiSelectionView.ItemsSource = ItemsSource;
            _multiSelectionView.SelectedItems = SelectedItems;

            _watermark?.IsVisible = _multiSelectionView.SelectedItems?.Count == 0;
            _multiSelectionView.SelectionChanged += OnSelectedItemsChanged;
        }
    }

    private async void OnSelectedItemsChanged(object sender, SelectionChangedEventArgs e)
    {
        await Task.Yield();
        _watermark?.IsVisible = _multiSelectionView?.SelectedItems?.Count == 0;
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

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
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
        }
    }
}
