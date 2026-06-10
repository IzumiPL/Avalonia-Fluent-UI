using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls.Metadata;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Controls.Primitives;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents the container for an item in a NavigationView control.
/// </summary>
[PseudoClasses(SharedPseudoclasses.s_pcLeftNav, SharedPseudoclasses.s_pcTopNav, SharedPseudoclasses.s_pcTopOverflow)]
[PseudoClasses(SharedPseudoclasses.s_pcIconLeft, SharedPseudoclasses.s_pcIconOnly, SharedPseudoclasses.s_pcContentOnly)]
[PseudoClasses(s_pcSelected)]
[PseudoClasses(s_pcIconCollapsed)]
[PseudoClasses(SharedPseudoclasses.s_pcChevronClosed, SharedPseudoclasses.s_pcChevronOpen, SharedPseudoclasses.s_pcChevronHidden)]
[PseudoClasses(s_pcInfoBadge)]
[TemplatePart(s_tpFlyoutContentGrid, typeof(Panel))]
[TemplatePart(s_tpNVIPresenter, typeof(NavigationViewItemPresenter))]
[TemplatePart(s_tpNVIRootGrid, typeof(Grid))]
[TemplatePart(s_tpNVIMenuItemsHost, typeof(ItemsRepeater))]
public partial class NavigationViewItem : NavigationViewItemBase
{
    /// <summary>
    /// Defines the <see cref="CompactPaneLength"/> property
    /// </summary>
    public static readonly StyledProperty<double> CompactPaneLengthProperty =
       AvaloniaProperty.Register<NavigationViewItem, double>(nameof(CompactPaneLength), 48.0);

    /// <summary>
    /// Defines the <see cref="HasUnrealizedChildren"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationViewItem, bool> HasUnrealizedChildrenProperty =
        AvaloniaProperty.RegisterDirect<NavigationViewItem, bool>(nameof(HasUnrealizedChildren),
            x => x.HasUnrealizedChildren, (x, v) => x.HasUnrealizedChildren = v);

    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<IconSource> IconSourceProperty =
        AvaloniaProperty.Register<NavigationViewItem, IconSource>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="IsChildSelected"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationViewItem, bool> IsChildSelectedProperty =
        AvaloniaProperty.RegisterDirect<NavigationViewItem, bool>(nameof(IsChildSelectedProperty),
            x => x.IsChildSelected, (x, v) => x.IsChildSelected = v);

    /// <summary>
    /// Defines the <see cref="IsExpanded"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationViewItem, bool> IsExpandedProperty =
        AvaloniaProperty.RegisterDirect<NavigationViewItem, bool>(nameof(IsExpanded),
            x => x.IsExpanded, (x, v) => x.IsExpanded = v);

    /// <summary>
    /// Defines the <see cref="MenuItems"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationViewItem, IList<object>> MenuItemsProperty =
        NavigationView.MenuItemsProperty.AddOwner<NavigationViewItem>(x => x.MenuItems);

    /// <summary>
    /// Defines the <see cref="MenuItemsSource"/> property
    /// </summary>
    public static readonly StyledProperty<IEnumerable> MenuItemsSourceProperty =
        NavigationView.MenuItemsSourceProperty.AddOwner<NavigationViewItem>();

    /// <summary>
    /// Defines the <see cref="SelectsOnInvoked"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationViewItem, bool> SelectsOnInvokedProperty =
        AvaloniaProperty.RegisterDirect<NavigationViewItem, bool>(nameof(SelectsOnInvoked),
            x => x.SelectsOnInvoked, (x, v) => x.SelectsOnInvoked = v);

    /// <summary>
    /// Defines the <see cref="InfoBadge"/> property
    /// </summary>
    public static readonly StyledProperty<InfoBadge> InfoBadgeProperty =
        AvaloniaProperty.Register<NavigationViewItem, InfoBadge>(nameof(InfoBadge));

