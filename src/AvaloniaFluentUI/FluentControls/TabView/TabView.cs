using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.Utilities;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Collections;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Controls.Primitives;
using System.Collections;


namespace AvaloniaFluentUI.Controls;

/// <summary>
/// A control used to display a set of tabs and their respective content
/// </summary>
[PseudoClasses(PC_TOP, PC_LEFT, PC_BOTTOM, PC_RIGHT)]
[PseudoClasses(SharedPseudoclasses.s_pcNoBorder, SharedPseudoclasses.s_pcBorderLeft, SharedPseudoclasses.s_pcBorderRight, PC_SINGLE_BORDER)]
[TemplatePart(Name = ADD_BUTTON,                Type = typeof(Button))]
[TemplatePart(Name = TAB_LIST_VIEW,             Type = typeof(TabViewListView))]
[TemplatePart(Name = TAB_CONTAINER_GRID,        Type = typeof(Grid))]
[TemplatePart(Name = TAA_CONTENT_PRESENTER,     Type = typeof(ContentPresenter))]
[TemplatePart(Name = RIGHT_CONTENT_PRESENTER,   Type = typeof(ContentPresenter))]
public class TabView : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="TabWidthMode"/> property
    /// </summary>
    public static readonly StyledProperty<TabViewWidthMode> TabWidthModeProperty =
        AvaloniaProperty.Register<TabView, TabViewWidthMode>(nameof(TabWidthMode),
            defaultValue: TabViewWidthMode.Equal);

    /// <summary>
    /// Defines the <see cref="CloseButtonOverlayMode"/> property
    /// </summary>
    public static readonly StyledProperty<TabViewCloseButtonOverlayMode> CloseButtonOverlayModeProperty =
        AvaloniaProperty.Register<TabView, TabViewCloseButtonOverlayMode>(nameof(CloseButtonOverlayMode),
            defaultValue: TabViewCloseButtonOverlayMode.Auto);

    /// <summary>
    /// Definse the <see cref="TabStripHeader"/> property
    /// </summary>
    public static readonly StyledProperty<object> TabStripHeaderProperty =
        AvaloniaProperty.Register<TabView, object>(nameof(TabStripHeader));

    /// <summary>
    /// Define the <see cref="TabStripHeaderTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> TabStripHeaderTemplateProperty =
        AvaloniaProperty.Register<TabView, IDataTemplate>(nameof(TabStripHeaderTemplate));

    /// <summary>
    /// Defines the <see cref="TabStripFooter"/> property
    /// </summary>
    public static readonly StyledProperty<object> TabStripFooterProperty =
        AvaloniaProperty.Register<TabView, object>(nameof(TabStripFooter));

    /// <summary>
    /// Defines the <see cref="TabStripFooterTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> TabStripFooterTemplateProperty =
        AvaloniaProperty.Register<TabView, IDataTemplate>(nameof(TabStripFooterTemplate));

    /// <summary>
    /// Defines the <see cref="IsAddTabButtonVisible"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsAddTabButtonVisibleProperty =
        AvaloniaProperty.Register<TabView, bool>(nameof(IsAddTabButtonVisible), true);

    /// <summary>
    /// Defines the <see cref="AddTabButtonCommand"/> property
    /// </summary>
    public static readonly StyledProperty<ICommand> AddTabButtonCommandProperty =
        AvaloniaProperty.Register<TabView, ICommand>(nameof(AddTabButtonCommand));

    /// <summary>
    /// Defines the <see cref="AddTabButtonCommandParameter"/> property
    /// </summary>
    public static readonly StyledProperty<object> AddTabButtonCommandParameterProperty =
        AvaloniaProperty.Register<TabView, object>(nameof(AddTabButtonCommandParameter));

    /// <summary>
    /// Defines the <see cref="TabItems"/> property
    /// </summary>
    public static readonly DirectProperty<TabView, IList> TabItemsProperty =
        AvaloniaProperty.RegisterDirect<TabView, IList>(nameof(TabItems),
            x => x.TabItems);

    /// <summary>
    /// Defines the <see cref="TabItemsSource"/> property
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> TabItemsSourceProperty =
        AvaloniaProperty.Register<TabView, IEnumerable?>(nameof(TabItemsSource));

    /// <summary>
    /// Defines the <see cref="TabItemTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> TabItemTemplateProperty =
        AvaloniaProperty.Register<TabView, IDataTemplate>(nameof(TabItemTemplate));

    /// <summary>
    /// Defines the <see cref="CanReorderTabs"/> property
    /// </summary>
    public static readonly StyledProperty<bool> CanReorderTabsProperty =
        AvaloniaProperty.Register<TabView, bool>(nameof(CanReorderTabs), true);

    /// <summary>
    /// Defines the <see cref="SelectedIndex"/> property
    /// </summary>
    public static readonly DirectProperty<TabView, int> SelectedIndexProperty =
        SelectingItemsControl.SelectedIndexProperty.AddOwner<TabView>(x => x.SelectedIndex,
            (x, v) => x.SelectedIndex = v);

    /// <summary>
    /// Defines the <see cref="SelectedItem"/> property
    /// </summary>
    public static readonly DirectProperty<TabView, object?> SelectedItemProperty =
        SelectingItemsControl.SelectedItemProperty.AddOwner<TabView>(x => x.SelectedItem,
            (x, v) => x.SelectedItem = v);

    /// <summary>
    /// Defines the <see cref="TabStripLocation"/> property
    /// </summary>
    public static readonly StyledProperty<TabViewTabStripLocation> TabStripLocationProperty =
        AvaloniaProperty.Register<TabView, TabViewTabStripLocation>(nameof(TabStripLocation));

    /// <summary>
    /// Defines the <see cref="IsVerticalPaneOpen"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsVerticalPaneOpenProperty = 
        AvaloniaProperty.Register<TabView, bool>(nameof(IsVerticalPaneOpen), defaultValue: true);

    /// <summary>
    /// Defines the <see cref="VerticalOpenPaneLength"/> property
    /// </summary>
    public static readonly StyledProperty<double> VerticalOpenPaneLengthProperty = 
        AvaloniaProperty.Register<TabView, double>(nameof(VerticalOpenPaneLength), defaultValue: 225d);

    /// <summary>
    /// Defines the <see cref="MinimumVerticalOpenPaneLength"/> property
    /// </summary>
    public static readonly StyledProperty<double> MinimumVerticalOpenPaneLengthProperty = 
        AvaloniaProperty.Register<TabView, double>(nameof(MinimumVerticalOpenPaneLength), defaultValue: 40d);

    /// <summary>
    /// Defines the <see cref="MaximumVerticalOpenPaneLength"/> property
    /// </summary>
    public static readonly StyledProperty<double> MaximumVerticalOpenPaneLengthProperty = 
        AvaloniaProperty.Register<TabView, double>(nameof(MaximumVerticalOpenPaneLength), defaultValue: 700d);

    /// <summary>
    /// Defines the <see cref="VerticalPaneDisplayMode"/> property
    /// </summary>
    public static readonly StyledProperty<SplitViewDisplayMode> VerticalPaneDisplayModeProperty = 
        AvaloniaProperty.Register<TabView, SplitViewDisplayMode>(nameof(VerticalPaneDisplayMode), defaultValue: SplitViewDisplayMode.Inline);

    
    /// <summary>
    /// Gets or sets how the tabs should be sized
    /// </summary>
    public TabViewWidthMode TabWidthMode
    {
        get => GetValue(TabWidthModeProperty);
        set => SetValue(TabWidthModeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates the behavior of the close button within tabs
    /// </summary>
    public TabViewCloseButtonOverlayMode CloseButtonOverlayMode
    {
        get => GetValue(CloseButtonOverlayModeProperty);
        set => SetValue(CloseButtonOverlayModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the content that is shown to the left of the tab strip
    /// </summary>
    public object TabStripHeader
    {
        get => GetValue(TabStripHeaderProperty);
        set => SetValue(TabStripHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the IDataTemplate used to dispaly the content of the TabStripHeader
    /// </summary>
    public IDataTemplate TabStripHeaderTemplate
    {
        get => GetValue(TabStripHeaderTemplateProperty);
        set => SetValue(TabStripHeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the content that is shown to the right of the tab strip
    /// </summary>
    public object TabStripFooter
    {
        get => GetValue(TabStripFooterProperty);
        set => SetValue(TabStripFooterProperty, value);
    }

    /// <summary>
    /// Gets or sets the IDataTemplate used to display the content of the TabStripFooter
    /// </summary>
    public IDataTemplate TabStripFooterTemplate
    {
        get => GetValue(TabStripFooterTemplateProperty);
        set => SetValue(TabStripFooterTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the add (+) tab button is visible
    /// </summary>
    public bool IsAddTabButtonVisible
    {
        get => GetValue(IsAddTabButtonVisibleProperty);
        set => SetValue(IsAddTabButtonVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to onvoke when the add (+) tab button is tapped
    /// </summary>
    public ICommand AddTabButtonCommand
    {
        get => GetValue(AddTabButtonCommandProperty);
        set => SetValue(AddTabButtonCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the parameter to pass to the <see cref="AddTabButtonCommand"/> property
    /// </summary>
    public object AddTabButtonCommandParameter
    {
        get => GetValue(AddTabButtonCommandParameterProperty);
        set => SetValue(AddTabButtonCommandParameterProperty, value);
    }

    /// <summary>
    /// Gets or sets the TabItems this TabView displays
    /// </summary>
    [Content]
    public IList TabItems
    {
        get => _tabItems;
        private set => SetAndRaise(TabItemsProperty, ref _tabItems, value);
    }

    /// <summary>
    /// Gets or sets the TabItems source for this TabView
    /// </summary>
    public IEnumerable? TabItemsSource
    {
        get => GetValue(TabItemsSourceProperty);
        set => SetValue(TabItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the IDataTemplate used to display each item
    /// </summary>
    public IDataTemplate TabItemTemplate
    {
        get => GetValue(TabItemTemplateProperty);
        set => SetValue(TabItemTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the tabs can be dragged and reordered
    /// through user interaction. This is the single switch for tab dragging: when true, a tab
    /// can be reordered within the strip (and also dragged out as a data payload); when false,
    /// dragging is disabled.
    /// </summary>
    public bool CanReorderTabs
    {
        get => GetValue(CanReorderTabsProperty);
        set => SetValue(CanReorderTabsProperty, value);
    }

    /// <summary>
    /// Gets or sets the index of the selected tab
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetAndRaise(SelectedIndexProperty, ref _selectedIndex, value);
    }

    /// <summary>
    /// Gets or sets the selected tab item
    /// </summary>
    public object? SelectedItem
    {
        get => _selectedItem;
        set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
    }

    /// <summary>
    /// Gets or sets the location of the tab strip for this TabView
    /// </summary>
    public TabViewTabStripLocation TabStripLocation
    {
        get => GetValue(TabStripLocationProperty);
        set => SetValue(TabStripLocationProperty, value);
    }

    /// <summary>
    /// When the tab strip is on the left or the right, returns whether the pane is open.
    /// If the tab strip is on the top or bottom, this has no effect
    /// </summary>
    public bool IsVerticalPaneOpen
    {
        get => GetValue(IsVerticalPaneOpenProperty);
        set => SetValue(IsVerticalPaneOpenProperty, value);
    }

    /// <summary>
    /// When the tab strip is on the left or the right, returns pane's open length
    /// If the tab strip is on the top or bottom, this has no effect
    /// </summary>
    public double VerticalOpenPaneLength
    {
        get => GetValue(VerticalOpenPaneLengthProperty);
        set => SetValue(VerticalOpenPaneLengthProperty, value);
    }

    /// <summary>
    /// When the tab strip is on the left or the right, returns the minimum width
    /// the pane can be opened. If the tab strip is on the top or bottom, this has no effect
    /// </summary>
    public double MinimumVerticalOpenPaneLength
    {
        get => GetValue(MinimumVerticalOpenPaneLengthProperty);
        set => SetValue(MinimumVerticalOpenPaneLengthProperty, value);
    }

    /// <summary>
    /// When the tab strip is on the left or the right, returns the maximum width
    /// the pane can be opened. If the tab strip is on the top or bottom, this has no effect
    /// </summary>
    public double MaximumVerticalOpenPaneLength
    {
        get => GetValue(MaximumVerticalOpenPaneLengthProperty);
        set => SetValue(MaximumVerticalOpenPaneLengthProperty, value);
    }

    /// <summary>
    /// When the tab strip is on the left or the right, returns the display mode of the pane.
    /// If the tab strip is on the top or bottom, this has no effect.
    /// </summary>
    public SplitViewDisplayMode VerticalPaneDisplayMode
    {
        get => GetValue(VerticalPaneDisplayModeProperty);
        set => SetValue(VerticalPaneDisplayModeProperty, value);
    }

    // Internal for Unit Tests Only
    internal TabViewListView? ListView => _listView;

    /// <summary>
    /// Raised when the user attempts to close a Tab via clicking the x-to-close button
    /// </summary>
    public event TypedEventHandler<TabView, TabViewTabCloseRequestedEventArgs>? TabCloseRequested;

    /// <summary>
    /// Occurs when the user completes a drag and drop operation by dropping a tab outside 
    /// of the tab strip area
    /// </summary>
    public event TypedEventHandler<TabView, TabViewTabDroppedOutsideEventArgs>? TabDroppedOutside;

    /// <summary>
    /// Occurs when the add (+) tab button has been clicked
    /// </summary>
    public event TypedEventHandler<TabView, EventArgs>? AddTabButtonClick;

    /// <summary>
    /// Raised when the items collection has changed
    /// </summary>
    public event TypedEventHandler<TabView, NotifyCollectionChangedEventArgs>? TabItemsChanged;

    /// <summary>
    /// Occurs when the currently selected tab changes
    /// </summary>
    public event SelectionChangedEventHandler? SelectionChanged;

    /// <summary>
    /// Occurs when a drag operation is initiated
    /// </summary>
    public event TypedEventHandler<TabView, TabViewTabDragStartingEventArgs>? TabDragStarting;

    /// <summary>
    /// Raised when the user completes the drag action
    /// </summary>
    public event TypedEventHandler<TabView, TabViewTabDragCompletedEventArgs>? TabDragCompleted;

    /// <summary>
    /// Occurs when the input system reports an underlying drag event with the TabStrip as 
    /// the potential drop target
    /// </summary>
    public event EventHandler<DragEventArgs>? TabStripDragOver;

    /// <summary>
    /// Occurs when the input system reports an underlying drop event with the TabStrip as
    /// the drop target
    /// </summary>
    public event EventHandler<DragEventArgs>? TabStripDrop;
    
    private IList _tabItems;
    private int _selectedIndex;
    private object? _selectedItem;
    
    private TabViewCommand _keyboardAcceleratorHandler;

    private bool _updateTabWidthOnPointerLeave;
    private bool _pointerInTabstrip;

    private ColumnDefinition? _leftContentColumn;
    private ColumnDefinition? _tabColumn;
    private ColumnDefinition? _addButtonColumn;
    private ColumnDefinition? _rightContentColumn;

    private TabViewListView? _listView;
    private ContentPresenter? _tabContentPresenter;
    private ContentPresenter? _rightContentPresenter;
    private Grid? _tabContainerGrid;
    private SingleDirectionScrollViewer? _scrollViewer;
    private RepeatButton? _scrollDecreaseButton;
    private RepeatButton? _scrollIncreaseButton;
    private Button? _addButton;
    private ItemsPresenter? _itemsPresenter;
    private Border? _verticalPaneResizeHandle;

    private bool _isDraggingPane;
    private Point? _initDragPanePoint;
    private double _startingPaneSize;

    private bool _isSwitchingTabLocation;

    // A bunch of event revokers
    private IDisposable? _listViewCanReorderItemsPropertyChangedRevoker;
    private IDisposable? _listViewAllowDropPropertyChangedRevoker;
    private Size _previousAvailableSize;

    /// <summary>
    /// The tab width most recently computed by <see cref="UpdateTabWidths"/> (NaN = auto).
    /// Containers prepared later by virtualization pick this up, since the per-item width
    /// loop may have run before those containers existed.
    /// </summary>
    internal double CurrentTabWidth { get; private set; } = double.NaN;

    private bool _isDragging = false;
    private bool _isItemDraggedOver;
    private double? _expandedWidthForDragOver;

    private static double _tabMinimumWidth = 48d;
    private static double _tabMaximumWidth = 200d;

    // (WinUI) TODO: what is the right number and should this be customizable?
    private static double _scrollAmount = 50d;

    // Internal for unit test access
    internal const string TAA_CONTENT_PRESENTER = "TabContentPresenter";
    private const string RIGHT_CONTENT_PRESENTER = "RightContentPresenter";
    private const string TAB_CONTAINER_GRID = "TabContainerGrid";
    private const string TAB_LIST_VIEW = "TabListView";
    internal const string ADD_BUTTON = "AddButton";

    // Technically these are template parts on the ScrollViewer, but we ref them here
    private const string SCROLL_DECREASE_BUTTON = "ScrollDecreaseButton";
    private const string SCROLL_INCREASE_BUTTON = "ScrollIncreaseButton";

    private const string BORDER_RESIZE_HANDLE_HOST = "BorderResizeHandleHost";

    // These two come from the WinUI port, so they don't follow the normal naming convention for parity upstream
    private static string RES_TAB_VIEW_ITEM_MIN_WIDTH = "TabViewItemMinWidth";
    private static string RES_TAB_VIEW_ITEM_MAX_WIDTH = "TabViewItemMaxWidth";

    private const string PC_SINGLE_BORDER = ":singleBorder";

    internal const string PC_TOP = ":top";
    internal const string PC_LEFT = ":left";
    internal const string PC_RIGHT = ":right";
    internal const string PC_BOTTOM = ":bottom";

    // TabViewItem subs to these in OnApplyTemplate, but we need to make sure the strong ref to TabView isn't
    // held if the TabViewItem is removed
    internal static readonly WeakEvent<TabView, TabViewTabDragStartingEventArgs> TabDragStartingWeakEvent = 
        WeakEvent.Register<TabView, TabViewTabDragStartingEventArgs>(
                (c, s) =>
                {
                    TypedEventHandler<TabView, TabViewTabDragStartingEventArgs> handler = (_, e) => s(c, e);
                    c.TabDragStarting += handler;
                    return () => c.TabDragStarting -= handler;
                });

    internal static readonly WeakEvent<TabView, TabViewTabDragCompletedEventArgs> TabDragCompletedWeakEvent =
        WeakEvent.Register<TabView, TabViewTabDragCompletedEventArgs>(
        (c, s) =>
        {
            TypedEventHandler<TabView, TabViewTabDragCompletedEventArgs> handler = (_, e) => s(c, e);
            c.TabDragCompleted += handler;
            return () => c.TabDragCompleted -= handler;
        });
    
    public TabView()
    {
        TabItems = new AvaloniaList<object>();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        // Get the current platform's Command Modifier instead of just assuming Control
        var ctrl = Application.Current.PlatformSettings.HotkeyConfiguration.CommandModifiers;

        // Keyboard Accelerators (KeyBindings in Avalonia)
        // Require a Command, so we wire this up *slightly* differently
        // compared to WinUI

        _keyboardAcceleratorHandler = new TabViewCommand(OnKeyboardAcceleratorInvoked);

        var closeTabGesture = new KeyGesture(Key.F4, ctrl);
        KeyBindings.Add(new KeyBinding
        {
            Gesture = closeTabGesture,
            Command = _keyboardAcceleratorHandler,
            CommandParameter = TabViewCommandType.CtrlF4
        });
        KeyBindings.Add(new KeyBinding
        {
            Gesture = new KeyGesture(Key.Tab, ctrl),
            Command = _keyboardAcceleratorHandler,
            CommandParameter = TabViewCommandType.CtrlTab
        });
        KeyBindings.Add(new KeyBinding
        {
            Gesture = new KeyGesture(Key.Tab, ctrl | KeyModifiers.Shift),
            Command = _keyboardAcceleratorHandler,
            CommandParameter = TabViewCommandType.CtrlShiftTab
        });

        PseudoClasses.Set(PC_TOP, true);
        DragDrop.SetAllowDrop(this, true);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        UnhookEventsAndClearFields();
        //_isItemBeingDragged = false;
        _isItemDraggedOver = false;
        _expandedWidthForDragOver = null;

        base.OnApplyTemplate(e);

        _tabContentPresenter = e.NameScope.Find<ContentPresenter>(TAA_CONTENT_PRESENTER);
        _rightContentPresenter = e.NameScope.Find<ContentPresenter>(RIGHT_CONTENT_PRESENTER);

        _tabContainerGrid = e.NameScope.Get<Grid>(TAB_CONTAINER_GRID);
        if (_tabContainerGrid.ColumnDefinitions.Count > 0)
        {
            _leftContentColumn = _tabContainerGrid.ColumnDefinitions[0];
            _tabColumn = _tabContainerGrid.ColumnDefinitions[1];
            _addButtonColumn = _tabContainerGrid.ColumnDefinitions[2];
            _rightContentColumn = _tabContainerGrid.ColumnDefinitions[3];
        }
        else
        {
            _tabContainerGrid.SizeChanged += HandleTabContainerGridSizeChangedForVerticalTabView;
        }
        
        _tabContainerGrid.PointerEntered += OnTabStripPointerEnter;
        _tabContainerGrid.PointerExited += OnTabStripPointerLeave;

        _listView = e.NameScope.Get<TabViewListView>(TAB_LIST_VIEW);
        if (_listView != null)
        {
            LogicalChildren.Add(_listView);
            _listView.Loaded += OnListViewLoaded;
            _listView.SelectionChanged += OnListViewSelectionChanged;
            _listView.SizeChanged += OnListViewSizeChanged;

            _listView.DragItemsStarting += OnListViewDragItemsStarting;
            _listView.DragItemsCompleted += OnListViewDragItemsCompleted;

            _listView.DragOver += OnListViewDragOver;
            _listView.Drop += OnListViewDrop;
            _listView.DragEnter += OnListViewDragEnter;
            _listView.DragLeave += OnListViewDragLeave;

            _listView.GettingFocus += OnListViewGettingFocus;

            _listViewCanReorderItemsPropertyChangedRevoker =
                _listView.GetPropertyChangedObservable(TabViewListView.CanReorderItemsProperty)
                .Subscribe(_ => OnListViewDraggingPropertyChanged());
            _listViewAllowDropPropertyChangedRevoker =
                _listView.GetPropertyChangedObservable(DragDrop.AllowDropProperty)
                .Subscribe(_ => OnListViewDraggingPropertyChanged());
        }

        _addButton = e.NameScope.Find<Button>(ADD_BUTTON);
        if (_addButton != null)
        {
            _addButton.Click += OnAddButtonClick;
            _addButton.KeyDown += OnAddButtonKeyDown;
        }

        var handle = e.NameScope.Find<Border>(BORDER_RESIZE_HANDLE_HOST);
        if (handle != null) // Null in Top/Bottom modes
        {
            handle.PointerPressed += OnPaneResizeHandlePointerPressed;
            handle.PointerMoved += OnPaneResizeHandlePointerMoved;
            handle.PointerReleased += OnPaneResizeHandlePointerReleased;
            handle.PointerCaptureLost += OnPaneResizeHandlePointerCaptureLost;
            _verticalPaneResizeHandle = handle;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_previousAvailableSize.Width != availableSize.Width)
        {
            _previousAvailableSize = availableSize;
            UpdateTabWidths();
        }

        return base.MeasureOverride(availableSize);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new TabViewAutomationPeer(this);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CloseButtonOverlayModeProperty)
        {
            OnCloseButtonOverlayModePropertyChanged(change);
        }
        else if (change.Property == SelectedIndexProperty)
        {
            OnSelectedIndexPropertyChanged(change);
        }
        else if (change.Property == SelectedItemProperty)
        {
            OnSelectedItemPropertyChanged(change);
        }
        else if (change.Property == TabItemsSourceProperty)
        {
            OnTabItemsSourcePropertyChanged(change);
        }
        else if (change.Property == TabWidthModeProperty)
        {
            OnTabWidthModePropertyChanged(change);
        }
        else if (change.Property == TabStripLocationProperty)
        {
            OnTabStripLocationPropertyChanged(change);
        }
        else if (change.Property == IsAddTabButtonVisibleProperty)
        {
            // 加号按钮可见性变化会改变它占用的列宽，需等下一次布局刷新后再重算，
            // 否则 _addButtonColumn.ActualWidth 还是旧值，按钮会被挤出可视区。
            Dispatcher.UIThread.Post(() => UpdateTabWidths());
        }
    }

    internal void SetTabSeparatorOpacity(int index, int opacityValue)
    {
        // A negative index happens when the tab strip is empty (e.g. after clearing all
        // tabs) - there's nothing to do in that case.
        if (index < 0)
        {
            return;
        }

        if (ContainerFromIndex(index) is TabViewItem tvi)
        {
            // The reason we set the opacity directly instead of using VisualState
            // is because we want to hide the separator on hover/pressed
            // but the tab adjacent on the left to the selected tab
            // must hide the tab separator at all times.
            // It causes two visual states to modify the same property
            // what leads to undesired behaviour.
            tvi.TabSeparator?.Opacity = opacityValue;
        }
    }

    internal void SetTabSeparatorOpacity(int index)
    {
        var selIndex = SelectedIndex;

        // If Tab is adjacent on the left to selected one or
        // it is selected tab - we hide the tabSeparator.
        if (index == selIndex || index + 1 == selIndex)
        {
            SetTabSeparatorOpacity(index, 0);
        }
        else
        {
            SetTabSeparatorOpacity(index, 1);
        }
    }

    protected virtual void OnTabStripLocationPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var (oldValue, newValue) = args.GetOldAndNewValue<TabViewTabStripLocation>();

        // TODO v3: Is this needed or left over from my testing?
        // Avalonia needs to fix https://github.com/AvaloniaUI/Avalonia/issues/21055 first
        // then this will probably need to be turned back on

        //if ((IsHorizontal(oldValue) && !IsHorizontal(newValue)) ||
        //    (!IsHorizontal(oldValue) && IsHorizontal(newValue)) &&
        //    _listView != null && _listView.ItemsSource == null)
        //{
        //    // We're switching from vertical to horizontal or horizontal to vertical
        //    // If we're not using the TabItemsSource, we need to make a copy of the
        //    // TabItems to unhook them from the ItemsControl
        //    var l = new List<object>();
        //    foreach (var item in TabItems)
        //        l.Add(item);

        //    _listView.Items.Clear();
        //    TabItems = l;
        //}


        _isSwitchingTabLocation = true;

        if ((IsHorizontal(oldValue) && !IsHorizontal(newValue)) ||
            (!IsHorizontal(oldValue) && IsHorizontal(newValue)))
        {
            // Only set TabContent to null if we're truly switching orientations
            UpdateTabContent();
        }
        

        var oldClass = GetClassForStripLocation(args.GetOldValue<TabViewTabStripLocation>());
        var newClass = GetClassForStripLocation(args.GetNewValue<TabViewTabStripLocation>());
        PseudoClasses.Remove(oldClass);
        PseudoClasses.Add(newClass);

        _listView?.HandleTabStripLocationChanged(args.GetNewValue<TabViewTabStripLocation>(), oldClass, newClass);
        
        UpdateTabWidths();

        static bool IsHorizontal(TabViewTabStripLocation loc) =>
            loc == TabViewTabStripLocation.Top || loc == TabViewTabStripLocation.Bottom;
    }

    private void OnListViewDraggingPropertyChanged()
    {
        //UpdateListViewItemContainerTransitions();
    }

    private void OnListViewGettingFocus(object? sender, FocusChangingEventArgs args)
    {
        // TabViewItems overlap each other by one pixel in order to get the desired visuals for the separator.
        // This causes problems with 2d focus navigation. Because the items overlap, pressing Down or Up from a
        // TabViewItem navigates to the overlapping item which is not desired.
        //
        // To resolve this issue, we detect the case where Up or Down focus navigation moves from one TabViewItem
        // to another.
        // How we handle it, depends on the input device.
        // For GamePad, we want to move focus to something in the direction of movement (other than the overlapping item)
        // For Keyboard, we cancel the focus movement.

        // TODO: v3
    }

    private void OnSelectedIndexPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        // We update previous selected and adjacent on the left tab
        // as well as current selected and adjacent on the left tab
        // to show/hide tabSeparator accordingly.
        UpdateSelectedIndex();
        SetTabSeparatorOpacity(args.GetOldValue<int>());
        SetTabSeparatorOpacity(args.GetOldValue<int>() - 1);
        SetTabSeparatorOpacity(args.GetNewValue<int>() - 1);
        SetTabSeparatorOpacity(args.GetNewValue<int>());

        UpdateBottomBorderLineVisualStates();
    }

    private void UpdateTabBottomBorderLineVisualStates()
    {
        int numItems = GetItemCount();
        int selIndex = SelectedIndex;

        for (int i = 0; i < numItems; i++)
        {
            // -1 = normal, 0 = no bottom border, 1 = leftofselectedtab, 2 = rightofselectedtab
            int state = -1;
            if (_isDragging)
            {
                state = 0;
            }
            else if (selIndex != -1)
            {
                if (i == selIndex)
                {
                    state = 0;
                }
                else if (i == selIndex - 1)
                {
                    state = 1;
                }
                else if (i == selIndex + 1)
                {
                    state = 2;
                }
            }

            if (ContainerFromIndex(i) is TabViewItem tvi)
            {
                ((IPseudoClasses)tvi.Classes).Set(SharedPseudoclasses.s_pcNoBorder, state == 0);
                ((IPseudoClasses)tvi.Classes).Set(SharedPseudoclasses.s_pcBorderLeft, state == 1);
                ((IPseudoClasses)tvi.Classes).Set(SharedPseudoclasses.s_pcBorderRight, state == 2);
            }
        }
    }

    private void UpdateBottomBorderLineVisualStates()
    {
        // Update border line on all tabs
        UpdateTabBottomBorderLineVisualStates();

        PseudoClasses.Set(PC_SINGLE_BORDER, _isDragging);

        // Update border lines in the inner TabViewListView
        if (_listView != null)
        {
            (_listView.Classes as IPseudoClasses).Set(SharedPseudoclasses.s_pcNoBorder, _isDragging);
        }

        // Update border lines in the ScrollViewer
        if (_scrollViewer != null)
        {
            (_scrollViewer.Classes as IPseudoClasses).Set(SharedPseudoclasses.s_pcNoBorder, _isDragging);
        }
    }

    private void OnSelectedItemPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        UpdateSelectedItem();
    }

    private void OnTabItemsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        UpdateListViewItemContainerTransitions();
    }

    private void UpdateListViewItemContainerTransitions() { }

    private void OnTabWidthModePropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        UpdateTabWidths();

        var newValue = change.GetNewValue<TabViewWidthMode>();
        // Switch the visual states of all tab items to the correct TabViewWidthMode
        int itemCount = GetItemCount();
        for (int i = 0; i < itemCount; i++)
        {
            if (ContainerFromIndex(i) is TabViewItem tvi)
            {
                tvi.OnTabViewWidthModeChanged(newValue);
            }
        }
    }

    private void OnCloseButtonOverlayModePropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var newValue = change.GetNewValue<TabViewCloseButtonOverlayMode>();
        // Switch the visual states of all tab items to the correct TabViewWidthMode
        int itemCount = GetItemCount();
        for (int i = 0; i < itemCount; i++)
        {
            if (ContainerFromIndex(i) is TabViewItem tvi)
            {
                tvi.OnCloseButtonOverlayModeChanged(newValue);
            }
        }
    }

    private void OnAddButtonClick(object? sender, RoutedEventArgs args)
    {
        AddTabButtonClick?.Invoke(this, args);
    }

    private void OnLoaded(object? sender, RoutedEventArgs args)
    {
        UpdateTabContent();
        // UpdateTabViewWithTearOutList();
        // AttachMoveSizeLoopEvents();
        // UpdateNonClientRegion();
    }

    private void OnUnloaded(object? sender, RoutedEventArgs args)
    {
        // UpdateTabViewWithTearOutList();
    }

    private void OnListViewLoaded(object? sender, RoutedEventArgs args)
    {
        var listView = _listView;
        if (listView == null)
        {
            return;
        }

        var lvItems = listView.Items;
        // 2nd condition added, if TabItems is already the ListView's ItemCollection, we just swapped in the same
        // orientation (top - bottom / left - right), so the ListView was reloaded, but its still the same one
        if (lvItems != TabItems)
        {
            if (listView.ItemsSource == null)
            {
                if (_isSwitchingTabLocation)
                {
                    // Unhook the TabItems from the old ItemCollection
                    var tabItems = TabItems;

                    foreach (var item in tabItems)
                    {
                        if (item is TabViewItem tvi && tvi.GetVisualParent() is Panel p)
                        {
                            p.Children.Remove(tvi);
                        }

                        lvItems.Add(item);
                    }

                    TabItems.Clear();
                }
                else
                {
                    // copy the list, because clearing lvItems may also clear TabItems
                    using var l = new PooledList<object>(lvItems.Count);

                    foreach (var item in TabItems)
                        l.Add(item);

                    lvItems.Clear();

                    foreach (var item in l.AsSpan())
                        lvItems.Add(item);
                }                
            }

            TabItems = lvItems;
        }


        // Ensure the ListView is configured correctly when it loads
        var stripLocation = TabStripLocation;
        listView.HandleTabStripLocationChanged(stripLocation, null, GetClassForStripLocation(stripLocation));

        if (SelectedItem != null)
        {
            UpdateSelectedItem();
        }
        else
        {
            // If SelectedItem wasn't set, default to selecting the first tab
            UpdateSelectedIndex();
        }

        SelectedIndex = listView.SelectedIndex;
        SelectedItem = listView.SelectedItem;

        if (_isSwitchingTabLocation)
        {
            _isSwitchingTabLocation = false;
            UpdateTabContent();
        }

        _itemsPresenter = listView.Presenter;
        if (_itemsPresenter != null)
        {
            _itemsPresenter.SizeChanged += OnItemsPresenterSizeChanged;
        }
        
        var scrollViewer = listView.Scroller;
        _scrollViewer = scrollViewer;
        if (scrollViewer != null)
        {
            if (scrollViewer.IsLoaded)
            {
                OnScrollViewerLoaded(null, null);
            }
            else
            {
                scrollViewer.Loaded += OnScrollViewerLoaded;
            }
        }        

        UpdateBottomBorderLineVisualStates();
        // UpdateNonClientRegion();
    }

    private void OnTabStripPointerLeave(object? sender, PointerEventArgs e)
    {
        _pointerInTabstrip = false;
        if (_updateTabWidthOnPointerLeave)
        {
            try
            {
                UpdateTabWidths();
            }
            finally
            {
                _updateTabWidthOnPointerLeave = false;
            }
        }
    }

    private void OnTabStripPointerEnter(object? sender, PointerEventArgs e)
    {
        _pointerInTabstrip = true;
    }

    private void OnScrollViewerLoaded(object? sender, RoutedEventArgs? args)
    {
        if (_scrollViewer != null)
        {
            _scrollViewer.Loaded -= OnScrollViewerLoaded;
        }

        var buttons = _scrollViewer?.GetTemplateDescendants().Where(x => x is RepeatButton);

        if (buttons != null)
        {
            foreach (RepeatButton? button in buttons)
            {
                if (button?.Name == SCROLL_DECREASE_BUTTON)
                {
                    _scrollDecreaseButton = button;
                    _scrollDecreaseButton.Click += OnScrollDecreaseClick;
                }
                else if (button?.Name == SCROLL_INCREASE_BUTTON)
                {
                    _scrollIncreaseButton = button;
                    _scrollIncreaseButton.Click += OnScrollIncreaseClick;
                }
            }
        }

        _scrollViewer?.ScrollChanged += OnScrollViewerViewChanged;
        
        UpdateTabWidths();
    }

    private void OnScrollViewerViewChanged(object? sender, ScrollChangedEventArgs args)
    {
        UpdateScrollViewerDecreaseAndIncreaseButtonsViewState();

        // Another case where we have to do something WinUI doesn't. Scrolling (recycling) tabs
        // doesn't ensure their widths are set correctly and so some will autosize to their
        // content and be just slightly bigger. Ensure that doesn't happen
        UpdateTabWidths();
    }

    private void UpdateScrollViewerDecreaseAndIncreaseButtonsViewState()
    {
        if (_scrollViewer != null && _scrollDecreaseButton != null && _scrollIncreaseButton != null)
        {
            const double minThreshold = 0.1d;
            var hOffset = _scrollViewer.Offset.X;
            var scrollableWidth = (_scrollViewer.Extent.Width - _scrollViewer.Viewport.Width);

            if (double.Abs(hOffset - scrollableWidth) < minThreshold)
            {
                _scrollDecreaseButton?.IsEnabled = true;
                _scrollIncreaseButton?.IsEnabled = false;
            }
            else if (double.Abs(hOffset) < minThreshold)
            {
                _scrollDecreaseButton.IsEnabled = false;
                _scrollIncreaseButton.IsEnabled = true;
            }
            else
            {
                _scrollDecreaseButton.IsEnabled = true;
                _scrollIncreaseButton.IsEnabled = true;
            }
        }
    }

    private void OnItemsPresenterSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        if (!_updateTabWidthOnPointerLeave)
        {
            // Presenter size didn't change because of item being removed, so update manually
            UpdateScrollViewerDecreaseAndIncreaseButtonsViewState();
            UpdateTabWidths();
            // Make sure that the selected tab is fully in view and not cut off
            BringSelectedTabIntoView();
        }
    }

    private void HandleTabContainerGridSizeChangedForVerticalTabView(object? sender, SizeChangedEventArgs e)
    {
        UpdateTabWidths();
    }

    private void BringSelectedTabIntoView()
    {
        if (SelectedItem != null)
        {
            var tvi = SelectedItem as TabViewItem ?? ContainerFromItem(SelectedItem) as TabViewItem;            
            tvi?.StartBringTabIntoView();
        }
    }

    internal void OnItemsChanged(object item)
    {
        if (item is NotifyCollectionChangedEventArgs args && _listView != null)
        {
            TabItemsChanged?.Invoke(this, args);

            int numItems = GetItemCount();
            var listViewInnerSelectedIndex = _listView.SelectedIndex;
            var selectedIndex = SelectedIndex;

            if (selectedIndex != listViewInnerSelectedIndex && listViewInnerSelectedIndex != -1)
            {
                SelectedIndex = listViewInnerSelectedIndex;
                selectedIndex = listViewInnerSelectedIndex;
            }

            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                _updateTabWidthOnPointerLeave = true;
                if (numItems > 0)
                {
                    // SelectedIndex might also already be -1
                    if (selectedIndex == -1 || selectedIndex == args.OldStartingIndex)
                    {
                        // Find the closest tab to select instead
                        int startIndex = args.OldStartingIndex;
                        if (startIndex >= numItems)
                        {
                            startIndex = numItems - 1;
                        }
                        int index = startIndex;

                        do
                        {
                            var nextitem = ContainerFromIndex(index) as TabViewItem;

                            if (nextitem != null && nextitem.IsEffectivelyEnabled
                                && nextitem.IsEffectivelyVisible)
                            {
                                SelectedItem = ItemFromContainer(nextitem);
                                break;
                            }

                            // try the next item
                            index++;
                            if (index >= numItems)
                            {
                                index = 0;
                            }
                        }
                        while (index != startIndex);
                    }
                }

                if (TabWidthMode == TabViewWidthMode.Equal)
                {
                    if (!_pointerInTabstrip || args.OldStartingIndex == numItems)
                    {
                        UpdateTabWidths(true, false);
                    }
                }
            }
            else
            {
                // GH#424, Adding a tab item wouldn't set the size correctly until pointer exit,
                // as when this is called following a collection change, the items haven't been
                // materialized yet in the panel so UpdateTabWidths using the old previous item
                // Posting to Dispatcher so delay calling this until after next layout pass
                // when items are all realized and ContainerFromIndex works
                // TODO: Do we still need to post to dispatcher
                
                Dispatcher.UIThread.Post(() =>
                {
                    UpdateTabWidths();
                    SetTabSeparatorOpacity(numItems - 1);
                });
            }
        }

        UpdateBottomBorderLineVisualStates();
    }

    private void OnListViewSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        // If we're currently switching TabLocation, ignore this selected item change
        // because it just got set to -1. We'll set it back to the correct index
        // when the ListView loaded handler is called
        if (_isSwitchingTabLocation || _listView == null)
            return;

        SelectedIndex = _listView.SelectedIndex;
        SelectedItem = _listView.SelectedItem;

        // Fix for GH714. Closing the active tab would not update the current tab content. Seems to be caused by
        // a delay in the VirtualizingStackPanel where ItemFromContainer would return the old container (we closed)
        // on the new SelectedIndex. So ensure the VSP is up to date before we switch tab content.
        Dispatcher.UIThread.Post(UpdateTabContent);

        SelectionChanged?.Invoke(this, args);
    }

    private void OnListViewSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        // UpdateNonClientRegion();
    }

    private TabViewItem? FindTabViewItemFromDragItem(object? item)
    {
        if (item == null)
        {
            return null;
        }

        // Fast path: the item is the data item of a container, so we can resolve
        // the container directly. If the item is already (part of) a container,
        // walk up the visual tree to find the TabViewItem.
        var tab = ContainerFromItem(item) as TabViewItem;

        if (tab == null && item is Visual visual)
        {
            tab = visual.FindAncestorOfType<TabViewItem>(true);
        }

        if (tab == null)
        {
            // This is a fallback scenario for tabs without a data context
            int numItems = GetItemCount();
            for (int i = 0; i < numItems; i++)
            {
                if (ContainerFromIndex(i) is TabViewItem tabItem &&
                    (ReferenceEquals(tabItem.Content, item) ||
                     ReferenceEquals(tabItem.Header, item) ||
                     ReferenceEquals(tabItem.DataContext, item)))
                {
                    tab = tabItem;
                    break;
                }
            }
        }

        return tab;
    }

    private void OnListViewDragItemsStarting(object? sender, DragItemsStartingEventArgs args)
    {
        // _isItemBeingDragged = true;

        if (args.Items == null || args.Items.Count == 0 || args.Items[0] is not { } item)
        {
            return;
        }

        var tab = FindTabViewItemFromDragItem(item);
        var myArgs = new TabViewTabDragStartingEventArgs(args, item, tab);
        TabDragStarting?.Invoke(this, myArgs);
        UpdateBottomBorderLineVisualStates();
    }

    private void OnListViewDragOver(object? sender, DragEventArgs args)
    {
        TabStripDragOver?.Invoke(this, args);
    }

    private void OnListViewDrop(object? sender, DragEventArgs args)
    {
        if (!args.Handled)
        {
            TabStripDrop?.Invoke(this, args);
        }

        UpdateIsItemDraggedOver(false);
    }

    private void OnListViewDragEnter(object? sender, DragEventArgs args)
    {
        // Don't show the "drop here" visual state if the item being dragged belongs
        // to this TabView (that's a reorder, not a new tab coming in).
        // NOTE: iterate the realized containers rather than TabItems as TabItems may not
        // be populated when a TabItemsSource is used.
        if (_listView?.IsInReorder == true)
        {
            return;
        }

        int itemCount = GetItemCount();
        for (int i = 0; i < itemCount; i++)
        {
            if (ContainerFromIndex(i) is TabViewItem tvi && tvi.IsBeingDragged)
            {
                return;
            }
        }

        UpdateIsItemDraggedOver(true);
    }

    private void OnListViewDragLeave(object? sender, DragEventArgs args)
    {
        UpdateIsItemDraggedOver(false);
    }

    private void OnListViewDragItemsCompleted(object? sender, DragItemsCompletedEventArgs args)
    {
        // _isItemBeingDragged = false;

        // Selection may have changed during drag if dragged outside, so we update SelectedIndex again.
        if (_listView != null)
        {
            SelectedIndex = _listView.SelectedIndex;
            SelectedItem = _listView.SelectedItem;

            BringSelectedTabIntoView();
        }

        if (args.Items == null || args.Items.Count == 0 || args.Items[0] is not { } item)
        {
            UpdateBottomBorderLineVisualStates();
            return;
        }

        var tab = FindTabViewItemFromDragItem(item);
        var myArgs = new TabViewTabDragCompletedEventArgs(args, item, tab);
        TabDragCompleted?.Invoke(this, myArgs);

        // None means it's outside of the tab strip area
        if (args.DropResult == DragDropEffects.None)
        {
            var tabDroppedArgs = new TabViewTabDroppedOutsideEventArgs(item, tab);
            TabDroppedOutside?.Invoke(this, tabDroppedArgs);
        }

        UpdateBottomBorderLineVisualStates();
    }

    private void UpdateTabContent()
    {
        if (_tabContentPresenter == null)
            return;

        if (SelectedItem == null || _isSwitchingTabLocation)
        {
            _tabContentPresenter.Content = null;
            _tabContentPresenter.ContentTemplate = null;
        }
        else
        {
            var tvi = (SelectedItem as TabViewItem) ?? ContainerFromItem(SelectedItem) as TabViewItem;

            if (tvi != null)
            {
                // If the focus was in the old tab content, we will lose focus when it is removed from the visual tree.
                // We should move the focus to the new tab content.
                // The new tab content is not available at the time of the LosingFocus event, so we need to
                // move focus later.
                bool shouldMoveFocusToNewTab = false;
                _tabContentPresenter.LosingFocus += TabContentPresenterLostFocus;

                void TabContentPresenterLostFocus(object? sender, FocusChangingEventArgs args)
                {
                    _tabContentPresenter.LosingFocus -= TabContentPresenterLostFocus;
                    shouldMoveFocusToNewTab = true;
                }

                _tabContentPresenter.Content = tvi.Content;
                _tabContentPresenter.ContentTemplate = tvi.ContentTemplate;

                // It is not ideal to call UpdateLayout here, but it is necessary to ensure that the ContentPresenter has expanded its content
                // into the live visual tree.
                
                if (shouldMoveFocusToNewTab)
                {
                    var focusable = TopLevel.GetTopLevel(this)?.FocusManager?.FindNextElement(NavigationDirection.Next,
                        new FindNextElementOptions { SearchRoot = _tabContentPresenter });
                    
                    // If there is nothing focusable in the new tab, just move focus to the TabViewItem itself.
                    focusable ??= tvi;

                    focusable.Focus();
                }
                else
                {
                    // Ensure this is disconnected
                    _tabContentPresenter.LosingFocus -= TabContentPresenterLostFocus;
                }
            }
        }       
    }

    internal void RequestCloseTab(TabViewItem container, bool updateTabWidths)
    {
        // If the tab being closed is the currently focused tab, we'll move focus to the next tab
        // when the tab closes.
        bool tabIsFocused = false;
        var focusedObject = TopLevel.GetTopLevel(this)?.FocusManager.GetFocusedElement();
        var focusedElement = focusedObject as Visual;

        while (focusedElement != null)
        {
            if (focusedElement == container)
            {
                tabIsFocused = true;
                break;
            }

            focusedElement = focusedElement.GetVisualParent();
        }

        if (tabIsFocused)
        {
            container.LosingFocus += ContainerLosingFocus;

            void ContainerLosingFocus(object? sender, FocusChangingEventArgs args)
            {
                container.LosingFocus -= ContainerLosingFocus;

                if (!args.Canceled && !args.Handled)
                {
                    int focusedIndex = IndexFromContainer(container);
                    Control? newFocusedElement = null;

                    for (int i = focusedIndex + 1; i < GetItemCount(); i++)
                    {
                        var control = ContainerFromIndex(i);
                        if (control != null)
                        {
                            if (IsFocusable(control))
                            {
                                newFocusedElement = control;
                                break;
                            }
                        }
                    }

                    if (newFocusedElement == null)
                    {
                        for (int i = focusedIndex - 1; i >= 0; i--)
                        {
                            var control =  ContainerFromIndex(i);
                            if (control != null)
                            {
                                if (IsFocusable(control))
                                {
                                    newFocusedElement = control;
                                    break;
                                }
                            }
                        }
                    }

                    if (newFocusedElement == args.NewFocusedElement)
                        return;

                    if (newFocusedElement == null)
                    {
                        newFocusedElement = _addButton;
                    }

                    args.Handled = args.TrySetNewFocusedElement(newFocusedElement);
                }
            }
        }

        if (_listView != null)
        {
            var args = new TabViewTabCloseRequestedEventArgs(ItemFromContainer(container), container);

            TabCloseRequested?.Invoke(this, args);

            container.RaiseRequestClose(args);
        }

        UpdateTabWidths(updateTabWidths);
    }

    private void OnScrollDecreaseClick(object? sender, RoutedEventArgs args)
    {
        if (_scrollViewer != null)
        {
            var current = _scrollViewer.Offset;
            // _scrollViewer.Offset = current.WithX(current.X - _scrollAmount);

            if (_scrollViewer.GetVisualDescendants().OfType<SingleDirectionScrollContentPresenter>().FirstOrDefault() is SingleDirectionScrollContentPresenter presenter)
            {
                _ = presenter.ScrollToAsync(current.WithX(current.X - _scrollAmount));
            }
        }
    }

    private void OnScrollIncreaseClick(object? sender, RoutedEventArgs args)
    {
        if (_scrollViewer != null)
        {
            var current = _scrollViewer.Offset;
            // _scrollViewer.Offset = current.WithX(current.X + _scrollAmount);
            
            if (_scrollViewer.GetVisualDescendants().OfType<SingleDirectionScrollContentPresenter>().FirstOrDefault() is SingleDirectionScrollContentPresenter presenter)
            {
                _ = presenter.ScrollToAsync(current.WithX(current.X + _scrollAmount));
            }
        }
    }

    private void UpdateTabWidths(bool shouldUpdateWidths = true, bool fillAllAvailableSpace = true)
    {
        var maxTabWidth = this.TryFindResource(RES_TAB_VIEW_ITEM_MAX_WIDTH, out var mtw) ? (double)mtw : _tabMaximumWidth;
        double tabWidth = double.NaN;
        int itemCount = GetItemCount();
        int tabCount = itemCount;

        if (itemCount == 0)
        {
            if (_tabColumn != null)
            {
                _tabColumn.Width = new GridLength(1, GridUnitType.Auto);
            }

            if (_listView != null)
            {
                _listView.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
            }

            return;
        }

        // If an item is being dragged over this TabView, then we'll want to act like there's an extra item
        // when updating tab widths, which will create a hole into which the item can be dragged.
        if (_isItemDraggedOver)
        {
            tabCount++;
        }
        var stripLocation = TabStripLocation;
        var isHorizontal = (stripLocation == TabViewTabStripLocation.Top || stripLocation == TabViewTabStripLocation.Bottom);
        if (_tabContainerGrid != null && isHorizontal)
        {
            // Add up width taken by custom content and + button
            double widthTaken = 0.0;
            if (_leftContentColumn != null)
            {
                widthTaken += _leftContentColumn.ActualWidth;
            }
            if (_addButtonColumn != null)
            {
                widthTaken += _addButtonColumn.ActualWidth;
            }
            if (_rightContentColumn != null)
            {
                if (_rightContentPresenter != null)
                {
                    var size = _rightContentPresenter.DesiredSize;
                    _rightContentColumn.MinWidth = size.Width;
                    widthTaken += size.Width;
                }
            }

            if (_tabColumn != null)
            {
                // Note: can be infinite
                var availableWidth = _previousAvailableSize.Width - widthTaken;

                // Size can be 0 when window is first created; in that case, skip calculations; we'll get a new size soon
                if (availableWidth > 0)
                {
                    if (TabWidthMode == TabViewWidthMode.Equal)
                    {
                        var minTabWidth = this.TryFindResource(RES_TAB_VIEW_ITEM_MIN_WIDTH, out var value) ? (double)value : _tabMinimumWidth;
                        var padding = Padding;

                        // We don't have this, so skip what WinUI does, but to avoid messing up the math
                        // just keep these variables around
                        double headerWidth = 0, footerWidth = 0;

                        // Measure the natural content width of each tab: the widest one is used
                        // for the uniform width when the strip needs to scroll.
                        double widestTabWidth = 0;
                        for (int i = 0; i < itemCount; i++)
                        {
                            if (ContainerFromIndex(i) is TabViewItem tvi)
                            {
                                tvi.Width = double.NaN;

                                // Tabs are measured while unselected, where the close button is
                                // collapsed. Once selected (or hovered) the button appears; if the
                                // natural width didn't account for it, the fixed tab width ends up
                                // too small and part of the tab content gets cut off. Include it.
                                var restoreCollapsed = false;
                                if (tvi.IsClosable && tvi.IsCloseCollapsedState)
                                {
                                    restoreCollapsed = true;
                                    tvi.SetCloseCollapsedState(false);
                                }

                                tvi.Measure(Size.Infinity);

                                if (restoreCollapsed)
                                {
                                    tvi.SetCloseCollapsedState(true);
                                }

                                if (tvi.DesiredSize.Width > widestTabWidth)
                                {
                                    widestTabWidth = tvi.DesiredSize.Width;
                                }
                            }
                        }
                        var widestTabWidthClamped = double.Clamp(widestTabWidth > 0 ? widestTabWidth : minTabWidth, minTabWidth, maxTabWidth);

                        _tabColumn.MaxWidth = availableWidth + headerWidth + footerWidth;

                        // WinUI 等宽模式：让选项卡均匀拉伸以填充整个选项卡条，宽度限制在
                        // [最小值, 最大值] 范围内。只有当即使使用最小宽度仍然无法容纳所有选项卡时，
                        // 才回退到最宽选项卡的宽度，并启用滚动。
                        //
                        // 预留 18px 的余量，避免选项卡条被拉伸到完全贴合边缘；
                        // 当选项卡填满整个选项卡条时，末尾仍保留一小段间距。
                        var availableForTabs = availableWidth - (padding.Horizontal() + headerWidth + footerWidth) - 8;
                        var evenTabWidth = double.Clamp(availableForTabs / itemCount, minTabWidth, maxTabWidth);

                        var requiredWidthEven = evenTabWidth * tabCount + headerWidth + footerWidth + padding.Horizontal();
                        if (requiredWidthEven > availableWidth)
                        {
                            // Overflow: keep every tab at the widest tab's width (uniform) and show
                            // the scroll buttons.
                            tabWidth = widestTabWidthClamped;
                            _tabColumn.Width = new GridLength(availableWidth, GridUnitType.Pixel);
                            if (_listView != null)
                            {
                                _listView.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
                                UpdateScrollViewerDecreaseAndIncreaseButtonsViewState();
                            }
                        }
                        else
                        {
                            // Everything fits: stretch the tabs evenly to fill the entire strip.
                            tabWidth = evenTabWidth;

                            var requiredWidth = tabWidth * tabCount + headerWidth + footerWidth + padding.Horizontal();

                            // If we're dragging over the TabView, we need to set the width to a specific value,
                            // since we want it to be larger than the items actually in it in order to accommodate
                            // the item being dragged into the TabView.  Otherwise, we can just set its width to Auto.
                            _tabColumn.Width = _isItemDraggedOver ?
                                new GridLength(requiredWidth, GridUnitType.Pixel) :
                                new GridLength(1, GridUnitType.Auto);

                            if (_listView != null)
                            {
                                if (shouldUpdateWidths && fillAllAvailableSpace)
                                {
                                    _listView.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
                                }
                                else
                                {
                                    _scrollDecreaseButton?.IsEnabled = false;
                                    _scrollIncreaseButton?.IsEnabled = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Case: TabWidthMode "Compact" or "SizeToContent"
                        _tabColumn.MaxWidth = availableWidth;

                        if (_listView != null)
                        {
                            // When an item is being dragged over, we need to reserve extra space for the potential new tab,
                            // so we can't rely on auto sizing in that case.  However, the ListView expands to the size of the column,
                            // so we need to store the value lest we keep expanding the width of the column every time we call this method.
                            if (_isItemDraggedOver)
                            {
                                if (!_expandedWidthForDragOver.HasValue)
                                {
                                    _expandedWidthForDragOver = _listView.Bounds.Width + maxTabWidth;
                                }

                                _tabColumn.Width = new GridLength(_expandedWidthForDragOver.Value, GridUnitType.Pixel);
                            }
                            else
                            {
                                if (_expandedWidthForDragOver.HasValue)
                                {
                                    _expandedWidthForDragOver = null;
                                }

                                _tabColumn.Width = new GridLength(1, GridUnitType.Auto);
                            }

                            _listView.MaxWidth = availableWidth;

                            var ip = _itemsPresenter;
                            if (ip != null)
                            {
                                var visible = ip.Bounds.Width > availableWidth;
                                _listView.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, visible ?
                                    ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden);

                                if (visible)
                                {
                                    UpdateScrollViewerDecreaseAndIncreaseButtonsViewState();
                                }
                            }
                        }
                    }
                }                
            }
        }

        if (!isHorizontal)
        {
            if (_listView != null)
            {
                // If not in Horizontal, ensure we let the scrollviewer work correctly
                _listView.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
                _listView.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            }
            
            if (_tabContainerGrid != null)
            {
                // var rows = _tabContainerGrid.RowDefinitions;
                // Calcuate the height of the rows without the TabView
                double height = 0;
                foreach (var item in _tabContainerGrid.Children)
                {
                    // The list view is the item we're sizing, and the pane resize handle is an
                    // overlay that spans every row - neither takes vertical space away from it.
                    if (item is TabViewListView || item.Name == BORDER_RESIZE_HANDLE_HOST)
                        continue;

                    height += item.DesiredSize.Height;
                }
                var maxSpace = _tabContainerGrid.Bounds.Height;
                
                if (_isItemDraggedOver)
                {
                    // Add the dragging space in vertical view by using the avg. item height
                    height += (height / _tabContainerGrid.Children.Count);
                }

                _scrollViewer?.MaxHeight = double.Clamp(maxSpace - height, 0, double.PositiveInfinity);
            }
        }
        else if (_scrollViewer != null)
        {
            _scrollViewer.MaxHeight = double.PositiveInfinity;
        }

        // Cache for containers that get prepared later by virtualization (NaN = auto).
        CurrentTabWidth = tabWidth;

        if (shouldUpdateWidths || TabWidthMode != TabViewWidthMode.Equal)
        {
            foreach (var item in TabItems)
            {
                var tvi = item as TabViewItem ?? ContainerFromItem(item) as TabViewItem;
                if (tvi == null)
                    continue;

                tvi.Width = tabWidth;

                // Refresh the selected-background geometry immediately with the new size.
                // Relying on SizeChanged leaves a window where the geometry still holds the
                // previous Bounds, so the path renders narrower than the item (part of the
                // selected tab then appears "missing").
                tvi.RefreshTabGeometry();
            }
        }
    }

    private void UpdateSelectedItem()
    {
        if (_listView != null)
            _listView.SelectedItem = SelectedItem;
    }

    private void UpdateSelectedIndex()
    {
        if (_listView != null)
        {
            var index = SelectedIndex;
            if (index < _listView.ItemCount)
            {
                _listView.SelectedIndex = index;
            }
        }
    }

    public Control? ContainerFromItem(object item) => _listView?.ContainerFromItem(item);

    public Control? ContainerFromIndex(int index) => _listView?.ContainerFromIndex(index);

    public int IndexFromContainer(Control container) => _listView?.IndexFromContainer(container) ?? -1;

    public object? ItemFromContainer(Control container) => _listView?.ItemFromContainer(container);

    private void OnPaneResizeHandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Handled)
            return;

        var pt = e.GetCurrentPoint(null);
        if (e.Properties.IsLeftButtonPressed)
        {
            _initDragPanePoint = pt.Position;
            _startingPaneSize = VerticalOpenPaneLength;
        }
    }

    private void OnPaneResizeHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Handled)
            return;

        if (_initDragPanePoint.HasValue)
        {
            var point = e.GetCurrentPoint(null);
            var delta = (point.Position - _initDragPanePoint.Value).X;
            if (!_isDraggingPane)
            {
                UISettings.GetSystemDragSize(TopLevel.GetTopLevel(this).RenderScaling, out var cxDrag, out _);
                
                if (double.Abs(delta) < cxDrag)
                {
                    return;
                }

                _isDraggingPane = true;
            }

            var min = MinimumVerticalOpenPaneLength;
            var max = MaximumVerticalOpenPaneLength;

            if (TabStripLocation == TabViewTabStripLocation.Right)
                delta *= -1;

            var paneLength = _startingPaneSize;
            var length = double.Clamp(paneLength + delta, min, max);

            SetCurrentValue(VerticalOpenPaneLengthProperty, length);
        }
    }

    private void OnPaneResizeHandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Handled)
            return;

        if (_initDragPanePoint.HasValue)
        {
            var point = e.GetCurrentPoint(null);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                _initDragPanePoint = null;
                _isDraggingPane = false;
            }
        }
    }

    private void OnPaneResizeHandlePointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (e.Handled)
            return;

        _initDragPanePoint = null;
        _isDraggingPane = false;
    }

    private int GetItemCount()
    {
        if (_listView != null)
        {
            return _listView.ItemCount;
        }

        var src = TabItemsSource;
        if (src is ICollection collection)
        {
            return collection.Count;
        }
        else if (src != null)
        {
            return src.Cast<object>().Count();
        }
        else
        {
            return TabItems.Count;
        }
    }

    internal bool MoveFocus(bool moveForward)
    {
        var toplevel = TopLevel.GetTopLevel(this);
        if (toplevel != null)
        {
            var focusedControl = toplevel.FocusManager.GetFocusedElement() as Control;

            // If there's no focused control, then we have nothing to do.
            if (focusedControl == null)
                return false;

            // Focus goes in this order:
            //
            //    Tab 1 -> Tab 1 close button -> Tab 2 -> Tab 2 close button -> ... -> Tab N -> Tab N close button -> Add tab button -> Tab 1
            //
            // Any element that's not focusable is skipped.
            using var focusOrderList = new PooledList<Control>();
            
            for (int i = 0; i < GetItemCount(); i++)
            {
                if (ContainerFromIndex(i) is TabViewItem tab)
                {
                    if (IsFocusable(tab))
                    {
                        focusOrderList.Add(tab);

                        var closeButton = tab.CloseButton;
                        if (closeButton != null)
                        {
                            if (IsFocusable(closeButton))
                            {
                                focusOrderList.Add(closeButton);
                            }
                        }
                    }
                }
            }

            if (_addButton != null)
            {
                if (IsFocusable(_addButton))
                {
                    focusOrderList.Add(_addButton);
                }
            }

            var index = focusOrderList.IndexOf(focusedControl);

            // The focused control is not in the focus order list - nothing for us to do here either.
            if (index == -1)
            {
                return false;
            }

            // At this point, we know that the focused control is indeed in the focus list, so we'll move focus to the next or previous control in the list.
            int sourceIndex = index;
            int listSize = focusOrderList.Count;
            int increment = moveForward ? 1 : -1;
            int nextIndex = sourceIndex + increment;

            if (nextIndex < 0)
            {
                nextIndex = listSize - 1;
            }
            else if (nextIndex >= listSize)
            {
                nextIndex = 0;
            }

            // We have to do a bit of a dance for the close buttons - we don't want users to be able to give them focus when tabbing through an app,
            // since we only want to tab into the TabView once and then tab out on the next tab press.  However, IsTabStop also controls keyboard
            // focusability in general - we can't give keyboard focus to a control with IsTabStop = false.  To work around this, we'll temporarily set
            // IsTabStop = true before calling Focus(), and then set it back to false if it was previously false.

            var control = focusOrderList[nextIndex];
            bool originalIsTabStop = control.IsTabStop;

            try
            {
                control.IsTabStop = true;

                bool focusResult = control.Focus(NavigationMethod.Tab);
                return focusResult;
            }
            finally
            {
                control.IsTabStop = originalIsTabStop;
            }
        }

        return false;
    }

    private bool MoveSelection(bool moveForward)
    {
        int originalIndex = SelectedIndex;
        int itemCount = GetItemCount();
        if (originalIndex < 0 || itemCount == 0)
        {
            return false;
        }

        int increment = moveForward ? 1 : -1;
        int currentIndex = originalIndex + increment;

        while (currentIndex != originalIndex)
        {
            if (currentIndex < 0)
            {
                currentIndex = itemCount - 1;
            }
            else if (currentIndex >= itemCount)
            {
                currentIndex = 0;
            }

            var control = ContainerFromIndex(currentIndex);
            if (control != null && IsFocusable(control))
            {
                SelectedIndex = currentIndex;
                return true;
            }

            currentIndex += increment;
        }

        return false;
    }

    private bool RequestCloseCurrentTab()
    {
        bool handled = false;
        var tvi = SelectedItem as TabViewItem;
        if (tvi == null && SelectedItem != null)
        {
            tvi = ContainerFromItem(SelectedItem) as TabViewItem;
        }

        if (tvi != null)
        {
            if (tvi.IsClosable)
            {
                RequestCloseTab(tvi, true);
                handled = true;
            }
        }

        return handled;
    }

    protected virtual void OnKeyboardAcceleratorInvoked(object? parameter)
    {
        if (parameter != null)
        {
            switch ((TabViewCommandType)parameter)
            {
                case TabViewCommandType.CtrlF4:
                    RequestCloseCurrentTab();
                    break;

                case TabViewCommandType.CtrlTab:
                    MoveSelection(true);
                    break;

                case TabViewCommandType.CtrlShiftTab:
                    MoveSelection(false);
                    break;
            }
        }
    }

    private void OnAddButtonKeyDown(object? sender, KeyEventArgs args)
    {
        var addButton = _addButton;
        if (args.Key == Key.Right)
        {
            args.Handled = MoveFocus(addButton?.FlowDirection == Avalonia.Media.FlowDirection.LeftToRight);
        }
        else if (args.Key == Key.Left)
        {
            args.Handled = MoveFocus(addButton?.FlowDirection == Avalonia.Media.FlowDirection.RightToLeft);
        }
    }

    // Note that the parameter is a DependencyObject for convenience to allow us to call this on the return value of ContainerFromIndex.
    // There are some non-control elements that can take focus - e.g. a hyperlink in a RichTextBlock - but those aren't relevant for our purposes here.
    private bool IsFocusable(InputElement? obj, bool checkTabStop = false)
    {
        if (obj == null)
            return false;

        if (obj is Control c)
        {
            return c.IsEffectivelyVisible &&
                (c.IsEffectivelyEnabled) &&
                (c.IsTabStop || !checkTabStop);
        }

        return false;
    }

    private void UpdateIsItemDraggedOver(bool isItemDraggedOver)
    {
        if (_isItemDraggedOver != isItemDraggedOver)
        {
            _isItemDraggedOver = isItemDraggedOver;
            UpdateTabWidths();
        }
    }

    private void UnhookEventsAndClearFields()
    {
        if (_tabContainerGrid != null)
        {
            _tabContainerGrid.PointerEntered -= OnTabStripPointerEnter;
            _tabContainerGrid.PointerExited -= OnTabStripPointerLeave;
            _tabContainerGrid.SizeChanged -= HandleTabContainerGridSizeChangedForVerticalTabView;
        }

        if (_listView != null)
        {
            _listView.Loaded -= OnListViewLoaded;
            LogicalChildren.Remove(_listView);
            _listView.SelectionChanged -= OnListViewSelectionChanged;
            _listView.SizeChanged -= OnListViewSizeChanged;
            _listView.GettingFocus -= OnListViewGettingFocus;

            _listView.DragItemsStarting -= OnListViewDragItemsStarting;
            _listView.DragItemsCompleted -= OnListViewDragItemsCompleted;
            _listView.DragOver -= OnListViewDragOver;
            _listView.Drop -= OnListViewDrop;
            _listView.DragEnter -= OnListViewDragEnter;
            _listView.DragLeave -= OnListViewDragLeave;

            _listViewAllowDropPropertyChangedRevoker?.Dispose();
            _listViewCanReorderItemsPropertyChangedRevoker?.Dispose();
        }

        _addButton?.Click -= OnAddButtonClick;
        _addButton?.KeyDown -= OnAddButtonKeyDown;

        _itemsPresenter?.SizeChanged -= OnItemsPresenterSizeChanged;

        _scrollDecreaseButton?.Click -= OnScrollDecreaseClick;

        _scrollIncreaseButton?.Click -= OnScrollIncreaseClick;

        if (_scrollViewer != null)
        {
            _scrollViewer.Loaded -= OnScrollViewerLoaded;
        }

        _scrollViewer?.ScrollChanged -= OnScrollViewerViewChanged;

        if (_verticalPaneResizeHandle != null) // Null in Top/Bottom modes
        {
            _verticalPaneResizeHandle.PointerPressed -= OnPaneResizeHandlePointerPressed;
            _verticalPaneResizeHandle.PointerMoved -= OnPaneResizeHandlePointerMoved;
            _verticalPaneResizeHandle.PointerReleased -= OnPaneResizeHandlePointerReleased;
            _verticalPaneResizeHandle.PointerCaptureLost -= OnPaneResizeHandlePointerCaptureLost;
        }

        _leftContentColumn = null;
        _tabColumn = null;
        _addButtonColumn = null;
        _rightContentColumn = null;

        _listView = null;
        _tabContentPresenter = null;
        _rightContentPresenter = null;
        _tabContainerGrid = null;
        _scrollViewer = null;
        _scrollDecreaseButton = null;
        _scrollIncreaseButton = null;
        _addButton = null;
        _itemsPresenter = null;
        _verticalPaneResizeHandle = null;
    }

    internal static string GetClassForStripLocation(TabViewTabStripLocation loc)
    {
        return loc switch
        {
            TabViewTabStripLocation.Left => PC_LEFT,
            TabViewTabStripLocation.Bottom => PC_BOTTOM,
            TabViewTabStripLocation.Right => PC_RIGHT,
            _ => PC_TOP
        };
    }

    class TabViewCommand : ICommand
    {
        public TabViewCommand(Action<object?>? execute)
        {
            ExecuteHandler = execute;
        }

        event EventHandler? ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        public Action<object?>? ExecuteHandler { get; }
        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            ExecuteHandler?.Invoke(parameter);
        }
    }

    enum TabViewCommandType
    {
        CtrlF4,
        CtrlTab,
        CtrlShiftTab
    }
}