    /// <summary>
    /// Gets the CompactPaneLength of the NavigationView that hosts this item.
    /// </summary>
    public double CompactPaneLength
    {
        get => GetValue(CompactPaneLengthProperty);
        private set => SetValue(CompactPaneLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the current item has child items that haven't been shown.
    /// </summary>
    public bool HasUnrealizedChildren
    {
        get => _hasUnrealizedChildren;
        set
        {
            if (SetAndRaise(HasUnrealizedChildrenProperty, ref _hasUnrealizedChildren, value))
            {
                OnHasUnrealizedChildrenPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the icon to show next to the menu item text.
    /// </summary>
    public IconSource IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the value that indicates whether or not descendant item is selected.
    /// </summary>
    public bool IsChildSelected
    {
        get => _isChildSelected;
        set => SetAndRaise(IsChildSelectedProperty, ref _isChildSelected, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a tree node is expanded. Ignored if there are no menu items.
    /// </summary>
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (SetAndRaise(IsExpandedProperty, ref _isExpanded, value))
            {
                OnIsExpandedPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the collection of menu items displayed as children of the NavigationViewItem.
    /// </summary>
    public IList<object> MenuItems
    {
        get => _menuItems;
        private set => SetAndRaise(MenuItemsProperty, ref _menuItems, value);
    }

    /// <summary>
    /// Gets or sets an object source used to generate the content of the NavigationViewItem submenu.
    /// </summary>
    public IEnumerable MenuItemsSource
    {
        get => GetValue(MenuItemsSourceProperty);
        set => SetValue(MenuItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether invoking a navigation menu item also selects it.
    /// </summary>
    public bool SelectsOnInvoked
    {
        get => _selectsOnInvoked;
        set => SetAndRaise(SelectsOnInvokedProperty, ref _selectsOnInvoked, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="InfoBadge"/> to display in the NavigationViewItem
    /// </summary>
    public InfoBadge InfoBadge
    {
        get => GetValue(InfoBadgeProperty);
        set => SetValue(InfoBadgeProperty, value);
    }

    //HELPER PROPERTIES

    internal Control SelectionIndicator => _presenter?.SelectionIndicator;

    internal NavigationViewItemPresenter NVIPresenter => _presenter;

    private bool HasChildren =>
        (MenuItems != null && MenuItems.Count() > 0) ||
        (MenuItemsSource != null && _repeater != null &&
        _repeater.ItemsSourceView != null &&
        _repeater.ItemsSourceView.Count > 0) ||
        HasUnrealizedChildren;

    private bool ShouldShowIcon => IconSource != null;

    private bool ShouldEnableToolTip => IsOnLeftNav && _isClosedCompact;

    private bool ShouldShowContent => Content != null;

    private bool IsOnLeftNav => Position == NavigationViewRepeaterPosition.LeftNav ||
        Position == NavigationViewRepeaterPosition.LeftFooter;

    private bool IsOnTopPrimary
    {
        get
        {
            bool isPaneDisplayModeTop = true;
            if (GetNavigationView is NavigationView nv)
            {
                // There is a delay between the NavigationViewPaneDisplayMode update and the 
                // position property of NavigationViewItem being updated. This function gets called
                // in that delay period, so we need to check the PaneDisplayMode as further verification
                // of whether we are in Top mode or switching away from it.
                isPaneDisplayModeTop = nv.PaneDisplayMode == NavigationViewPaneDisplayMode.Top;
            }

            return Position == NavigationViewRepeaterPosition.TopPrimary && isPaneDisplayModeTop;
        }
    }

    internal bool ShouldRepeaterShowInFlyout => (_isClosedCompact && IsTopLevelItem) || IsOnTopPrimary;

    internal bool IsRepeaterVisible => _repeater?.IsVisible ?? false;

    internal ItemsRepeater GetRepeater => _repeater;

    private bool _hasUnrealizedChildren;
    private bool _isChildSelected;
    private bool _isExpanded;
    private IList<object> _menuItems;
    private bool _selectsOnInvoked = true;

    private const string s_tpNVIPresenter = "NVIPresenter";
    private const string s_tpNVIRootGrid = "NVIRootGrid";
    private const string s_tpNVIMenuItemsHost = "NVIMenuItemsHost";
    private const string s_tpFlyoutContentGrid = "FlyoutContentGrid";
        
    private const string s_pcSelected = ":selected";
    private const string s_pcIconCollapsed = ":iconcollapsed";
    private const string s_pcInfoBadge = ":infobadge";
    
    /// <summary>
    /// Create instance of <see cref="NavigationViewItem"/>.
    /// </summary>
    public NavigationViewItem()
    {
        MenuItems = new AvaloniaList<object>();
    }

    /// <inheritdoc />
    protected override void OnNavigationViewItemBaseDepthChanged()
    {
        UpdateItemIndentation();
        PropagateDepthToChildren(Depth + 1);
    }

    /// <inheritdoc />
    protected override void OnNavigationViewItemBaseIsSelectedChanged()
    {
        UpdateVisualState();
    }

    /// <inheritdoc />
    protected override void OnNavigationViewItemBasePositionChanged()
    {
        UpdateVisualState();
        ReparentRepeater();

        // We can't set the Flyout position in Styles, so we change the position here
        if (_rootGrid != null)
        {
            var flyout = _rootGrid.GetValue(FlyoutBase.AttachedFlyoutProperty) as PopupFlyoutBase;
            if (flyout != null)
            {
                flyout.Placement = (Position == NavigationViewRepeaterPosition.TopPrimary ||
                    Position == NavigationViewRepeaterPosition.TopFooter) ?
                    PlacementMode.BottomEdgeAlignedLeft :
                    PlacementMode.RightEdgeAlignedTop;
            }
        }
    }

    /// <inheritdoc />
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _appliedTemplate = false;
        _restoreToExpandedState = false;

        UnhookEventsAndClearFields();

        base.OnApplyTemplate(e);

        _presenter = e.NameScope.Find<NavigationViewItemPresenter>(s_tpNVIPresenter);

        _rootGrid = e.NameScope.Find<Grid>(s_tpNVIRootGrid);
        if (_rootGrid != null)
        {
            var flyout = FlyoutBase.GetAttachedFlyout(_rootGrid) as PopupFlyoutBase;
            flyout?.Closing += OnFlyoutClosing;
        }

        var navView = GetNavigationView;
        //WinUI must have a different order of doing things b/c OnApplyTemplate is called BEFORE
        //OnElementPrepared in Avalonia, so NavigationView & SplitView refs don't exist yet. WinUI
        //must do this reversed and not add the item to the tree until AFTER OnElementPrepared
        //To compensate, we'll set the target now
        //Conveniently, this also means we don't need to impl winui #5039, because we are already
        //in the visual tree so the splitview reference is made
        if (navView == null)
        {
            navView = this.FindAncestorOfType<NavigationView>();
            SetNavigationViewParent(navView);
        }

        var splitView = GetSplitView;
        if (splitView != null)
        {
            PrepNavigationViewItem(splitView);
        }
        else
        {
            Loaded += HandleLoaded;
        }

        //var navView = GetNavigationView;
        if (navView != null)
        {
            _repeater = e.NameScope.Find<ItemsRepeater>(s_tpNVIMenuItemsHost);
            if (_repeater != null)
            {
                (_repeater.Layout as StackLayout).DisableVirtualization = true;

                _repeater.ElementPrepared += navView.OnRepeaterElementPrepared;
                _repeater.ElementClearing += navView.OnRepeaterElementClearing;

                _repeater.ItemTemplate = navView.ItemsFactory;
            }

            UpdateRepeaterItemsSource();
        }

        _flyoutContentGrid = e.NameScope.Find<Panel>(s_tpFlyoutContentGrid);

        _appliedTemplate = true;

        UpdateItemIndentation();
        UpdateVisualState();
        ReparentRepeater();
        // We dont want to update the repeater visibilty during OnApplyTemplate if NavigationView is in a mode when items are shown in a flyout
        if (!ShouldRepeaterShowInFlyout)
        {
            ShowHideChildren();
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconSourceProperty)
        {
            OnIconPropertyChanged(change);
        }
        else if (change.Property == ContentProperty)
        {
            OnContentChanged(change);
        }
        else if (change.Property == InfoBadgeProperty)
        {
            UpdateVisualStateForInfoBadge();
        }
        else if (change.Property == MenuItemsProperty)
        {
            OnMenuItemsPropertyChanged();
        }
        else if (change.Property == MenuItemsSourceProperty)
        {
            OnMenuItemsSourcePropertyChanged();
        }
    }

    private void UpdateRepeaterItemsSource()
    {
        if (_repeater != null)
        {
            if (_repeater.ItemsSourceView != null)
            {
                _repeater.ItemsSourceView.CollectionChanged -= OnItemsSourceViewChanged;
            }

            var miSource = MenuItemsSource;

            _repeater.ItemsSource = miSource != null ? miSource : _menuItems;

            if (_repeater.ItemsSourceView != null)
            {
                _repeater.ItemsSourceView.CollectionChanged += OnItemsSourceViewChanged;
            }
        }
    }

    private void OnItemsSourceViewChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        UpdateVisualStateForChevron();
    }

    private void OnSplitViewPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == SplitView.CompactPaneLengthProperty)
        {
            UpdateCompactPaneLength();
        }
        else if (args.Property == SplitView.IsPaneOpenProperty ||
            args.Property == SplitView.DisplayModeProperty)
        {
            UpdateIsClosedCompact();
            ReparentRepeater();
        }
    }

    private void UpdateCompactPaneLength()
    {
        var splitView = GetSplitView;
        if (splitView != null)
        {
            double paneLength = splitView.CompactPaneLength;
            CompactPaneLength = paneLength;

            if (_presenter != null)
            {
                _presenter.UpdateCompactPaneLength(paneLength, IsOnLeftNav);
            }
        }
    }

    private void UpdateIsClosedCompact()
    {
        var splitView = GetSplitView;
        if (splitView != null)
        {
            _isClosedCompact = !splitView.IsPaneOpen &&
                (splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay || splitView.DisplayMode == SplitViewDisplayMode.CompactInline);

            UpdateVisualState();
        }
    }

    private void UpdateVisualStateForClosedCompact()
    {
        if (_presenter != null)
        {
            _presenter.UpdateClosedCompactVisualState(IsTopLevelItem, _isClosedCompact);
        }
    }

    private void UpdateNavigationViewItemToolTip()
    {
        var tip = ToolTip.GetTip(this);

        if (tip == null || tip == _suggestedToolTipContent)
        {
            if (ShouldEnableToolTip)
            {
                if (tip != _suggestedToolTipContent)
                    ToolTip.SetTip(this, _suggestedToolTipContent);
            }
            else
            {
                ToolTip.SetTip(this, null);
            }
        }
    }

    private void SuggestedToolTipChanged(object newContent)
    {
        object newToolTip = null;
        if (newContent is string s)
        {
            newToolTip = s;
        }

        // Both customer and NavigationViewItem can update ToolTipContent by winrt::ToolTipService::SetToolTip or XAML
        // If the ToolTipContent is not the same as m_suggestedToolTipContent, then it's set by customer.
        // Customer's ToolTip take high priority, and we never override Customer's ToolTip.
        var toolTip = ToolTip.GetTip(this);
        if (_suggestedToolTipContent != null)
        {
            if (toolTip == _suggestedToolTipContent)
            {
                ToolTip.SetTip(this, null);
            }
        }

        _suggestedToolTipContent = newToolTip;
    }

    protected virtual void OnIsExpandedPropertyChanged()
    {
        _restoreToExpandedState = false;
        UpdateVisualStateForChevron();
    }

    protected virtual void OnIconPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        UpdateVisualState();
    }

    protected virtual void OnContentChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SuggestedToolTipChanged(args.NewValue);
        UpdateVisualState();

        if (!IsOnLeftNav)
        {
            var navView = GetNavigationView;
            navView?.TopNavigationViewItemContentChanged();
        }
    }

    protected virtual void OnMenuItemsPropertyChanged()
    {
        // We shouldn't need this now as Avalonia has no support for x:Load
        // see WinUI #6808
        //if (_menuItems.Count > 0)
        //{
        //    LoadElementsForDisplayingChildren();
        //}

        UpdateRepeaterItemsSource();
        UpdateVisualStateForChevron();
    }

    protected virtual void OnMenuItemsSourcePropertyChanged()
    {
        // See above
        //LoadElementsForDisplayingChildren();
        UpdateRepeaterItemsSource();
        UpdateVisualStateForChevron();
    }
        
    private void OnHasUnrealizedChildrenPropertyChanged()
    {
        UpdateVisualStateForChevron();
    }

    private void ShowSelectionIndicator(bool vis)
    {
        if (SelectionIndicator != null)
        {
            SelectionIndicator.Opacity = vis ? 1.0 : 0.0;
        }
    }

    private void UpdateVisualStateForIconAndContent(bool showIcon, bool showContent)
    {
        if (_presenter != null)
        {
            //Possible states :iconleft, :icononly, :contentonly
            ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcIconLeft, showIcon && showContent);
            ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcIconOnly, showIcon && !showContent);
            ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcContentOnly, !showIcon);
        }
    }

    private void UpdateVisualStateForNavigationViewPositionChange()
    {
        // v2: We no longer need to propagate the styles with ControlThemes, however,
        // for compat and external support we will still set the pseudoclasses
        switch (Position)
        {
            case NavigationViewRepeaterPosition.LeftNav:
            case NavigationViewRepeaterPosition.LeftFooter:
                PseudoClasses.Set(SharedPseudoclasses.s_pcLeftNav, true);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopNav, false);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopOverflow, false);

                if (_presenter != null)
                {
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcLeftNav, true);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopNav, false);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopOverflow, false);
                }

                break;

            case NavigationViewRepeaterPosition.TopPrimary:
            case NavigationViewRepeaterPosition.TopFooter:
                _restoreToExpandedState = false;
                PseudoClasses.Set(SharedPseudoclasses.s_pcLeftNav, false);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopNav, true);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopOverflow, false);

                if (_presenter != null)
                {
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcLeftNav, false);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopNav, true);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopOverflow, false);
                }
                break;

            case NavigationViewRepeaterPosition.TopOverflow:
                _restoreToExpandedState = false;
                PseudoClasses.Set(SharedPseudoclasses.s_pcLeftNav, false);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopNav, false);
                PseudoClasses.Set(SharedPseudoclasses.s_pcTopOverflow, true);

                if (_presenter != null)
                {
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcLeftNav, false);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopNav, false);
                    ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcTopOverflow, true);
                }
                break;
        }

        UpdateVisualStateForClosedCompact();
    }

    private void UpdateVisualStateForToolTip()
    {
        UpdateNavigationViewItemToolTip();
    }

    internal void UpdateVisualState()
    {
        if (!_appliedTemplate)
            return;

        if (_presenter != null)
        {
            ((IPseudoClasses)_presenter.Classes).Set(s_pcSelected, IsSelected);
        }

        UpdateVisualStateForNavigationViewPositionChange();

        bool showIcon = ShouldShowIcon;
        bool showContent = ShouldShowContent;

        if (IsOnLeftNav)
        {
            if (_presenter != null)
            {
                //This is supposed to be for backwards compatibility with RS4-, but
                //is apparently still used in the NVIPresenterWhenOnLeftPane style
                ((IPseudoClasses)_presenter.Classes).Set(s_pcIconCollapsed, !showIcon);
                //Only using IconCollapsed, IconVisible is default
            }
        }
        else
        {
            if (_presenter != null)
            {
                ((IPseudoClasses)_presenter.Classes).Set(s_pcIconCollapsed, false);
            }
        }

        UpdateVisualStateForToolTip();

        UpdateVisualStateForIconAndContent(showIcon, showContent);

        UpdateVisualStateForInfoBadge();

        UpdateVisualStateForChevron();
    }

    private void UpdateVisualStateForChevron()
    {
        if (_presenter != null)
        {
            //auto const chevronState = HasChildren() && !(m_isClosedCompact && ShouldRepeaterShowInFlyout()) ? 
            //                          (IsExpanded() ? c_chevronVisibleOpen : c_chevronVisibleClosed) : c_chevronHidden;
            //winrt::VisualStateManager::GoToState(presenter, chevronState, true);

            //States :chevronopen, :chevronclosed, :chevronhidden

            bool show = HasChildren && !(_isClosedCompact && ShouldRepeaterShowInFlyout);
            bool expand = IsExpanded;

            if (_presenter != null)
            {
                ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcChevronOpen, show && expand);
                ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcChevronClosed, show & !expand);
                ((IPseudoClasses)_presenter.Classes).Set(SharedPseudoclasses.s_pcChevronHidden, !show);
            }
        }
    }

    internal void ShowHideChildren()
    {
        if (_repeater == null)
            return;

        bool shouldShowChildren = IsExpanded;
        _repeater.IsVisible = shouldShowChildren;

        if (ShouldRepeaterShowInFlyout)
        {
            if (shouldShowChildren)
            {
                if (!_isRepeaterParentedToFlyout)
                {
                    ReparentRepeater();
                }

                Dispatcher.UIThread.Post(() => FlyoutBase.ShowAttachedFlyout(_rootGrid));
            }
            else
            {
                FlyoutBase.GetAttachedFlyout(_rootGrid)?.Hide();
            }
        }
    }

    private void ReparentRepeater()
    {
        if (HasChildren && _repeater != null)
        {
            if (ShouldRepeaterShowInFlyout && !_isRepeaterParentedToFlyout)
            {
                _rootGrid.Children.Remove(_repeater);
                _flyoutContentGrid.Children.Add(_repeater);
                _isRepeaterParentedToFlyout = true;

                PropagateDepthToChildren(0);
            }
            else if (!ShouldRepeaterShowInFlyout && _isRepeaterParentedToFlyout)
            {
                _flyoutContentGrid.Children.Remove(_repeater);
                _rootGrid.Children.Add(_repeater);
                _isRepeaterParentedToFlyout = false;

                PropagateDepthToChildren(1);
            }
        }
    }

    private void UpdateItemIndentation()
    {
        if (_presenter != null)
        {
            var newLeftmargin = Depth * _itemIndentation;
            _presenter.UpdateContentLeftIndentation(newLeftmargin);
        }
    }

    internal void PropagateDepthToChildren(int depth)
    {
        if (_repeater == null || _repeater.ItemsSourceView == null)
            return;

        var count = _repeater.ItemsSourceView.Count;

        for (int i = 0; i < count; i++)
        {
            if (_repeater.TryGetElement(i) is NavigationViewItemBase nvib)
            {
                nvib.Depth = depth;
            }
        }
    }

    internal void OnExpandCollapseChevronTapped(object sender, RoutedEventArgs args)
    {
        IsExpanded = !IsExpanded;
        args.Handled = true;
    }

    private void OnFlyoutClosing(object sender, CancelEventArgs args)
    {
        IsExpanded = false;
    }

    internal void RotateExpandCollapseChevron(bool isExpanded)
    {
        if (_presenter != null)
        {
            _presenter.RotateExpandCollapseChevron(isExpanded);
        }
    }

    private void UnhookEventsAndClearFields()
    {
        if (_rootGrid != null)
        {
            var flyout = FlyoutBase.GetAttachedFlyout(_rootGrid) as PopupFlyoutBase;
            if (flyout != null)
            {
                flyout.Closing -= OnFlyoutClosing;
            }
            _rootGrid = null;
        }

        _splitViewRevokers?.Dispose();
        _splitViewRevokers = null;

        var navView = GetNavigationView;
        if (navView != null && _repeater != null)
        {
            _repeater.ElementPrepared -= navView.OnRepeaterElementPrepared;
            _repeater.ElementClearing -= navView.OnRepeaterElementClearing;

            if (_repeater.ItemsSourceView != null)
            {
                _repeater.ItemsSourceView.CollectionChanged -= OnItemsSourceViewChanged;
            }
            _repeater.ItemsSource = null;
            _repeater = null;
        }

        _presenter = null;
        _flyoutContentGrid = null;
    }

    private void UpdateVisualStateForInfoBadge()
    {
        if (_presenter != null)
            ((IPseudoClasses)_presenter.Classes).Set(s_pcInfoBadge, InfoBadge != null);
    }

    public override string ToString()
    {
        return Content?.ToString() ?? "NavigationViewItem";
    }

    private void PrepNavigationViewItem(SplitView splitView)
    {
        _splitViewRevokers = new FACompositeDisposable(
            splitView.GetPropertyChangedObservable(SplitView.IsPaneOpenProperty).Subscribe(OnSplitViewPropertyChanged),
            splitView.GetPropertyChangedObservable(SplitView.DisplayModeProperty).Subscribe(OnSplitViewPropertyChanged),
            splitView.GetPropertyChangedObservable(SplitView.CompactPaneLengthProperty).Subscribe(OnSplitViewPropertyChanged));

        UpdateCompactPaneLength();
        UpdateIsClosedCompact();
    }

    private void HandleLoaded(object sender, RoutedEventArgs args)
    {
        if (GetSplitView is SplitView sv)
        {
            PrepNavigationViewItem(sv);
        }

        UpdateVisualStateForChevron();
        Loaded -= HandleLoaded;
    }

    // NavigationView needs to force collapse top level items when the pane closes.
    // This is done to avoid a compact state with children showing.
    // This is done in a way that allows the control to restore the expanded
    // state when the pane is opened again.
    private void HandleExpansionStateMemory()
    {
        if (IsTopLevelItem)
        {
            if (GetSplitView is SplitView sv)
            {
                if (sv.IsPaneOpen)
                {
                    RestoreExpandedState();
                }
                else
                {
                    ForceCollapse();
                }
            }
        }
    }

    private void ForceCollapse()
    {
        if (IsExpanded)
        {
            IsExpanded = false;
            _restoreToExpandedState = true;
        }
    }

    private void RestoreExpandedState()
    {
        if (_restoreToExpandedState)
        {
            IsExpanded = true;
            _restoreToExpandedState = false;
        }
    }

    private FACompositeDisposable _splitViewRevokers;
    private NavigationViewItemPresenter _presenter;
    private object _suggestedToolTipContent;
    private ItemsRepeater _repeater;
    private Panel _flyoutContentGrid;
    private Grid _rootGrid;

    private bool _isClosedCompact;
    private bool _appliedTemplate;
    //private bool _hasKeyboardFocus;//TODO: needed?
    private bool _isRepeaterParentedToFlyout;
    private bool _restoreToExpandedState;
}
