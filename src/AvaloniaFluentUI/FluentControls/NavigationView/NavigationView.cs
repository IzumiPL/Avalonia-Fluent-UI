using System;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Automation;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Avalonia.Styling;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Media.Animation;
using AvaloniaFluentUI.Locale;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents a container that enables navigation of app content. It has a header, 
/// a view for the main content, and a menu pane for navigation commands.
/// </summary>
[PseudoClasses(s_pcListSizeCompact, s_pcClosedCompact)]
[PseudoClasses(s_pcBackButtonCollapsed, s_pcPaneCollapsed, s_pcHeaderCollapsed)]
[PseudoClasses(s_pcMinimalWithBack, s_pcMinimal, s_pcTopNavMinimal, s_pcCompact, s_pcExpanded)]
[PseudoClasses(s_pcAutoSuggestCollapsed, s_pcSettingsCollapsed, s_pcPaneToggleCollapsed, s_pcPaneNotOverlaying)]
[TemplatePart(s_tpTogglePaneButton, typeof(Button))]
[TemplatePart(s_tpPaneHeaderContentBorder, typeof(ContentControl))]
[TemplatePart(s_tpPaneCustomContentBorder, typeof(ContentControl))]
[TemplatePart(s_tpFooterContentBorder, typeof(ContentControl))]
[TemplatePart(s_tpPaneHeaderOnTopPane, typeof(ContentControl))]
[TemplatePart(s_tpPaneTitleOnTopPane, typeof(ContentControl))]
[TemplatePart(s_tpPaneCustomContentOnTopPane, typeof(ContentControl))]
[TemplatePart(s_tpPaneFooterOnTopPane, typeof(ContentControl))]
[TemplatePart(s_tpRootSplitView, typeof(SplitView))]
[TemplatePart(s_tpTopNavGrid, typeof(Grid))]
[TemplatePart(s_tpMenuItemsHost, typeof(ItemsRepeater))]
[TemplatePart(s_tpTopNavMenuItemsHost, typeof(ItemsRepeater))]
[TemplatePart(s_tpTopNavMenuItemsOverflowHost, typeof(ItemsRepeater))]
[TemplatePart(s_tpTopNavOverflowButton, typeof(Button))]
[TemplatePart(s_tpFooterMenuItemsHost, typeof(ItemsRepeater))]
[TemplatePart(s_tpTopFooterMenuItemsHost, typeof(ItemsRepeater))]
[TemplatePart(s_tpTopNavContentOverlayAreaGrid, typeof(Border))]
[TemplatePart(s_tpPaneAutoSuggestBoxPresenter, typeof(ContentControl))]
[TemplatePart(s_tpTopPaneAutoSuggestBoxPresenter, typeof(ContentControl))]
[TemplatePart(s_tpPaneContentGrid, typeof(Grid))]
[TemplatePart(s_tpContentLeftPadding, typeof(Rectangle))]
[TemplatePart(s_tpPlaceholderGrid, typeof(Grid))]
[TemplatePart(s_tpPaneTitleTextBlock, typeof(Control))]
[TemplatePart(s_tpPaneTitlePresenter, typeof(ContentControl))]
[TemplatePart(s_tpPaneTitleHolder, typeof(Control))]
[TemplatePart(s_tpPaneAutoSuggestButton, typeof(Button))]
[TemplatePart(s_tpNavigationViewBackButton, typeof(Button))]
[TemplatePart(s_tpNavigationViewCloseButton, typeof(Button))]
[TemplatePart(s_tpMenuItemsScrollViewer, typeof(SmoothScrollViewer))]
[TemplatePart(s_tpFooterItemsScrollViewer, typeof(SmoothScrollViewer))]
[TemplatePart(s_tpItemsContainerGrid, typeof(Control))]
public partial class NavigationView : HeaderedContentControl
{
    /// <summary>
    /// Defines the <see cref="AlwaysShowHeader"/> property
    /// </summary>
    public static readonly StyledProperty<bool> AlwaysShowHeaderProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(AlwaysShowHeader), true);

    /// <summary>
    /// Defines the <see cref="AutoCompleteBox"/> property
    /// </summary>
    public static readonly StyledProperty<Control> AutoCompleteBoxProperty =
        AvaloniaProperty.Register<NavigationView, Control>(nameof(AutoCompleteBox));

    /// <summary>
    /// Defines the <see cref="CompactModeThresholdWidth"/> property
    /// </summary>
    public static readonly StyledProperty<double> CompactModeThresholdWidthProperty =
        AvaloniaProperty.Register<NavigationView, double>(nameof(CompactModeThresholdWidth),
            641.0, coerce: CoercePropertyValueToGreaterThanZero);

    /// <summary>
    /// Defines the <see cref="CompactPaneLength"/> property
    /// </summary>
    public static readonly StyledProperty<double> CompactPaneLengthProperty =
        AvaloniaProperty.Register<NavigationView, double>(nameof(CompactPaneLength),
            48.0, coerce: CoercePropertyValueToGreaterThanZero);

    /// <summary>
    /// Defines the <see cref="ContentOverlay"/> property
    /// </summary>
    public static readonly StyledProperty<Control> ContentOverlayProperty =
        AvaloniaProperty.Register<NavigationView, Control>(nameof(ContentOverlay));

    /// <summary>
    /// Defines the <see cref="DisplayMode"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationView, NavigationViewDisplayMode> DisplayModeProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, NavigationViewDisplayMode>(nameof(DisplayMode),
            x => x.DisplayMode);

    /// <summary>
    /// Defines the <see cref="ExpandedModeThresholdWidth"/> property
    /// </summary>
    public static readonly StyledProperty<double> ExpandedModeThresholdWidthProperty =
        AvaloniaProperty.Register<NavigationView, double>(nameof(ExpandedModeThresholdWidth), 1008.0,
            coerce: CoercePropertyValueToGreaterThanZero);

    /// <summary>
    /// Defines the <see cref="FooterMenuItemsProperty"/>
    /// </summary>
    public static readonly DirectProperty<NavigationView, IList<object>> FooterMenuItemsProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, IList<object>>(nameof(FooterMenuItems),
            x => x.FooterMenuItems);

    /// <summary>
    /// Defines the <see cref="FooterMenuItems"/> property
    /// </summary>
    public static readonly StyledProperty<IEnumerable> FooterMenuItemsSourceProperty =
        AvaloniaProperty.Register<NavigationView, IEnumerable>(nameof(FooterMenuItemsSource));
        
    /// <summary>
    /// Defines the <see cref="IsBackButtonVisible"/> property
    /// </summary>
    /// <remarks>
    /// In WinUI, this is an enum NavigationViewBackButtonVisible with values
    /// Visible, Collapsed, and Auto (depends on form factor). For our purposes,
    /// bool works just fine for now
    /// </remarks>
    public static readonly StyledProperty<bool> IsBackButtonVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsBackButtonVisible));

    /// <summary>
    /// Defines the <see cref="IsBackEnabled"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsBackEnabledProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsBackEnabled), false);

    /// <summary>
    /// Defines the <see cref="IsPaneOpen"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        SplitView.IsPaneOpenProperty.AddOwner<NavigationView>(
            new StyledPropertyMetadata<bool>(defaultValue: true));

    /// <summary>
    /// Defines the <see cref="IsPaneToggleButtonVisible"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsPaneToggleButtonVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsPaneToggleButtonVisible), true);

    /// <summary>
    /// Defines the <see cref="IsPaneVisible"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsPaneVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsPaneVisible), true);

    /// <summary>
    /// Defines the <see cref="IsSettingsVisible"/> property
    /// </summary>
    public static readonly StyledProperty<bool> IsSettingsVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsSettingsVisible), true);

    //SKIP for now, IsTitleBarAutoPaddingEnabled...

    /// <summary>
    /// Defines the <see cref="MenuItems"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationView, IList<object>> MenuItemsProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, IList<object>>(nameof(MenuItems),
            o => o.MenuItems);

    /// <summary>
    /// Defines the <see cref="MenuItemsSource"/> property
    /// </summary>
    public static readonly StyledProperty<IEnumerable> MenuItemsSourceProperty =
        AvaloniaProperty.Register<NavigationView, IEnumerable>(nameof(MenuItemsSource));

    /// <summary>
    /// Defines the <see cref="MenuItemTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate> MenuItemTemplateProperty =
        AvaloniaProperty.Register<NavigationView, IDataTemplate>(nameof(MenuItemTemplate));

    /// <summary>
    /// Defines the <see cref="MenuItemTemplate"/> property
    /// </summary>
    public static readonly StyledProperty<DataTemplateSelector> MenuItemTemplateSelectorProperty =
        AvaloniaProperty.Register<NavigationView, DataTemplateSelector>(nameof(MenuItemTemplateSelector));

    public static readonly StyledProperty<ControlTheme> MenuItemContainerThemeProperty =
        AvaloniaProperty.Register<NavigationView, ControlTheme>(nameof(MenuItemContainerTheme));

    /// <summary>
    /// Defines the <see cref="OpenPaneLength"/> property
    /// </summary>
    public static readonly StyledProperty<double> OpenPaneLengthProperty =
        AvaloniaProperty.Register<NavigationView, double>(nameof(OpenPaneLength),
            320.0, coerce: CoercePropertyValueToGreaterThanZero);

    //OverflowLabelModeProperty removed, as it was deprecated

    /// <summary>
    /// Defines the <see cref="PaneCustomContent"/> property
    /// </summary>
    public static readonly StyledProperty<Control> PaneCustomContentProperty =
        AvaloniaProperty.Register<NavigationView, Control>(nameof(PaneCustomContent));

    /// <summary>
    /// Defines the <see cref="PaneDisplayMode"/> property
    /// </summary>
    public static readonly StyledProperty<NavigationViewPaneDisplayMode> PaneDisplayModeProperty =
        AvaloniaProperty.Register<NavigationView, NavigationViewPaneDisplayMode>(nameof(PaneDisplayMode),
            NavigationViewPaneDisplayMode.Auto);

    /// <summary>
    /// Defines the <see cref="PaneFooter"/> property
    /// </summary>
    public static readonly StyledProperty<Control> PaneFooterProperty =
        AvaloniaProperty.Register<NavigationView, Control>(nameof(PaneFooter));

    /// <summary>
    /// Defines the <see cref="PaneHeader"/> property
    /// </summary>
    public static readonly StyledProperty<Control> PaneHeaderProperty =
        AvaloniaProperty.Register<NavigationView, Control>(nameof(PaneHeader));

    /// <summary>
    /// Defines the <see cref="PaneTitle"/> property
    /// </summary>
    public static readonly StyledProperty<string> PaneTitleProperty =
        AvaloniaProperty.Register<NavigationView, string>(nameof(PaneTitle));

    /// <summary>
    /// Defines the <see cref="SelectedItem"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationView, object> SelectedItemProperty =
        SelectingItemsControl.SelectedItemProperty.AddOwner<NavigationView>(x => x.SelectedItem, 
            (x, v) => x.SelectedItem = v, 
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="SelectionFollowsFocus"/> property
    /// </summary>
    /// <remarks>
    /// WinUI uses an enum here, but only has Disabled/Enabled, so just use bool
    /// </remarks>
    public static readonly StyledProperty<bool> SelectionFollowsFocusProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(SelectionFollowsFocus));

    /// <summary>
    /// Defines the <see cref="SettingsItem"/> property
    /// </summary>
    public static readonly DirectProperty<NavigationView, NavigationViewItem> SettingsItemProperty =
        AvaloniaProperty.RegisterDirect<NavigationView, NavigationViewItem>(nameof(SettingsItem),
            x => x.SettingsItem);

    //Ignore Shoulder Navigation (xbox)

    /// <summary>
    /// Defines the <see cref="TemplateSettings"/> property
    /// </summary>
    public static readonly StyledProperty<NavigationViewTemplateSettings> TemplateSettingsProperty =
        AvaloniaProperty.Register<NavigationView, NavigationViewTemplateSettings>(nameof(TemplateSettings));

    public static readonly StyledProperty<ICommand> BackCommandProperty =
        AvaloniaProperty.Register<NavigationView, ICommand>(nameof(BackCommand));

    public static readonly StyledProperty<bool> PaneFooterSeparatorIsVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(PaneFooterSeparatorIsVisible));

    public bool PaneFooterSeparatorIsVisible
    {
        get => GetValue(PaneFooterSeparatorIsVisibleProperty);
        set => SetValue(PaneFooterSeparatorIsVisibleProperty, value);
    }

    public ICommand BackCommand
    {
        get => GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the header is always visible.
    /// </summary>
    public bool AlwaysShowHeader
    {
        get => GetValue(AlwaysShowHeaderProperty);
        set => SetValue(AlwaysShowHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets an <see cref="Avalonia.Controls.AutoCompleteBox"/> to be displayed in the NavigationView.
    /// </summary>
    public Control AutoCompleteBox
    {
        get => GetValue(AutoCompleteBoxProperty);
        set => SetValue(AutoCompleteBoxProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum window width at which the NavigationView enters Compact display mode.
    /// </summary>
    public double CompactModeThresholdWidth
    {
        get => GetValue(CompactModeThresholdWidthProperty);
        set => SetValue(CompactModeThresholdWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the NavigationView pane in its compact display mode.
    /// </summary>
    public double CompactPaneLength
    {
        get => GetValue(CompactPaneLengthProperty);
        set => SetValue(CompactPaneLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets a UI element that is shown at the top of the control, below the pane 
    /// if PaneDisplayMode is Top.
    /// </summary>
    public Control ContentOverlay
    {
        get => GetValue(ContentOverlayProperty);
        set => SetValue(ContentOverlayProperty, value);
    }

    /// <summary>
    /// Gets a value that specifies how the pane and content areas of a NavigationView are being shown.
    /// </summary>
    public NavigationViewDisplayMode DisplayMode
    {
        get => _displayMode;
        private set => SetAndRaise(DisplayModeProperty, ref _displayMode, value);
    }

    /// <summary>
    /// Gets or sets the minimum window width at which the NavigationView enters Expanded display mode.
    /// </summary>
    public double ExpandedModeThresholdWidth
    {
        get => GetValue(ExpandedModeThresholdWidthProperty);
        set => SetValue(ExpandedModeThresholdWidthProperty, value);
    }

    /// <summary>
    /// Gets the list of objects to be used as navigation items in the footer menu.
    /// </summary>
    public IList<object> FooterMenuItems
    {
        get => _footerMenuItems;
        private set => SetAndRaise(FooterMenuItemsProperty, ref _footerMenuItems, value);
    }

    /// <summary>
    /// Gets or sets the object that represents the navigation items to be used in the footer menu.
    /// </summary>
    public IEnumerable FooterMenuItemsSource
    {
        get => GetValue(FooterMenuItemsSourceProperty);
        set => SetValue(FooterMenuItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the back button is visible or not.
    /// </summary>
    public bool IsBackButtonVisible
    {
        get => GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the back button is enabled or disabled.
    /// </summary>
    public bool IsBackEnabled
    {
        get => GetValue(IsBackEnabledProperty);
        set => SetValue(IsBackEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies whether the NavigationView pane is expanded to its full width.
    /// </summary>
    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the menu toggle button is shown.
    /// </summary>
    public bool IsPaneToggleButtonVisible
    {
        get => GetValue(IsPaneToggleButtonVisibleProperty);
        set => SetValue(IsPaneToggleButtonVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that determines whether the pane is shown.
    /// </summary>
    public bool IsPaneVisible
    {
        get => GetValue(IsPaneVisibleProperty);
        set => SetValue(IsPaneVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the settings button is shown.
    /// </summary>
    public bool IsSettingsVisible
    {
        get => GetValue(IsSettingsVisibleProperty);
        set => SetValue(IsSettingsVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the DataTemplate used to display each menu item.
    /// </summary>
    public IDataTemplate MenuItemTemplate
    {
        get => GetValue(MenuItemTemplateProperty);
        set => SetValue(MenuItemTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets a reference to a custom DataTemplateSelector logic class. The DataTemplateSelector 
    /// referenced by this property returns a template to apply to items.
    /// </summary>
    /// <remarks>
    /// This property should generally not be used but was added to support different containers for different
    /// data types. Should a more "Avalonia-like" solution arise, this property will be removed
    /// </remarks>
    public DataTemplateSelector MenuItemTemplateSelector
    {
        get => GetValue(MenuItemTemplateSelectorProperty);
        set => SetValue(MenuItemTemplateSelectorProperty, value);
    }

    /// <summary>
    /// Gets or sets the ControlTheme applied to MenuItems
    /// </summary>
    public ControlTheme MenuItemContainerTheme
    {
        get => GetValue(MenuItemContainerThemeProperty);
        set => SetValue(MenuItemContainerThemeProperty, value);
    }

    /// <summary>
    /// Gets the collection of menu items displayed in the NavigationView.
    /// </summary>
    public IList<object> MenuItems
    {
        get => _menuItems;
        set => SetAndRaise(MenuItemsProperty, ref _menuItems, value);
    }

    /// <summary>
    /// Gets or sets an object source used to generate the content of the NavigationView menu.
    /// </summary>
    public IEnumerable MenuItemsSource
    {
        get => GetValue(MenuItemsSourceProperty);
        set => SetValue(MenuItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the NavigationView pane when it's fully expanded.
    /// </summary>
    public double OpenPaneLength
    {
        get => GetValue(OpenPaneLengthProperty);
        set => SetValue(OpenPaneLengthProperty, value);
    }

    //OverflowLabelMode removed, deprecated in WinUI

    /// <summary>
    /// Gets or sets a UI element that is shown in the NavigationView pane.
    /// </summary>
    public Control PaneCustomContent
    {
        get => GetValue(PaneCustomContentProperty);
        set => SetValue(PaneCustomContentProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates how and where the NavigationView pane is shown.
    /// </summary>
    public NavigationViewPaneDisplayMode PaneDisplayMode
    {
        get => GetValue(PaneDisplayModeProperty);
        set => SetValue(PaneDisplayModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the content for the pane footer.
    /// </summary>
    public Control PaneFooter
    {
        get => GetValue(PaneFooterProperty);
        set => SetValue(PaneFooterProperty, value);
    }

    /// <summary>
    /// Gets or sets the content for the pane header.
    /// </summary>
    public Control PaneHeader
    {
        get => GetValue(PaneHeaderProperty);
        set => SetValue(PaneHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the label adjacent to the menu icon when the NavigationView pane is open.
    /// </summary>
    public string PaneTitle
    {
        get => GetValue(PaneTitleProperty);
        set => SetValue(PaneTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    public object SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
        }
    }

    //WinUI uses an enum here, but only has Disabled/Enabled, so just use bool
    /// <summary>
    /// Gets or sets a value that indicates whether item selection changes when keyboard focus changes.
    /// </summary>
    /// <remarks>
    /// Do not set this property to true if you have hierarchical navigation as things get weird. This
    /// behavior also occurs in WinUI
    /// </remarks>
    public bool SelectionFollowsFocus
    {
        get => GetValue(SelectionFollowsFocusProperty);
        set => SetValue(SelectionFollowsFocusProperty, value);
    }

    /// <summary>
    /// Gets the navigation item that represents the entry point to app settings.
    /// </summary>
    public NavigationViewItem SettingsItem
    {
        get => _settingsItem;
        internal set => SetAndRaise(SettingsItemProperty, ref _settingsItem, value);
    }

    /// <summary>
    /// Gets an object that provides calculated values that can be referenced as TemplateBinding sources 
    /// when defining templates for a NavigationView control.
    /// </summary>
    public NavigationViewTemplateSettings TemplateSettings
    {
        get => GetValue(TemplateSettingsProperty);
        protected set => SetValue(TemplateSettingsProperty, value);
    }

    /// <summary>
    /// Coerces a double to ensure its valid for use, i.e. >= 0 and not NaN or infinity
    /// Used for: CompactModeThresholdWidthProperty, CompactPaneLengthProperty, 
    /// ExpandedModeThresholdWidthProperty, and OpenPaneLengthProperty
    /// </summary>
    /// <returns></returns>
    private static double CoercePropertyValueToGreaterThanZero(AvaloniaObject arg1, double arg2)
    {
        if (double.IsNaN(arg2) || double.IsInfinity(arg2))
            return 0;
        return Math.Max(arg2, 0.0);
    }

    /// <summary>
    /// Occurs when the NavigationView pane is closing.
    /// </summary>
    public event TypedEventHandler<NavigationView, NavigationViewPaneClosingEventArgs> PaneClosing;

    /// <summary>
    /// Occurs when the NavigationView pane is closed.
    /// </summary>
    public event TypedEventHandler<NavigationView, EventArgs> PaneClosed;

    /// <summary>
    /// Occurs when the NavigationView pane is opening.
    /// </summary>
    public event TypedEventHandler<NavigationView, EventArgs> PaneOpening;

    /// <summary>
    /// Occurs when the NavigationView pane is opened.
    /// </summary>
    public event TypedEventHandler<NavigationView, EventArgs> PaneOpened;

    /// <summary>
    /// Occurs when the back button receives an interaction such as a click or tap.
    /// </summary>
    public event EventHandler<NavigationViewBackRequestedEventArgs> BackRequested;

    /// <summary>
    /// Occurs when the currently selected item changes.
    /// </summary>
    public event EventHandler<NavigationViewSelectionChangedEventArgs> SelectionChanged;

    /// <summary>
    /// Occurs when an item in the menu receives an interaction such a a click or tap.
    /// </summary>
    public event EventHandler<NavigationViewItemInvokedEventArgs> ItemInvoked;

    /// <summary>
    /// Occurs when the DisplayMode property changes.
    /// </summary>
    public event EventHandler<NavigationViewDisplayModeChangedEventArgs> DisplayModeChanged;

    /// <summary>
    /// Occurs when a node in the tree starts to expand.
    /// </summary>
    public event EventHandler<NavigationViewItemExpandingEventArgs> ItemExpanding;

    /// <summary>
    /// Occurs when a node in the tree is collapsed.
    /// </summary>
    public event EventHandler<NavigationViewItemCollapsedEventArgs> ItemCollapsed;

    /// <summary>
    /// Property that stores disposables to each NavigationViewItem when their created in the ItemsRepeater,
    /// so they can be disposed when the item is removed
    /// </summary>
    internal static readonly AttachedProperty<FACompositeDisposable> NavigationViewItemBaseRevokersProperty =
        AvaloniaProperty.RegisterAttached<NavigationView, NavigationViewItemBase, FACompositeDisposable>("NavigationViewItemBaseRevokers");

    private object _selectedItem;
    private IList<object> _menuItems;
    private IList<object> _footerMenuItems;
    private NavigationViewDisplayMode _displayMode = NavigationViewDisplayMode.Minimal;
    private NavigationViewItem _settingsItem;

    private const string s_tpTogglePaneButton = "TogglePaneButton";
    private const string s_tpPaneHeaderContentBorder = "PaneHeaderContentBorder";
    private const string s_tpPaneCustomContentBorder = "PaneCustomContentBorder";
    private const string s_tpFooterContentBorder = "FooterContentBorder";
    private const string s_tpPaneHeaderOnTopPane = "PaneHeaderOnTopPane";
    private const string s_tpPaneTitleOnTopPane = "PaneTitleOnTopPane";
    private const string s_tpPaneCustomContentOnTopPane = "PaneCustomContentOnTopPane";
    private const string s_tpPaneFooterOnTopPane = "PaneFooterOnTopPane";
    private const string s_tpRootSplitView = "RootSplitView";
    private const string s_tpTopNavGrid = "TopNavGrid";
    private const string s_tpMenuItemsHost = "MenuItemsHost";
    private const string s_tpTopNavMenuItemsHost = "TopNavMenuItemsHost";
    private const string s_tpTopNavMenuItemsOverflowHost = "TopNavMenuItemsOverflowHost";
    private const string s_tpTopNavOverflowButton = "TopNavOverflowButton";
    private const string s_tpFooterMenuItemsHost = "FooterMenuItemsHost";
    private const string s_tpTopFooterMenuItemsHost = "TopFooterMenuItemsHost";
    private const string s_tpTopNavContentOverlayAreaGrid = "TopNavContentOverlayAreaGrid";
    private const string s_tpPaneAutoSuggestBoxPresenter = "PaneAutoSuggestBoxPresenter";
    private const string s_tpTopPaneAutoSuggestBoxPresenter = "TopPaneAutoSuggestBoxPresenter";
    private const string s_tpPaneContentGrid = "PaneContentGrid";
    private const string s_tpContentLeftPadding = "ContentLeftPadding";
    private const string s_tpPlaceholderGrid = "PlaceholderGrid";
    private const string s_tpPaneTitleTextBlock = "PaneTitleTextBlock";
    private const string s_tpPaneTitlePresenter = "PaneTitlePresenter";
    private const string s_tpPaneTitleHolder = "PaneTitleHolder";
    private const string s_tpPaneAutoSuggestButton = "PaneAutoSuggestButton";
    private const string s_tpNavigationViewBackButton = "NavigationViewBackButton";
    private const string s_tpNavigationViewCloseButton = "NavigationViewCloseButton";
    private const string s_tpMenuItemsScrollViewer = "MenuItemsScrollViewer";
    private const string s_tpFooterItemsScrollViewer = "FooterItemsScrollViewer";
    private const string s_tpItemsContainerGrid = "ItemsContainerGrid";

    private const string s_pcListSizeCompact = ":listsizecompact";
    private const string s_pcBackButtonCollapsed = ":backbuttoncollapsed";
    private const string s_pcMinimalWithBack = ":minimalwithback";
    private const string s_pcMinimal = ":minimal";
    private const string s_pcTopNavMinimal = ":topnavminimal";
    private const string s_pcCompact = ":compact";
    private const string s_pcExpanded = ":expanded";
    private const string s_pcAutoSuggestCollapsed = ":autosuggestcollapsed";
    private const string s_pcSettingsCollapsed = ":settingscollapsed";
    private const string s_pcPaneToggleCollapsed = ":panetogglecollapsed";
    private const string s_pcPaneNotOverlaying = ":panenotoverlaying";
    private const string s_pcClosedCompact = ":closedcompact";
    private const string s_pcPaneCollapsed = ":panecollapsed";
    private const string s_pcHeaderCollapsed = ":headercollapsed";

    private const string s_resPaneToggleButtonWidth = "PaneToggleButtonWidth";
    private const string s_resPaneToggleButtonHeight = "PaneToggleButtonHeight";
   
    //Con't logic for pane arrow key navigation
    private bool VerifyInPane(Visual focus, Visual parent)
    {
        if (parent == null)
            return false;

        //First test the back button, close button, and panetogglebutton
        //since they don't reside in the content grids
        if (_backButton != null && focus == _backButton)
            return true;

        if (_closeButton != null && focus == _closeButton)
            return true;

        if (_paneToggleButton != null && focus == _paneToggleButton)
            return true;

        while (focus != null)
        {
            if (focus == parent)
                return true;

            focus = focus.GetVisualParent();
        }
        return false;
    }

    private Control SearchTreeForLowestFocusItem(NavigationViewItem start)
    {
        if (DoesNavigationViewItemHaveChildren(start) && start.IsExpanded)
        {
            var ct = start.GetRepeater.ItemsSourceView.Count;
            for (int j = ct - 1; j >= 0; j--)
            {
                if (start.GetRepeater.TryGetElement(j) is NavigationViewItem nvi)
                {
                    return SearchTreeForLowestFocusItem(nvi);
                }
            }
        }

        return start;
    }

    //Helpers

    private int SelectedItemIndex => _topDataProvider.IndexOf(SelectedItem);

    internal bool IsTopNavigationView => PaneDisplayMode == NavigationViewPaneDisplayMode.Top;

    private bool IsTopPrimaryListVisible => _topNavRepeater != null && TemplateSettings.TopPaneVisibility;

    private double GetPaneToggleButtonWidth() =>
        this.TryFindResource(s_resPaneToggleButtonWidth, out object value) ? (double)value : 40;

    private double GetPaneToggleButtonHeight() =>
        this.TryFindResource(s_resPaneToggleButtonHeight, out object value) ? (double)value : 40;

    internal bool IsOverlay => _splitView != null && _splitView.DisplayMode == SplitViewDisplayMode.Overlay;

    private bool IsLightDismissable => _splitView != null && (
        _splitView.DisplayMode != SplitViewDisplayMode.Inline &&
        _splitView.DisplayMode != SplitViewDisplayMode.CompactInline);

    internal bool ShouldShowBackButton
    {
        get
        {
            if (DisplayMode == NavigationViewDisplayMode.Minimal && IsPaneOpen)
                return false;

            return ShouldShowBackOrCloseButton;
        }
    }

    internal bool ShouldShowCloseButton
    {
        get
        {
            if (_backButton != null && _closeButton != null)
            {
                if (!IsPaneOpen)
                {
                    return false;
                }

                var pdm = PaneDisplayMode;

                if (pdm != NavigationViewPaneDisplayMode.LeftMinimal &&
                    (pdm != NavigationViewPaneDisplayMode.Auto ||
                    DisplayMode != NavigationViewDisplayMode.Minimal))
                {
                    return false;
                }

                return ShouldShowBackOrCloseButton;
            }

            return false;
        }
    }

    internal bool ShouldShowBackOrCloseButton
    {
        get
        {
            bool vis = IsBackButtonVisible;
            return vis;
        }
    }

    private bool IsTopLevelItem(NavigationViewItemBase nvib)
    {
        return IsRootItemsRepeater(GetParentItemsRepeaterForContainer(nvib));
    }


    private bool DoesNavigationViewItemHaveChildren(NavigationViewItem nvi)
    {
        var miSource = nvi?.MenuItemsSource;
        if (miSource != null)
        {
            return miSource.Count() > 0;
        }
        return nvi != null &&
            ((nvi.MenuItems != null && nvi.MenuItems.Count() > 0) || nvi.HasUnrealizedChildren);
    }

    private bool IsSelectionSuppressed(object item)
    {
        if (item != null)
        {
            return !NavigationViewItemOrSettingsContentFromData(item)?.SelectsOnInvoked ?? false;
        }

        return false;
    }

    private bool IsRootItemsRepeater(object ir)
    {
        return ir != null &&
            (ir == _topNavRepeater ||
            ir == _leftNavRepeater ||
            ir == _topNavRepeaterOverflowView ||
            ir == _leftNavFooterMenuRepeater ||
            ir == _topNavFooterMenuRepeater);
    }

    private bool IsRootGridOfFlyout(object item)
    {
        //TODO: Why do we need the root grid of the flyout?
        return item is Panel p && p.Name == "FlyoutRootGrid";
    }

    private ItemsRepeater GetParentRootItemsRepeaterForContainer(NavigationViewItemBase nvib)
    {
        var parentIR = GetParentItemsRepeaterForContainer(nvib);

        while (!IsRootItemsRepeater(parentIR))
        {
            nvib = GetParentNavigationViewItemForContainer(nvib);
            if (nvib == null)
            {
                return null;
            }

            parentIR = GetParentItemsRepeaterForContainer(nvib);
        }

        return parentIR;
    }

    private ItemsRepeater GetParentItemsRepeaterForContainer(NavigationViewItemBase nvib)
    {
        return nvib?.FindAncestorOfType<ItemsRepeater>();
    }

    private NavigationViewItem GetParentNavigationViewItemForContainer(NavigationViewItemBase nvib)
    {
        // (WinUI) TODO: This scenario does not find parent items when in a flyout, which causes problems
        // if item if first loaded straight in the flyout. Fix.This logic can be merged with the
        // 'GetIndexPathForContainer' logic below.
        var parent = GetParentItemsRepeaterForContainer(nvib);
        if (!IsRootItemsRepeater(parent))
        {
            return parent.FindAncestorOfType<NavigationViewItem>();
        }

        return null;
    }

    private IndexPath GetIndexPathForContainer(NavigationViewItemBase nvib)
    {
        var path = new List<int>(4);
        bool isInFooterMenu = false;

        Control child = nvib;
        var parent = nvib.GetVisualParent();
        if (parent == null)
        {
            return IndexPath.CreateFromIndices(path);
        }

        // Search through VisualTree for a root ItemsRepeater
        while (parent != null && !IsRootItemsRepeater(parent) && !IsRootGridOfFlyout(parent))
        {
            if (parent is ItemsRepeater ir)
            {
                path.Insert(0, ir.GetElementIndex(child));
            }
            child = (Control)parent;
            parent = parent.GetVisualParent();
        }

        // If the item is in a flyout, then we need to final index of its parent
        if (IsRootGridOfFlyout(parent))
        {
            if (_lastItemExpandedIntoFlyout != null)
            {
                child = _lastItemExpandedIntoFlyout;
                parent = IsTopNavigationView ? _topNavRepeater : _leftNavRepeater;
            }
        }

        // If item is in one of the disconnected ItemRepeaters, account for that in IndexPath calculations
        if (parent == _topNavRepeaterOverflowView)
        {
            // Convert index of selected item in overflow to index in datasource
            var contIndex = _topNavRepeaterOverflowView.GetElementIndex(child);
            var item = _topDataProvider.GetOverflowItems()[contIndex];
            var indexAtRoot = _topDataProvider.IndexOf(item);
            path.Insert(0, indexAtRoot);
        }
        else if (parent == _topNavRepeater)
        {
            // Convert index of selected item in overflow to index in datasource
            var contIndex = _topNavRepeater.GetElementIndex(child);
            var item = _topDataProvider.GetPrimaryItems()[contIndex];
            var indexAtRoot = _topDataProvider.IndexOf(item);
            path.Insert(0, indexAtRoot);
        }
        else if (parent is ItemsRepeater parentIR)
        {
            path.Insert(0, parentIR.GetElementIndex(child));
        }

        isInFooterMenu = parent == _leftNavFooterMenuRepeater || parent == _topNavFooterMenuRepeater;

        path.Insert(0, isInFooterMenu ? _footerMenuBlockIndex : _mainMenuBlockIndex);

        return IndexPath.CreateFromIndices(path);
    }


    private NavigationViewItemBase NavigationViewItemBaseOrSettingsContentFromData(object data)
        => GetContainerForData<NavigationViewItemBase>(data);

    private NavigationViewItem NavigationViewItemOrSettingsContentFromData(object data)
        => GetContainerForData<NavigationViewItem>(data);


    internal object MenuItemFromContainer(object container)
    {
        if (container is NavigationViewItemBase nvib)
        {
            var parentIR = GetParentItemsRepeaterForContainer(nvib);
            if (parentIR != null)
            {
                var contIndex = parentIR.GetElementIndex(nvib);
                if (contIndex >= 0)
                    return GetItemFromIndex(parentIR, contIndex);
            }
        }

        return null;
    }

    private Control ContainerFromMenuItem(object item)
    {
        return NavigationViewItemBaseOrSettingsContentFromData(item);
    }

    private int GetNavigationViewItemCountInPrimaryList =>
        _topDataProvider?.NavigationViewItemCountInPrimaryList ?? 0;

    private int GetNavigationViewItemCountInTopNav =>
        _topDataProvider?.NavigationViewItemCountInTopNav ?? 0;

    private bool IsSettingsItem(object item)
    {
        if (item != null && _settingsItem != null)
        {
            return (item == _settingsItem) || (_settingsItem.Content == item);
        }

        return false;
    }

    private double MeasureTopNavigationViewDesiredWidth(Size availableSize) =>
        LayoutHelper.MeasureChild(_topNavGrid, availableSize, new Thickness()).Width;

    private double MeasureTopNavMenuItemsHostDesiredWidth(Size availableSize) =>
        LayoutHelper.MeasureChild(_topNavRepeater, availableSize, new Thickness()).Width;

    private double GetTopNavigationViewActualWidth => _topNavGrid.Bounds.Width;

    private bool HasTopNavigationViewItemNotInPrimaryList() =>
        _topDataProvider.PrimaryListSize != _topDataProvider.Size;

    private void SetOverflowButtonVisibility(bool vis)
    {
        TemplateSettings.OverflowButtonVisibility = vis;
    }

    private bool NeedTopPadding() => false;//TitleBar stuff

    private int GetContainerCountInRepeater(ItemsRepeater ir)
    {
        if (ir != null && ir.ItemsSourceView != null)
        {
            return ir.ItemsSourceView.Count;
        }

        return -1;
    }

    private bool DoesRepeaterHaveRealizedContainers(ItemsRepeater ir)
    {
        return ir != null && ir.TryGetElement(0) != null;
    }

    private int GetIndexFromItem(ItemsRepeater ir, object data)
    {
        if (ir != null && ir.ItemsSourceView != null)
        {
            return ir.ItemsSourceView.IndexOf(data);
        }

        return -1;
    }

    private object GetItemFromIndex(ItemsRepeater ir, int index)
    {
        if (ir != null && ir.ItemsSourceView != null)
        {
            return ir.ItemsSourceView.GetAt(index);
        }

        return null;
    }

    private IndexPath GetIndexPathOfItem(object item)
    {
        if (item is NavigationViewItemBase nvib)
        {
            return GetIndexPathForContainer(nvib);
        }

        // In the databinding scenario, we need to conduct a search where we go through every item,
        // realizing it if necessary.
        if (IsTopNavigationView)
        {
            // First search through primary list
            var ip = SearchEntireTreeForIndexPath(_topNavRepeater, item, false);
            if (ip != IndexPath.Unselected)
            {
                return ip;
            }

            // If item was not located in primary list, search through overflow
            ip = SearchEntireTreeForIndexPath(_topNavRepeaterOverflowView, item, false);
            if (ip != IndexPath.Unselected)
            {
                return ip;
            }

            // If item was not located in primary list and overflow, search through footer
            ip = SearchEntireTreeForIndexPath(_topNavFooterMenuRepeater, item, true);
            if (ip != IndexPath.Unselected)
            {
                return ip;
            }
        }
        else
        {
            var ip = SearchEntireTreeForIndexPath(_leftNavFooterMenuRepeater, item, true);
            if (ip != IndexPath.Unselected)
            {
                return ip;
            }

            ip = SearchEntireTreeForIndexPath(_leftNavFooterMenuRepeater, item, true);
            if (ip != IndexPath.Unselected)
            {
                return ip;
            }
        }

        return IndexPath.Unselected;
    }

    private bool IsContainerTheSelectedItemInTheSelectionModel(NavigationViewItemBase nvib)
    {
        var selItem = _selectionModel.SelectedItem;

        if (selItem == null)
            return false;

        var selItemCont = selItem as NavigationViewItemBase;
        if (selItemCont == null)
        {
            selItemCont = GetContainerForIndexPath(_selectionModel.SelectedIndex);
        }

        return selItemCont == nvib;
    }

    internal NavigationViewItem GetSelectedContainer()
    {
        if (SelectedItem == null)
            return null;

        if (SelectedItem is NavigationViewItem nvi)
        {
            return nvi;
        }
        else
        {
            return NavigationViewItemOrSettingsContentFromData(SelectedItem);
        }
    }

    private IEnumerable GetChildren(NavigationViewItem nvi)
    {
        return nvi.MenuItems.Count > 0 ? nvi.MenuItems : nvi.MenuItemsSource;
    }

    private ItemsRepeater GetChildRepeaterForIndexPath(IndexPath ip)
    {
        if (GetContainerForIndexPath(ip) is NavigationViewItem nvi)
        {
            return nvi.GetRepeater;
        }

        return null;
    }

    private NavigationRecommendedTransitionDirection GetRecommendedTransitionDirection(Control prev, Control next)
    {
        var recTransDir = NavigationRecommendedTransitionDirection.Default;
        var ir = _topNavRepeater;

        if (prev != null && next != null && ir != null)
        {
            var prevIndexPath = GetIndexPathForContainer(prev as NavigationViewItemBase);
            var nextIndexPath = GetIndexPathForContainer(next as NavigationViewItemBase);

            var compare = prevIndexPath.CompareTo(nextIndexPath);

            switch (compare)
            {
                case -1:
                    recTransDir = NavigationRecommendedTransitionDirection.FromRight;
                    break;
                case 1:
                    recTransDir = NavigationRecommendedTransitionDirection.FromLeft;
                    break;
                default:
                    recTransDir = NavigationRecommendedTransitionDirection.Default;
                    break;
            }
        }

        return recTransDir;
    }

    private NavigationTransitionInfo CreateNavigationTransitionInfo(NavigationRecommendedTransitionDirection recDir)
    {
        // In current implementation, if click is from overflow item, just recommend FromRight Slide animation.
        if (recDir == NavigationRecommendedTransitionDirection.FromOverflow)
        {
            recDir = NavigationRecommendedTransitionDirection.FromRight;
        }

        if ((recDir == NavigationRecommendedTransitionDirection.FromLeft ||
            recDir == NavigationRecommendedTransitionDirection.FromRight))
        {
            return new SlideNavigationTransitionInfo
            {
                Effect = recDir == NavigationRecommendedTransitionDirection.FromRight ?
                 SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft
            };
        }
        else
        {
            return new EntranceNavigationTransitionInfo();
        }
    }

    internal NavigationViewItemsFactory ItemsFactory => _itemsFactory;

    private void UnhookEventsAndClearFields()
    {
        if (_paneToggleButton != null)
        {
            _paneToggleButton.Click -= OnPaneToggleButtonClick;
            _paneToggleButton = null;
        }

        if (_splitView != null)
        {
            _splitViewRevokers?.Dispose();
            _splitView.PaneClosed -= OnSplitViewPaneClosed;
            _splitView.PaneClosing -= OnSplitViewPaneClosing;
            _splitView.PaneOpened -= OnSplitViewPaneOpened;
            _splitView.PaneOpening -= OnSplitViewPaneOpening;
            _splitView = null;
        }

        if (_leftNavRepeater != null)
        {
            _leftNavRepeater.ElementClearing -= OnRepeaterElementClearing;
            _leftNavRepeater.ElementPrepared -= OnRepeaterElementPrepared;

            _leftNavRepeater.Loaded -= OnRepeaterLoaded;
            _leftNavRepeater.GettingFocus -= OnRepeaterGettingFocus;
            _leftNavRepeater = null;
        }

        if (_topNavRepeater != null)
        {
            _topNavRepeater.ElementClearing -= OnRepeaterElementClearing;
            _topNavRepeater.ElementPrepared -= OnRepeaterElementPrepared;

            _topNavRepeater.Loaded -= OnRepeaterLoaded;
            _topNavRepeater.GettingFocus -= OnRepeaterGettingFocus;
            _topNavRepeater = null;
        }

        if (_topNavRepeaterOverflowView != null)
        {
            _topNavRepeaterOverflowView.ElementClearing -= OnRepeaterElementClearing;
            _topNavRepeaterOverflowView.ElementPrepared -= OnRepeaterElementPrepared;

            _topNavRepeaterOverflowView = null;
        }

        if (_topNavOverflowButton != null)
        {
            var flyout = _topNavOverflowButton.Flyout as PopupFlyoutBase;
            flyout?.Closing -= OnFlyoutClosing;
        }

        if (_leftNavFooterMenuRepeater != null)
        {
            _leftNavFooterMenuRepeater.ElementClearing -= OnRepeaterElementClearing;
            _leftNavFooterMenuRepeater.ElementPrepared -= OnRepeaterElementPrepared;

            _leftNavFooterMenuRepeater.Loaded -= OnRepeaterLoaded;
            _leftNavFooterMenuRepeater.GettingFocus -= OnRepeaterGettingFocus;
            _leftNavFooterMenuRepeater = null;
        }

        if (_topNavFooterMenuRepeater != null)
        {
            _topNavFooterMenuRepeater.ElementClearing -= OnRepeaterElementClearing;
            _topNavFooterMenuRepeater.ElementPrepared -= OnRepeaterElementPrepared;

            _topNavFooterMenuRepeater.Loaded -= OnRepeaterLoaded;
            _topNavFooterMenuRepeater.GettingFocus -= OnRepeaterGettingFocus;
            _topNavFooterMenuRepeater = null;
        }

        _paneTitleHolderRevoker?.Dispose();
        _paneTitleHolderRevoker = null;

        _paneSearchButton?.Click -= OnPaneSearchButtonClick;

        _backButton?.Click -= OnBackButtonClicked;

        //titlebar?

        _closeButton?.Click -= OnPaneToggleButtonClick;

        _itemsContainerSizeRevoker?.Dispose();
        _itemsContainerSizeRevoker = null;

        _itemsContainerSizeRevoker?.Dispose();
    }

    private NavigationViewItemsFactory _itemsFactory;
    internal SplitView GetSplitView => _splitView;

    //Template Items
    private Button _paneToggleButton;
    private SplitView _splitView;
    private RowDefinition _itemsContainerRow;
    private ScrollViewer _menuItemsScrollViewer;
    private ScrollViewer _footerItemsScrollViewer;
    private Grid _paneContentGrid;
    //private ColumnDefinition _paneToggleButtonIconGridColumn;
    private Control _paneTitleHolderFrameworkElement;
    private Control _paneTitleFrameworkElement;
    //private IControl _visualItemsSeparator;
    private Button _paneSearchButton;
    private Button _backButton;
    private Button _closeButton;
    private ItemsRepeater _leftNavRepeater;
    private ItemsRepeater _topNavRepeater;
    private ItemsRepeater _leftNavFooterMenuRepeater;
    private ItemsRepeater _topNavFooterMenuRepeater;
    private Button _topNavOverflowButton;
    private ItemsRepeater _topNavRepeaterOverflowView;
    private Grid _topNavGrid;
    private Border _topNavContentOverlayAreaGrid;
    private Control _itemsContainer;

    //Indicator animations
    private Control _prevIndicator;
    private Control _nextIndicator;
    private Control _activeIndicator;
    private object _lastSelectedItemPendingAnimationInTopNav;

    //private IControl _togglePaneTopPadding;
    //private IControl _contentPaneTopPadding;
    private Control _contentLeftPadding;

    //Titlebar

    private ContentControl _leftNavAutoSuggestBoxPresenter;
    private ContentControl _topNavAutoSuggestBoxPresenter;

    private ContentControl _leftNavPaneHeaderContentBorder;
    private ContentControl _leftNavPaneCustomContentBorder;
    private ContentControl _leftNavFooterContentBorder;

    private ContentControl _paneHeaderOnTopPane;
    private ContentControl _paneTitleOnTopPane;
    private ContentControl _paneCustomContentOnTopPane;
    private ContentControl _paneFooterOnTopPane;
    private ContentControl _paneTitlePresenter;

    private ColumnDefinition _paneHeaderCloseButtonColumn;
    private ColumnDefinition _paneHeaderToggleButtonColumn;
    private RowDefinition _paneHeaderContentBorderRow;

    private NavigationViewItem _lastItemExpandedIntoFlyout;

    private IDisposable _splitViewRevokers;
    private IDisposable _sizeChangedRevoker;
    private IDisposable _paneTitleHolderRevoker;
    private IDisposable _itemsContainerSizeRevoker;

    bool _wasForceClosed;
    bool _isClosedCompact;
    bool _blockNextClosingEvent;
    //bool _initialListSizeStateSet;
    bool _isLeftPaneTitleEmpty;

    private TopNavigationViewDataProvider _topDataProvider;

    private SelectionModel _selectionModel;
    private AvaloniaList<IEnumerable> _selectionModelSource;
    private Avalonia.Controls.ItemsSourceView _menuItemsSource;
    private Avalonia.Controls.ItemsSourceView _footerItemsSource;

    //private ItemsSourceView _menuItemsSource;
    //private ItemsSourceView _footerItemsSource;

    private bool _appliedTemplate;

    // Identifies whenever a call is the result of OnApplyTemplate
    private bool _fromOnApplyTemplate;

    // Used to defer updating the SplitView displaymode property
    private bool _updateVisualStateForDisplayModeFromOnLoaded;


    // flag is used to stop recursive call. eg:
    // Customer select an item from SelectedItem property->ChangeSelection update ListView->LIstView raise OnSelectChange(we want stop here)->change property do do animation again.
    // Customer clicked listview->listview raised OnSelectChange->SelectedItem property changed->ChangeSelection->Undo the selection by SelectedItem(prevItem) (we want it stop here)->ChangeSelection again ->...
    private bool _shouldIgnoreNextSelectionChange;

    // A flag to track that the selectionchange is caused by selection a item in topnav overflow menu
    private bool _selectionChangeFromOverflowMenu;

    // Flag indicating whether selection change should raise item invoked. This is needed to be able to raise ItemInvoked before SelectionChanged while SelectedItem should point to the clicked item
    private bool _shouldRaiseItemInvokedAfterSelection;

    private TopNavigationViewLayoutState _topNavigationMode = TopNavigationViewLayoutState.Uninitialized;

    // A threshold to stop recovery from overflow to normal happens immediately on resize.
    private readonly float _topNavigationRecoveryGracePeriodWidth = 5f;

    // There are three ways to change IsPaneOpen:
    // 1, customer call IsPaneOpen=true/false directly or nav.IsPaneOpen is binding with a variable and the value is changed.
    // 2, customer click ToggleButton or splitView.IsPaneOpen->nav.IsPaneOpen changed because of window resize
    // 3, customer changed PaneDisplayMode.
    // 2 and 3 are internal implementation and will call by ClosePane/OpenPane. the flag is to indicate 1 if it's false
    private bool _isOpenPaneForInteraction;

    private bool _moveTopNavOverflowItemOnFlyoutClose;

    private bool _shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise;

    private bool _orientationChangedPendingAnimation;

    private bool _tabKeyPrecedesFocusChange;

    private bool _initialNonForcedModeUpdate = true;

    private static readonly SymbolIconSource _settingsIconSource = new SymbolIconSource { Symbol = Symbol.Settings };

    private const int _backButtonHeight = 40;
    private const int _backButtonWidth = 40;
    private const int _paneToggleButtonHeight = 40;
    private const int _paneToggleButtonWidth = 40;
    private const int _backButtonRowDefinition = 1;
    private const float paneElevationTranslationZ = 32;
    private const int c_toggleButtonHeightWithNoBackButton = 56;

    private const int _mainMenuBlockIndex = 0;
    private const int _footerMenuBlockIndex = 1;

    private const int _itemNotFound = -1;

    private double _openPaneWidth = 320; //WinUI #5800

    // Added in WinUI1.5
    private bool _isSelectionChangedPending;
    private object _pendingSelectionChangedItem;
    private NavigationRecommendedTransitionDirection _pendingSelectionChangedDirection;

    // Localization String Resources
    private const string SR_SettingsButtonName = "SettingsButtonName";
    private const string SR_NavigationOverflowButtonToolTip = "NavigationOverflowButtonToolTip";
    private const string SR_NavigationViewSearchButtonName = "NavigationViewSearchButtonName";
    private const string SR_NavigationBackButtonToolTip = "NavigationBackButtonToolTip";
    private const string SR_NavigationButtonOpenName = "NavigationButtonOpenName";
    private const string SR_NavigationButtonClosedName = "NavigationButtonClosedName";
    private const string SR_NavigationOverflowButtonName = "NavigationOverflowButtonName";
    
    public NavigationView()
    {
        //PseudoClasses.Add(":autosuggestcollapsed");
        //PseudoClasses.Add(":headercollapsed");
        //PseudoClasses.Add(":backbuttoncollapsed");
        //PseudoClasses.Add(":expanded");

        TemplateSettings = new NavigationViewTemplateSettings();

        _sizeChangedRevoker = this.GetObservable(BoundsProperty).Subscribe(OnSizeChanged);

        _selectionModelSource = new AvaloniaList<IEnumerable>(2) { null, null };
        
        _topDataProvider = new TopNavigationViewDataProvider(this);

        MenuItems = new AvaloniaList<object>();
        FooterMenuItems = new AvaloniaList<object>();

        _topDataProvider.OnRawDataChanged((args) => OnTopNavDataSourceChanged(args));

        Loaded += OnNavViewLoaded;
        // Unloaded is titlebar related - ignore

        _selectionModel = new SelectionModel()
        {
            SingleSelect = true,
            Source = _selectionModelSource
        };
        _selectionModel.SelectionChanged += OnSelectionModelSelectionChanged;
        _selectionModel.ChildrenRequested += OnSelectionModelChildrenRequested;

        _itemsFactory = new NavigationViewItemsFactory();
        
        LocalizationService.Instance.PropertyChanged += OnLanguageChanged;
    }

    /// <summary>
    /// 语言更新重新获取值
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLanguageChanged(object sender, PropertyChangedEventArgs e)
    {
        try
        {
            var tip = ToolTip.GetTip(_topNavOverflowButton);
            if (tip != null)
            {
                ToolTip.SetTip(_topNavOverflowButton, LocalizationService.Instance.GetString(SR_NavigationOverflowButtonToolTip));
            }
        
            var searchButtonName = LocalizationService.Instance.GetString(SR_NavigationViewSearchButtonName);
            AutomationProperties.SetName(_paneSearchButton, searchButtonName); 
            ToolTip.SetTip(_paneSearchButton, searchButtonName);
        
            var navigationName = LocalizationService.Instance.GetString(SR_NavigationBackButtonToolTip);
            ToolTip.SetTip(_backButton, navigationName);
            AutomationProperties.SetName(_backButton, navigationName);
        
            ToolTip.SetTip(_closeButton, LocalizationService.Instance.GetString(SR_NavigationButtonOpenName));
        
            _settingsItem.Content = IsTopNavigationView ? null : LocalizationService.Instance.GetString(SR_SettingsButtonName);
            UpdateSettingsItemToolTip();
            SetPaneToggleButtonAutomationName();
        }
        catch (Exception exception) { }
    }


    ///////////////////////////////////////
    //////// OVERRIDE METHODS ////////////
    /////////////////////////////////////

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        // Stop update anything because of PropertyChange during OnApplyTemplate. Update them all together at the end of this function
        try
        {
            _fromOnApplyTemplate = true;

            UnhookEventsAndClearFields();

            base.OnApplyTemplate(e);

            _paneToggleButton = e.NameScope.Get<Button>(s_tpTogglePaneButton);
            if (_paneToggleButton != null)
            {
                _paneToggleButton.Click += OnPaneToggleButtonClick;

                SetPaneToggleButtonAutomationName();

                //KeyboardAccelerator Win+Back
            }

            _leftNavPaneHeaderContentBorder = e.NameScope.Get<ContentControl>(s_tpPaneHeaderContentBorder);
            _leftNavPaneCustomContentBorder = e.NameScope.Get<ContentControl>(s_tpPaneCustomContentBorder);
            _leftNavFooterContentBorder = e.NameScope.Get<ContentControl>(s_tpFooterContentBorder);
            _paneHeaderOnTopPane = e.NameScope.Get<ContentControl>(s_tpPaneHeaderOnTopPane);
            _paneTitleOnTopPane = e.NameScope.Get<ContentControl>(s_tpPaneTitleOnTopPane);
            _paneCustomContentOnTopPane = e.NameScope.Get<ContentControl>(s_tpPaneCustomContentOnTopPane);
            _paneFooterOnTopPane = e.NameScope.Get<ContentControl>(s_tpPaneFooterOnTopPane);

            _splitView = e.NameScope.Get<SplitView>(s_tpRootSplitView);
            if (_splitView != null)
            {
                _splitViewRevokers = new FACompositeDisposable(
                    _splitView.GetPropertyChangedObservable(SplitView.IsPaneOpenProperty).Subscribe(OnSplitViewClosedCompactChanged),
                    _splitView.GetPropertyChangedObservable(SplitView.DisplayModeProperty).Subscribe(OnSplitViewClosedCompactChanged));

                _splitView.PaneClosed += OnSplitViewPaneClosed;
                _splitView.PaneClosing += OnSplitViewPaneClosing;
                _splitView.PaneOpened += OnSplitViewPaneOpened;
                _splitView.PaneOpening += OnSplitViewPaneOpening;

                UpdateIsClosedCompact();
            }

            _topNavGrid = e.NameScope.Get<Grid>(s_tpTopNavGrid);

            // (WinUI) Change code to NOT do this if we're in top nav mode, to prevent it from being realized:
            _leftNavRepeater = e.NameScope.Get<ItemsRepeater>(s_tpMenuItemsHost);
            if (_leftNavRepeater != null)
            {
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                (_leftNavRepeater.Layout as StackLayout).DisableVirtualization = true;

                _leftNavRepeater.ElementPrepared += OnRepeaterElementPrepared;
                _leftNavRepeater.ElementClearing += OnRepeaterElementClearing;

                _leftNavRepeater.Loaded += OnRepeaterLoaded;
                _leftNavRepeater.GettingFocus += OnRepeaterGettingFocus;

                _leftNavRepeater.ItemTemplate = _itemsFactory;
            }

            // (WinUI) Change code to NOT do this if we're in left nav mode, to prevent it from being realized:
            _topNavRepeater = e.NameScope.Get<ItemsRepeater>(s_tpTopNavMenuItemsHost);
            if (_topNavRepeater != null)
            {
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                (_topNavRepeater.Layout as StackLayout).DisableVirtualization = true;

                _topNavRepeater.ElementPrepared += OnRepeaterElementPrepared;
                _topNavRepeater.ElementClearing += OnRepeaterElementClearing;

                _topNavRepeater.Loaded += OnRepeaterLoaded;
                _topNavRepeater.GettingFocus += OnRepeaterGettingFocus;

                _topNavRepeater.ItemTemplate = _itemsFactory;
            }

            //TODO: This may not be found b/c its in the button flyout
            _topNavRepeaterOverflowView = e.NameScope.Get<ItemsRepeater>(s_tpTopNavMenuItemsOverflowHost);
            if (_topNavRepeaterOverflowView != null)
            {
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                (_topNavRepeaterOverflowView.Layout as StackLayout).DisableVirtualization = true;

                _topNavRepeaterOverflowView.ElementPrepared += OnRepeaterElementPrepared;
                _topNavRepeaterOverflowView.ElementClearing += OnRepeaterElementClearing;

                _topNavRepeater.ItemTemplate = _itemsFactory;
            }

            _topNavOverflowButton = e.NameScope.Get<Button>(s_tpTopNavOverflowButton);
            if (_topNavOverflowButton != null)
            {
                // Newest style doesn't have content, only an icon, so we'll skip setting that here like WinUI
                // TODO: Automation
                var flyout = _topNavOverflowButton.Flyout as PopupFlyoutBase;
                flyout?.Closing += OnFlyoutClosing;

                var tip = ToolTip.GetTip(_topNavOverflowButton);
                if (tip != null)
                {
                    ToolTip.SetTip(_topNavOverflowButton, LocalizationService.Instance.GetString(SR_NavigationOverflowButtonToolTip));
                }
            }

            // Change code to NOT do this if we're in top nav mode, to prevent it from being realized:
            _leftNavFooterMenuRepeater = e.NameScope.Get<ItemsRepeater>(s_tpFooterMenuItemsHost);
            if (_leftNavFooterMenuRepeater != null)
            {
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                (_leftNavFooterMenuRepeater.Layout as StackLayout).DisableVirtualization = true;

                _leftNavFooterMenuRepeater.ElementPrepared += OnRepeaterElementPrepared;
                _leftNavFooterMenuRepeater.ElementClearing += OnRepeaterElementClearing;

                _leftNavFooterMenuRepeater.Loaded += OnRepeaterLoaded;
                _leftNavFooterMenuRepeater.GettingFocus += OnRepeaterGettingFocus;

                _leftNavFooterMenuRepeater.ItemTemplate = _itemsFactory;
            }

            // Change code to NOT do this if we're in left nav mode, to prevent it from being realized:
            _topNavFooterMenuRepeater = e.NameScope.Get<ItemsRepeater>(s_tpTopFooterMenuItemsHost);
            if (_topNavFooterMenuRepeater != null)
            {
                // Disabling virtualization for now because of https://github.com/microsoft/microsoft-ui-xaml/issues/2095
                (_topNavFooterMenuRepeater.Layout as StackLayout).DisableVirtualization = true;

                _topNavFooterMenuRepeater.ElementPrepared += OnRepeaterElementPrepared;
                _topNavFooterMenuRepeater.ElementClearing += OnRepeaterElementClearing;

                _topNavFooterMenuRepeater.Loaded += OnRepeaterLoaded;
                _topNavFooterMenuRepeater.GettingFocus += OnRepeaterGettingFocus;

                _topNavFooterMenuRepeater.ItemTemplate = _itemsFactory;
            }

            _topNavContentOverlayAreaGrid = e.NameScope.Get<Border>(s_tpTopNavContentOverlayAreaGrid);
            _leftNavAutoSuggestBoxPresenter = e.NameScope.Get<ContentControl>(s_tpPaneAutoSuggestBoxPresenter);
            _topNavAutoSuggestBoxPresenter = e.NameScope.Get<ContentControl>(s_tpTopPaneAutoSuggestBoxPresenter);

            _paneContentGrid = e.NameScope.Get<Grid>(s_tpPaneContentGrid);

            _contentLeftPadding = e.NameScope.Get<Rectangle>(s_tpContentLeftPadding);

            var placeholderGrid = e.NameScope.Get<Grid>(s_tpPlaceholderGrid);
            if (placeholderGrid != null)
            {
                _paneHeaderCloseButtonColumn = placeholderGrid.ColumnDefinitions[0];
                _paneHeaderToggleButtonColumn = placeholderGrid.ColumnDefinitions[1];
                _paneHeaderContentBorderRow = placeholderGrid.RowDefinitions[0];
            }

            _paneTitleFrameworkElement = e.NameScope.Get<Control>(s_tpPaneTitleTextBlock);
            _paneTitlePresenter = e.NameScope.Get<ContentControl>(s_tpPaneTitlePresenter);

            _paneTitleHolderFrameworkElement = e.NameScope.Get<Control>(s_tpPaneTitleHolder);
            if (_paneTitleHolderFrameworkElement != null)
            {
                _paneTitleHolderRevoker = _paneTitleHolderFrameworkElement.GetObservable(BoundsProperty).Subscribe(OnPaneTitleHolderSizeChanged);
            }

            _paneSearchButton = e.NameScope.Get<Button>(s_tpPaneAutoSuggestButton);
            if (_paneSearchButton != null)
            {
                _paneSearchButton.Click += OnPaneSearchButtonClick;

                var searchButtonName = LocalizationService.Instance.GetString(SR_NavigationViewSearchButtonName);
                AutomationProperties.SetName(_paneSearchButton, searchButtonName);
                ToolTip.SetTip(_paneSearchButton, searchButtonName);
            }

            _backButton = e.NameScope.Get<Button>(s_tpNavigationViewBackButton);
            if (_backButton != null)
            {
                _backButton.Click += OnBackButtonClicked;
                var navigationName = LocalizationService.Instance.GetString(SR_NavigationBackButtonToolTip);
                ToolTip.SetTip(_backButton, navigationName);
                AutomationProperties.SetName(_backButton, navigationName);
            }

            //titlebar

            _closeButton = e.NameScope.Get<Button>(s_tpNavigationViewCloseButton);
            if (_closeButton != null)
            {
                _closeButton.Click += OnPaneToggleButtonClick;

                ToolTip.SetTip(_closeButton, LocalizationService.Instance.GetString(SR_NavigationButtonOpenName));
            }

            if (_paneContentGrid != null)
            {
                _itemsContainerRow = _paneContentGrid.RowDefinitions[_paneContentGrid.RowDefinitions.Count - 1];
            }

            _menuItemsScrollViewer = e.NameScope.Get<ScrollViewer>(s_tpMenuItemsScrollViewer);
            _footerItemsScrollViewer = e.NameScope.Get<ScrollViewer>(s_tpFooterItemsScrollViewer);

            _itemsContainer = e.NameScope.Find<Control>(s_tpItemsContainerGrid);
            if (_itemsContainerRow != null)
            {
                _itemsContainerSizeRevoker = _itemsContainer.GetObservable(BoundsProperty).Subscribe(OnItemsContainerSizeChanged);
            }

            UpdatePaneShadow();

            _appliedTemplate = true;

            // Do initial setup
            UpdatePaneDisplayMode();
            UpdateHeaderVisibility();
            UpdatePaneTitleFrameworkElementParents();
            UpdateTitleBarPadding();
            UpdatePaneTabFocusNavigation();
            UpdateBackAndCloseButtonsVisibility();
            //UpdateSingleSelectionFollowsFocusTemplateSetting();
            UpdatePaneVisibility();
            UpdateVisualState();
            // UpdatePaneTitleMargins();
            UpdatePaneLayout();
            UpdatePaneOverlayGroup();

            //There's a slight difference in the way we have to do things vs WinUI
            //This gets called from UpdatePaneDisplayMode(), but with arg = False
            //So call again to force the selectionModelSource to get correct refs
            UpdateRepeaterItemsSource(true);
            UpdateFooterRepeaterItemsSource(true, true);
        }
        finally
        {
            _fromOnApplyTemplate = false;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (IsTopNavigationView && IsTopPrimaryListVisible)
        {
            if (double.IsInfinity(availableSize.Width))
            {
                // We have infinite space, so move all items to primary list
                _topDataProvider.MoveAllItemsToPrimaryList();
            }
            else
            {
                HandleTopNavigationMeasureOverride(availableSize);
#if DEBUG
                if (_topDataProvider.Size > 0)
                {
                    Debug.Assert(_topDataProvider.GetPrimaryItems().Count > 0);
                }
#endif
            }
        }

        this.LayoutUpdated += OnLayoutUpdated;

        return base.MeasureOverride(availableSize);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CompactModeThresholdWidthProperty ||
            change.Property == ExpandedModeThresholdWidthProperty)
        {
            UpdateAdaptiveLayout(Bounds.Width);
        }
        else if (change.Property == AlwaysShowHeaderProperty ||
            change.Property == HeaderProperty)
        {
            UpdateHeaderVisibility();
        }
        else if (change.Property == PaneTitleProperty)
        {
            UpdatePaneTitleFrameworkElementParents();
            UpdateBackAndCloseButtonsVisibility();
            UpdatePaneToggleSize();
        }
        else if (change.Property == PaneDisplayModeProperty)
        {
            // m_wasForceClosed is set to true because ToggleButton is clicked and Pane is closed.
            // When PaneDisplayMode is changed, reset the force flag to make the Pane can be opened automatically again.
            _wasForceClosed = false;

            var (oldValue, newValue) = change.GetOldAndNewValue<NavigationViewPaneDisplayMode>();

            UpdatePaneToggleButtonVisibility();
            UpdatePaneDisplayMode(oldValue, newValue);
            UpdatePaneTitleFrameworkElementParents();
            UpdatePaneVisibility();
            UpdateVisualState();
            UpdatePaneButtonWidths();
        }
        else if (change.Property == IsPaneVisibleProperty)
        {
            UpdatePaneVisibility();
            UpdateVisualStateForDisplayModeGroup(DisplayMode);

            // When NavView is in expaneded mode with fixed window size, setting IsPaneVisible to false doesn't closes the pane
            // We manually close/open it for this case
            if (!IsPaneVisible && IsPaneOpen)
            {
                ClosePane();
            }

            if (IsPaneVisible && DisplayMode == NavigationViewDisplayMode.Expanded && !IsPaneOpen)
            {
                OpenPane();
            }
        }
        else if (change.Property == AutoCompleteBoxProperty)
        {
            InvalidateTopNavPrimaryLayout();

            if (change.NewValue != null)
            {
                //TODO: WinUI has SuggestionChosen event handler here, find compatible event...
            }

            UpdateVisualState();
        }
        //else if (change.Property == SelectionFollowsFocusProperty)
        //{
        //    //[IGNORE - Stupid this needs to go in template settings...]
        //}
        else if (change.Property == IsPaneToggleButtonVisibleProperty)
        {
            UpdatePaneTitleFrameworkElementParents();
            UpdateBackAndCloseButtonsVisibility();
            UpdatePaneToggleButtonVisibility();
            UpdateVisualState();
        }
        else if (change.Property == IsSettingsVisibleProperty)
        {
            UpdateFooterRepeaterItemsSource(false, true);
        }
        else if (change.Property == CompactPaneLengthProperty)
        {
            // Need to update receiver margins when CompactPaneLength changes
            //UpdatePaneShadow(); TODO

            // Update pane-button-grid width when pane is closed and we are not in minimal
            UpdatePaneButtonWidths();
        }
        //Skip IsTitleBarAutoPaddingEnabledProperty
        else if (change.Property == MenuItemTemplateProperty ||
            change.Property == MenuItemTemplateSelectorProperty)
        {
            //SyncItemTemplates(); [This just calls UpdateNavigationViewItemsFactory, so why not just do that]
            UpdateNavigationViewItemsFactory();
        }
        else if (change.Property == PaneFooterProperty)
        {
            UpdatePaneLayout();
        }
        else if (change.Property == SelectedItemProperty)
        {
            OnSelectedItemPropertyChanged(change.OldValue, change.NewValue);
        }
        else if (change.Property == IsBackButtonVisibleProperty)
        {
            UpdateBackAndCloseButtonsVisibility();
            UpdateAdaptiveLayout(Bounds.Width);
            if (IsTopNavigationView)
            {
                InvalidateTopNavPrimaryLayout();
            }

            // Enabling back button shifts grid instead of resizing, so let's update the layout.
            if (_backButton != null)
            {
                //Don't have update layout, so
                _backButton.InvalidateMeasure();
            }
            UpdatePaneLayout();
        }
        else if (change.Property == MenuItemsSourceProperty)
        {
            UpdateRepeaterItemsSource(true /*forceSelectionModelUpdate*/);
        }
        else if (change.Property == MenuItemsProperty)
        {
            UpdateRepeaterItemsSource(true /*forceSelectionModelUpdate*/);
        }
        else if (change.Property == FooterMenuItemsSourceProperty)
        {
            UpdateFooterRepeaterItemsSource(true /*sourceCollectionReset*/, true /*sourceCollectionChanged*/);
        }
        else if (change.Property == FooterMenuItemsProperty)
        {
            UpdateFooterRepeaterItemsSource(true /*sourceCollectionReset*/, true /*sourceCollectionChanged*/);
        }
        else if (change.Property == IsPaneOpenProperty)
        {
            OnIsPaneOpenChanged();
            UpdateVisualStateForDisplayModeGroup(_displayMode);
        }
        else if (change.Property == OpenPaneLengthProperty)
        {
            UpdateOpenPaneWidth(Bounds.Width);
        }
    }

    //WinUI also uses PreviewKeyDown to reset m_TabKeyPrecedesFocusChange
    protected override void OnKeyDown(KeyEventArgs e)
    {
        _tabKeyPrecedesFocusChange = false;
        switch (e.Key)
        {
            case Key.Back:
                if (IsPaneOpen && IsLightDismissable)
                {
                    e.Handled = AttemptClosePaneLightly();
                }
                break;

            case Key.Tab:
                // arrow keys navigation through ItemsRepeater don't get here
                // so handle tab key to distinguish between tab focus and arrow focus navigation
                _tabKeyPrecedesFocusChange = true;
                break;

            case Key.Left:
                if (((e.KeyModifiers & KeyModifiers.Alt) == KeyModifiers.Alt) && IsPaneOpen && IsLightDismissable)
                {
                    e.Handled = AttemptClosePaneLightly();
                    
                }
                break;
        }

        base.OnKeyDown(e);
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter.Name == "ContentPresenter")
            return true;

        return base.RegisterContentPresenter(presenter);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new NavigationViewAutomationPeer(this);
    }

    private void OnLayoutUpdated(object sender, EventArgs e)
    {
        // We only need to handle once after MeasureOverride, so revoke the token.
        this.LayoutUpdated -= OnLayoutUpdated;

        // In topnav, when an item in overflow menu is clicked, the animation is delayed because that item is not move to primary list yet.
        // And it depends on LayoutUpdated to re-play the animation. m_lastSelectedItemPendingAnimationInTopNav is the last selected overflow item.
        if (_lastSelectedItemPendingAnimationInTopNav != null)
        {
            var lastItem = _lastSelectedItemPendingAnimationInTopNav;
            _lastSelectedItemPendingAnimationInTopNav = null;
            AnimateSelectionChanged(lastItem);
        }

        if (_orientationChangedPendingAnimation)
        {
            _orientationChangedPendingAnimation = false;
            AnimateSelectionChanged(SelectedItem);
        }
    }

    private void OnNavViewLoaded(object sender, RoutedEventArgs e)
    {
        if (_updateVisualStateForDisplayModeFromOnLoaded)
        {
            _updateVisualStateForDisplayModeFromOnLoaded = false;
            UpdateVisualStateForDisplayModeGroup(DisplayMode);
        }

        //titlebar

        // Update pane buttons now since we the CompactPaneLength is actually known now.
        UpdatePaneButtonWidths();
    }



    /////////////////////////////////////////////
    //////// ITEMS REPEATER RELATED ////////////
    ///////////////////////////////////////////

    private void OnRepeaterLoaded(object sender, RoutedEventArgs args)
    {
        var item = SelectedItem;
        if (item != null && !IsSelectionSuppressed(item))
        {
            if (!IsSelectionSuppressed(item))
            {
                var nvi = NavigationViewItemOrSettingsContentFromData(item);
                nvi.IsSelected = true;

                // Make sure the SelectionModel m_selectionModel and actual selection are in sync. An item may have been selected
                // while the ItemsRepeater was still unloaded. Thus m_selectionModel still does not know about that selection.
                UpdateSelectionModelSelectionForSelectedItem(item);
            }
            
            AnimateSelectionChanged(item);
        }
    }

    private void UpdateRepeaterItemsSource(bool forceSelectionModelUpdate)
    {
        IEnumerable itemsSource;
        var miSource = MenuItemsSource;
        if (miSource != null)
        {
            itemsSource = miSource;
        }
        else
        {
            UpdateSelectionForMenuItems();
            itemsSource = _menuItems;
        }

        if (forceSelectionModelUpdate)
        {
            _selectionModelSource[0] = itemsSource;
        }

        _menuItemsSource?.CollectionChanged -= OnMenuItemsSourceCollectionChanged;

        if (itemsSource != null)
        {
            _menuItemsSource = Avalonia.Controls.ItemsSourceView.GetOrCreate(itemsSource);
            _menuItemsSource.CollectionChanged += OnMenuItemsSourceCollectionChanged;
        }

        if (IsTopNavigationView)
        {
            UpdateLeftRepeaterItemSource(null);
            UpdateTopNavRepeatersItemSource(itemsSource);
            InvalidateTopNavPrimaryLayout();
        }
        else
        {
            UpdateTopNavRepeatersItemSource(null);
            UpdateLeftRepeaterItemSource(itemsSource);
        }
    }

    private void UpdateLeftRepeaterItemSource(IEnumerable items)
    {
        UpdateItemsRepeaterItemsSource(_leftNavRepeater, items);
        UpdatePaneLayout();
    }

    private void UpdateTopNavRepeatersItemSource(IEnumerable items)
    {
        _topDataProvider.SetDataSource(items);

        UpdateTopNavPrimaryRepeaterItemsSource(items);
        UpdateTopNavOverflowRepeaterItemsSource(items);
    }

    private void UpdateTopNavPrimaryRepeaterItemsSource(IEnumerable items)
    {
        if (items != null)
        {
            UpdateItemsRepeaterItemsSource(_topNavRepeater, _topDataProvider.GetPrimaryItems());
        }
        else
        {
            UpdateItemsRepeaterItemsSource(_topNavRepeater, null);
        }
    }

    private void UpdateTopNavOverflowRepeaterItemsSource(IEnumerable items)
    {
        //_topNavOverflowRevoker
        if (_topNavRepeaterOverflowView != null)
        {
            if (_topNavRepeaterOverflowView.ItemsSourceView != null)
            {
                _topNavRepeaterOverflowView.ItemsSourceView.CollectionChanged -= OnOverflowItemsSourceCollectionChanged;
            }

            if (items != null)
            {
                var itemsSource = _topDataProvider.GetOverflowItems();
                _topNavRepeaterOverflowView.ItemsSource = itemsSource;

                // We listen to changes to the overflow menu item collection so we can set the visibility of the overflow button
                // to collapsed when it no longer has any items.
                //
                // Normally, MeasureOverride() kicks off updating the button's visibility, however, it is not run when the overflow menu
                // only contains a *single* item and we
                // - either remove that menu item or
                // - remove menu items displayed in the NavigationView pane until there is enough room for the single overflow menu item
                //   to be displayed in the pane
                if (_topNavRepeater.ItemsSourceView != null)
                {
                    _topNavRepeaterOverflowView.ItemsSourceView.CollectionChanged += OnOverflowItemsSourceCollectionChanged;
                }
            }
            else
            {
                _topNavRepeaterOverflowView.ItemsSource = null;
            }
        }
    }

    private void UpdateItemsRepeaterItemsSource(ItemsRepeater ir, IEnumerable source)
    {
        if (ir != null)
        {
            ir.ItemsSource = source;
        }
    }

    private void UpdateFooterRepeaterItemsSource(bool sourceCollectionReset, bool sourceCollectionChanged)
    {
        if (!_appliedTemplate)
            return;

        IEnumerable itemsSource;
        var fmiSource = FooterMenuItemsSource;
        if (fmiSource != null)
        {
            itemsSource = fmiSource;
        }
        else
        {
            UpdateSelectionForMenuItems();
            itemsSource = _footerMenuItems;
        }

        UpdateItemsRepeaterItemsSource(_leftNavFooterMenuRepeater, null);
        UpdateItemsRepeaterItemsSource(_topNavFooterMenuRepeater, null);

        if (_settingsItem == null || sourceCollectionChanged || sourceCollectionReset)
        {
            var dataSource = new List<object>(itemsSource.Count() + 1);
            if (_settingsItem == null)
            {
                var si = new NavigationViewItem();
                si.Name = "SettingsItem";
                _itemsFactory.SettingsItem = si;
                _settingsItem = si;
            }

            //We don't need to do this (already have IEnumerable)
            if (sourceCollectionReset)
            {
                if (_footerItemsSource != null)
                {
                    _footerItemsSource.CollectionChanged -= OnFooterItemsSourceCollectionChanged;
                    _footerItemsSource = null;
                }
            }

            if (_footerItemsSource == null)
            {
                _footerItemsSource = Avalonia.Controls.ItemsSourceView.GetOrCreate(itemsSource);
                _footerItemsSource.CollectionChanged += OnFooterItemsSourceCollectionChanged;
            }

            if (_footerItemsSource != null)
            {
                var size = _footerItemsSource.Count;

                for (int i = 0; i < size; i++)
                {
                    dataSource.Add(_footerItemsSource.GetAt(i));
                }

                if (IsSettingsVisible)
                {
                    CreateAndHookEventsToSettings();
                    // add settings item to the end of footer
                    dataSource.Add(_settingsItem);
                }
            }

            _selectionModelSource[1] = dataSource;
        }

        if (IsTopNavigationView)
        {
            UpdateItemsRepeaterItemsSource(_topNavFooterMenuRepeater, _selectionModelSource[1] as IEnumerable);
        }
        else
        {
            if (_leftNavFooterMenuRepeater != null)
            {
                UpdateItemsRepeaterItemsSource(_leftNavFooterMenuRepeater, _selectionModelSource[1] as IEnumerable);

                // Footer items changed and we need to recalculate the layout.
                // However repeater "lags" behind, so we need to force it to reevaluate itself now.
                _leftNavFooterMenuRepeater.InvalidateMeasure();
                _leftNavFooterMenuRepeater.InvalidateArrange();
                //_leftNavFooterMenuRepeater.UpdateLayout();

                // Footer items changed, so let's update the pane layout
                UpdatePaneLayout();
            }

            _settingsItem?.BringIntoView();
        }
    }

    internal void OnRepeaterElementPrepared(object sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        if (args.Element is NavigationViewItemBase nvib)
        {
            nvib.SetNavigationViewParent(this);
            nvib.IsTopLevelItem = IsTopLevelItem(nvib);
            nvib.IsInNavigationViewOwnedRepeater = true;
            NavigationViewRepeaterPosition position(ItemsRepeater ir)
            {
                if (IsTopNavigationView)
                {
                    if (ir == _topNavRepeater)
                        return NavigationViewRepeaterPosition.TopPrimary;

                    if (ir == _topNavFooterMenuRepeater)
                        return NavigationViewRepeaterPosition.TopFooter;

                    return NavigationViewRepeaterPosition.TopOverflow;
                }

                if (ir == _leftNavFooterMenuRepeater)
                {
                    return NavigationViewRepeaterPosition.LeftFooter;
                }

                return NavigationViewRepeaterPosition.LeftNav;
            }

            nvib.Position = position(sender as ItemsRepeater);

            var parentNVI = GetParentNavigationViewItemForContainer(nvib);
            if (parentNVI != null)
            {
                nvib.Depth = parentNVI.ShouldRepeaterShowInFlyout ? 0 : parentNVI.Depth + 1;
            }
            else
            {
                nvib.Depth = 0;
            }

            // Apply any custom container styling
            ApplyCustomMenuItemContainerStyling(nvib, (sender as ItemsRepeater), args.Index);

            SetNavigationViewItemBaseRevokers(nvib);

            if (args.Element is NavigationViewItem nvi)
            {
                var childDepth = nvib.Position == NavigationViewRepeaterPosition.TopPrimary ? 0 : nvib.Depth + 1;

                nvi.PropagateDepthToChildren(childDepth);

                SetNavigationViewItemRevokers(nvi);

                var item = MenuItemFromContainer(nvi);
                if (SelectedItem == item && nvi.IsEffectivelyVisible)
                {
                    if (_isSelectionChangedPending && _pendingSelectionChangedItem != null)
                    {
                        Debug.Assert(_pendingSelectionChangedItem == item);
                    }

                    nvi.LayoutUpdated += OnSelectedItemLayoutUpdated;
                }
            }
        }
    }

    private void ApplyCustomMenuItemContainerStyling(NavigationViewItemBase item, ItemsRepeater ir, int index)
    {
        var theme = MenuItemContainerTheme;
        item.Theme = theme;
    }

    internal void OnRepeaterElementClearing(object sender, ItemsRepeaterElementClearingEventArgs args)
    {
        if (args.Element is NavigationViewItemBase nvib)
        {
            nvib.Depth = 0;
            nvib.IsTopLevelItem = false;
            nvib.IsInNavigationViewOwnedRepeater = false;
            ClearNavigationViewItemBaseRevokers(nvib);

            if (nvib is NavigationViewItem nvi)
            {
                nvi.Tapped -= OnNavigationViewItemTapped;
                nvi.KeyDown -= OnNavigationViewItemKeyDown;
                nvi.GotFocus -= OnNavigationViewItemGotFocus;
                var rev = GetValue(NavigationViewItemBaseRevokersProperty);
                rev?.Dispose();
                SetValue(NavigationViewItemBaseRevokersProperty, null);
            }
        }
    }

    private void OnRepeaterGettingFocus(object sender, FocusChangingEventArgs e)
    {
        // if focus change was invoked by tab key
        // and there is selected item in ItemsRepeater that gatting focus
        // we should put focus on selected item
        if (_tabKeyPrecedesFocusChange && _selectionModel.SelectedIndex != IndexPath.Unselected &&
            (e.NavigationMethod == NavigationMethod.Directional || e.NavigationMethod == NavigationMethod.Tab))
        {
            // Note: we can't implement yet (still in v3/Av12) because FocusChangingEventArgs doesn't have a Direction
            // property like WinUI does, which is part of this code
            if (e.OldFocusedElement is Control c)
            {
                if (sender is ItemsRepeater newRootItemsRepeater)
                {
                    // f(x) - isFocusOutSideCurrentRootRepeater
                    // f(x) - rootRepeaterForSelectedItem

                    // If focus is coming from outside the root repeater,
                    // and selected item is within current repeater
                    // we should put focus on selected item
                    //if (newRootItemsRepeater == rootRepeaterForSelectedItem && isFocuseOutsideCurrentRootRepeater)
                    //{
                    //    var selectedContainer = GetContainerForIndexPath(_selectionModel.SelectedIndex, true);
                    //    if (e.TrySetNewFocusedElement(selectedContainer))
                    //    {
                    //        e.Handled = true;
                    //    }
                    //}
                    //else if (!isFocusOutsideCurrentRootRepeater)
                    //{
                        
                    //}
                }
            }
        }

        _tabKeyPrecedesFocusChange = false;
    }

    private void UpdateNavigationViewItemsFactory()
    {
        if (MenuItemTemplate == null)
        {
            _itemsFactory.UserElementFactory(MenuItemTemplateSelector);
        }
        else
        {
            _itemsFactory.UserElementFactory(MenuItemTemplate);
        }
    }





    ////////////////////////////////////////////////
    //////// PROPERTY CHANGED HANDLERS ////////////
    //////////////////////////////////////////////

    private void OnMenuItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!IsTopNavigationView)
        {
            //call update layout on LeftNavRepeater...just call invalidate measure?
            // TOOD
            _leftNavRepeater?.InvalidateMeasure();
            UpdatePaneLayout();
        }
    }

    private void OnFooterItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateFooterRepeaterItemsSource(false /*sourceCollectionReset*/, true /*sourceCollectionChanged*/);

        // Pane footer items changed. This means we might need to reevaluate the pane layout.
        UpdatePaneLayout();
    }

    private void OnOverflowItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        if (_topNavRepeaterOverflowView != null && _topNavRepeaterOverflowView.ItemsSourceView != null &&
            _topNavRepeaterOverflowView.ItemsSourceView.Count == 0)
        {
            SetOverflowButtonVisibility(false);
        }
    }

    private void OnSizeChanged(Rect r)
    {
        UpdateOpenPaneWidth(r.Width);
        UpdateAdaptiveLayout(r.Width);
        UpdateTitleBarPadding();
        UpdateBackAndCloseButtonsVisibility();
        UpdatePaneLayout();
    }

    private void OnItemsContainerSizeChanged(Rect rc)
    {
        UpdatePaneLayout();
    }

    private void OnTopNavDataSourceChanged(NotifyCollectionChangedEventArgs args)
    {
        CloseTopNavigationViewFlyout();

        // Assume that raw data doesn't change very often for navigationview.
        // So here is a simple implementation and for each data item change, it request a layout change
        // update this in the future if there is performance problem

        // If it's Uninitialized, it means that we didn't start the layout yet.

        if (_topNavigationMode != TopNavigationViewLayoutState.Uninitialized)
        {
            _topDataProvider.MoveAllItemsToPrimaryList();
        }

        _lastSelectedItemPendingAnimationInTopNav = null;
    }

    private void OnSelectedItemPropertyChanged(object oldItem, object newItem)
    {
        //Changed From AvaloniaPropertyChangedEventArgs to old,new            

        ChangeSelection(oldItem, newItem);

        if (_appliedTemplate && IsTopNavigationView)
        {
            //Also checks for LayoutUpdatedToken == null
            if (newItem != null && _topDataProvider.IndexOf(newItem) != _itemNotFound &&
                _topDataProvider.IndexOf(newItem, NavigationViewSplitVectorID.PrimaryList) == _itemNotFound) // selection is in overflow
            {
                InvalidateTopNavPrimaryLayout();
            }
        }
    }

    private void OnIsPaneOpenChanged()
    {
        bool isOpen = IsPaneOpen;
        if (isOpen && _wasForceClosed)
        {
            _wasForceClosed = false; // remove the pane open flag since Pane is opened.
        }
        else if (!_isOpenPaneForInteraction && !isOpen)
        {
            if (_splitView != null)
            {
                // splitview.IsPaneOpen and nav.IsPaneOpen is two way binding. If nav.IsPaneOpen=false and splitView.IsPaneOpen=true,
                // then the pane has been closed by API and we treat it as a forced close.
                // If, however, splitView.IsPaneOpen=false, then nav.IsPaneOpen is just following the SplitView here and the pane
                // was closed, for example, due to app window resizing. We don't set the force flag in this situation.
                _wasForceClosed = _splitView.IsPaneOpen;
            }
            else
            {
                // If there is no SplitView (for example it hasn't been loaded yet) then nav.IsPaneOpen was set directly
                // so we treat it as a closed force.
                _wasForceClosed = true;
            }
        }

        SetPaneToggleButtonAutomationName();
        UpdatePaneTabFocusNavigation();
        UpdateSettingsItemToolTip();
        UpdatePaneTitleFrameworkElementParents();
        UpdatePaneOverlayGroup();
        UpdatePaneButtonWidths();
    }









    //////////////////////////////////////////////////////////
    //////// SELECTION & SELECTION MODEL RELATED ////////////
    ////////////////////////////////////////////////////////

    private void OnSelectionModelChildrenRequested(object sender, SelectionModelChildrenRequestedEventArgs e)
    {
        //TODO Rewrite SelectionModel as it is in WinUI to get rid of Observables...

        // this is main menu or footer
        if (e.SourceIndex.GetSize() == 1)
        {
            e.Children = e.Source;
        }
        else if (e.Source is NavigationViewItem nvi)
        {
            e.Children = GetChildren(nvi);
        }
        else
        {
            var children = GetChildrenForItemInIndexPath(e.SourceIndex, true);
            if (children != null)
            {
                e.Children = children;
            }
        }
    }

    private void OnSelectionModelSelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
    {
        var selItem = _selectionModel.SelectedItem;

        // Ignore this callback if:
        // 1. the SelectedItem property of NavigationView is already set to the item
        //    being passed in this callback. This is because the item has already been selected
        //    via API and we are just updating the m_selectionModel state to accurately reflect the new selection.
        // 2. Template has not been applied yet. SelectionModel's selectedIndex state will get properly updated
        //    after the repeater finishes loading.
        // TODO: Update SelectedItem comparison to work for the exact same item datasource scenario
        if (_shouldIgnoreNextSelectionChange || selItem == SelectedItem || !_appliedTemplate)
        {
            return;
        }

        bool setSelectedItem = true;
        var selIndex = _selectionModel.SelectedIndex;

        if (IsTopNavigationView)
        {
            // If selectedIndex does not exist, means item is being deselected through API
            bool isInOverflow = (selIndex != IndexPath.Unselected && selIndex.GetSize() > 1)
                ? selIndex.GetAt(0) == _mainMenuBlockIndex && !_topDataProvider.IsItemInPrimaryList(selIndex.GetAt(1))
                : false;

            if (isInOverflow)
            {
                // We only want to close the overflow flyout and move the item on selection if it is a leaf node
                bool itemShouldBeMoved(IndexPath p)
                {
                    var selContainer = GetContainerForIndexPath(selIndex);
                    if (selContainer is NavigationViewItem nvi && DoesNavigationViewItemHaveChildren(nvi))
                    {
                        return false;
                    }

                    return true;
                }

                if (itemShouldBeMoved(selIndex))
                {
                    SelectAndMoveOverflowItem(selItem, selIndex, true /*close flyout*/);
                    setSelectedItem = false;
                }
                else
                {
                    _moveTopNavOverflowItemOnFlyoutClose = true;
                }
            }
        }

        if (setSelectedItem)
        {
            SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(selItem);
        }
    }

    private void SelectAndMoveOverflowItem(object selItem, IndexPath selIndex, bool closeFlyout)
    {
        try
        {
            _selectionChangeFromOverflowMenu = true;

            if (closeFlyout)
            {
                CloseTopNavigationViewFlyout();
            }

            if (!IsSelectionSuppressed(selItem))
            {
                SelectOverflowItem(selItem, selIndex);
            }
        }
        finally
        {
            _selectionChangeFromOverflowMenu = false;
        }
    }

    // We only need to close the flyout if the selected item is a leaf node
    private void CloseFlyoutIfRequired(NavigationViewItem selItem)
    {
        var selIndex = _selectionModel.SelectedIndex;

        bool isInModeWithFlyout = false;
        if (_splitView != null)
        {
            var svdm = _splitView.DisplayMode;
            isInModeWithFlyout = (!_splitView.IsPaneOpen && (svdm == SplitViewDisplayMode.CompactOverlay || svdm == SplitViewDisplayMode.CompactInline)) ||
                PaneDisplayMode == NavigationViewPaneDisplayMode.Top;
        }

        if (isInModeWithFlyout && selIndex != IndexPath.Unselected && !DoesNavigationViewItemHaveChildren(selItem))
        {
            // Item selected is a leaf node, find top level parent and close flyout
            var rootItem = GetContainerForIndex(selIndex.GetAt(1), selIndex.GetAt(0) == _footerMenuBlockIndex /* inFooter */);
            if (rootItem is NavigationViewItem nvi && nvi.ShouldRepeaterShowInFlyout)
            {
                nvi.IsExpanded = false;
            }
        }
    }

    private void RaiseSelectionChangedEvent(object nextItem, bool isSettings, NavigationRecommendedTransitionDirection recDir)
    {
        NavigationViewItemBase container = null;
        if (nextItem != null)
        {
            if (NavigationViewItemBaseOrSettingsContentFromData(nextItem) is NavigationViewItemBase b)
            {
                container = b;
            }
            else if (GetContainerForIndexPath(_selectionModel.SelectedIndex, false, true /* forceRealize */) is NavigationViewItemBase b2)
            {
                container = b2;
            }
        }

        var ea = new NavigationViewSelectionChangedEventArgs()
        {
            SelectedItem = nextItem,
            IsSettingsSelected = isSettings,
            SelectedItemContainer = container,
            RecommendedNavigationTransitionInfo = CreateNavigationTransitionInfo(recDir)
        };

        SelectionChanged?.Invoke(this, ea);
    }

    // SelectedItem change can be invoked by API or user's action like clicking. if it's not from API, m_shouldRaiseInvokeItemInSelectionChange would be true
    // If nextItem is selectionsuppressed, we should undo the selection. We didn't undo it OnSelectionChange because we want change by API has the same undo logic.
    private void ChangeSelection(object prevItem, object nextItem)
    {
        bool isSettings = IsSettingsItem(nextItem);

        if (IsSelectionSuppressed(nextItem))
        {
            // This should not be a common codepath. Only happens if customer passes a 'selectionsuppressed' item via API.
            UndoSelectionAndRevertSelectionTo(prevItem, nextItem);
            RaiseItemInvoked(nextItem, isSettings);
        }
        else
        {
            // Other transition other than default only apply to topnav
            // when clicking overflow on topnav, transition is from bottom
            // otherwise if prevItem is on left side of nextActualItem, transition is from left
            //           if prevItem is on right side of nextActualItem, transition is from right
            // click on Settings item is considered Default
            var recDir = NavigationRecommendedTransitionDirection.Default;
            if (IsTopNavigationView)
            {
                if (_selectionChangeFromOverflowMenu)
                {
                    recDir = NavigationRecommendedTransitionDirection.FromOverflow;
                }
                else if (prevItem != null && nextItem != null)
                {
                    recDir = GetRecommendedTransitionDirection(NavigationViewItemBaseOrSettingsContentFromData(prevItem),
                        NavigationViewItemBaseOrSettingsContentFromData(nextItem));
                }
            }

            // Bug 17850504, Customer may use NavigationViewItem.IsSelected in ItemInvoke or SelectionChanged Event.
            // To keep the logic the same as RS4, ItemInvoke is before unselect the old item
            // And SelectionChanged is after we selected the new item.
            var selItem = SelectedItem;
            if (_shouldRaiseItemInvokedAfterSelection)
            {
                // If selection changed inside ItemInvoked, the flag does not get said to false and the event get's raised again,so we need to set it to false now!
                _shouldRaiseItemInvokedAfterSelection = false;
                RaiseItemInvoked(nextItem, isSettings, NavigationViewItemOrSettingsContentFromData(nextItem), recDir);
            }
            // Selection was modified inside ItemInvoked, skip everything here!
            if (selItem != SelectedItem)
            {
                return;
            }

            UnselectPrevItem(prevItem, nextItem);
            ChangeSelectStatusForItem(nextItem, true /* isSelected */);
            UpdateSelectionModelSelectionForSelectedItem(nextItem);

            // UIA stuff
            {
                try
                {
                    // Selection changed and we need to notify UIA
                    // HOWEVER expand collapse can also trigger if an item can expand/collapse
                    // There are multiple cases when selection changes:
                    // - Through click on item with no children -> No expand/collapse change
                    // - Through click on item with children -> Expand/collapse change
                    // - Through API with item without children -> No expand/collapse change
                    // - Through API with item with children -> No expand/collapse change
                    if (!_shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise)
                    {
                        if (ControlAutomationPeer.FromElement(this) is NavigationViewAutomationPeer p)
                        {
                            p.RaiseSelectionChangedEvent(prevItem, nextItem);
                        }
                    }
                }
                finally
                {
                    _shouldIgnoreUIASelectionRaiseAsExpandCollapseWillRaise = false;
                }
            }

            // If this item has an associated container, we'll raise the SelectionChanged event on it immediately.
            var nvi = NavigationViewItemOrSettingsContentFromData(nextItem);
            if (nvi != null)
            {
                AnimateSelectionChanged(nextItem);
                RaiseSelectionChangedEvent(nextItem, isSettings, recDir);
                ClosePaneIfNecessaryAfterItemIsClicked(nvi);
            }
            else
            {
                // Otherwise, we'll wait until a container gets realized for this item and raise it then.
                _isSelectionChangedPending = true;
                _pendingSelectionChangedItem = nextItem;
                _pendingSelectionChangedDirection = recDir;

                Dispatcher.UIThread.Post(() =>
                {
                    CompletePendingSelectionChange();
                });
            }
        }
    }

    private void CompletePendingSelectionChange()
    {
        // It may be the case that this item is in a collapsed repeater, in which case
        // no container will be realized for it.  We'll assume that this this is the case
        // if the UI thread has fallen idle without any SelectionChanged being raised.
        // In this case, we'll raise the SelectionChanged at that time, as otherwise it'll never be raised.
        if (_isSelectionChangedPending)
        {
            AnimateSelectionChanged(FindLowestLevelContainerToDisplaySelectionIndicator());
            _isSelectionChangedPending = false;

            var item = _pendingSelectionChangedItem;
            var direction = _pendingSelectionChangedDirection;

            _pendingSelectionChangedItem = null;
            _pendingSelectionChangedDirection = default;

            RaiseSelectionChangedEvent(item, IsSettingsItem(item), direction);
        }
    }

    private void UpdateSelectionModelSelectionForSelectedItem(object selectedItem)
    {
        IndexPath indexPath = IndexPath.Unselected;

        if (NavigationViewItemBaseOrSettingsContentFromData(selectedItem) is NavigationViewItemBase c)
        {
            indexPath = GetIndexPathForContainer(c);
        }
        else
        {
            indexPath = GetIndexPathOfItem(selectedItem);
        }

        if (indexPath != IndexPath.Unselected && indexPath.GetSize() > 0)
        {
            // The SelectedItem property has already been updated. So we want to block any logic from executing
            // in the SelectionModel selection changed callback.
            try
            {
                _shouldIgnoreNextSelectionChange = true;
                UpdateSelectionModelSelection(indexPath);
            }
            finally
            {
                _shouldIgnoreNextSelectionChange = false;
            }
        }
    }

    private void UpdateSelectionModelSelection(IndexPath ip)
    {
        var prevIP = _selectionModel.SelectedIndex;
        _selectionModel.SelectAt(ip);
        UpdateIsChildSelected(prevIP, ip);
    }

    private void UpdateIsChildSelected(IndexPath prevIP, IndexPath nextIP)
    {
        if (prevIP != IndexPath.Unselected && prevIP.GetSize() > 0)
        {
            UpdateIsChildSelectedForIndexPath(prevIP, false);
        }

        if (nextIP != IndexPath.Unselected && nextIP.GetSize() > 0)
        {
            UpdateIsChildSelectedForIndexPath(nextIP, true);
        }
    }

    private void UpdateIsChildSelectedForIndexPath(IndexPath ip, bool isChildSelected)
    {
        // Update the isChildSelected property for every container on the IndexPath (with the exception of the actual container pointed to by the indexpath)
        var cont = GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == _footerMenuBlockIndex);
        // first index is fo mainmenu or footer
        // second is index of item in mainmenu or footer
        // next in menuitem children 
        var index = 2;
        while (cont != null)
        {
            if (cont is NavigationViewItem nvi)
            {
                nvi.IsChildSelected = isChildSelected;
                if (nvi.GetRepeater is ItemsRepeater ir)
                {
                    if (index < ip.GetSize() - 1)
                    {
                        cont = ir.TryGetElement(ip.GetAt(index));
                        index++;
                        continue;
                    }
                }
            }
            cont = null;
        }
    }

    private void RaiseItemInvoked(object item, bool isSettings, NavigationViewItemBase container = null, NavigationRecommendedTransitionDirection recDir = NavigationRecommendedTransitionDirection.Default)
    {
        var ea = new NavigationViewItemInvokedEventArgs();

        if (container != null)
        {
            item = container.Content;
        }
        else
        {
            // InvokedItem is container for Settings, but Content of item for other ListViewItem
            if (!isSettings)
            {
                var contFromData = NavigationViewItemBaseOrSettingsContentFromData(item);
                item = contFromData.Content;
                container = contFromData;
            }
            else
            {
                container = item as NavigationViewItemBase;
            }
        }

        ea.InvokedItem = item;
        ea.InvokedItemContainer = container;
        ea.IsSettingsInvoked = isSettings;
        ea.RecommendedNavigationTransitionInfo = CreateNavigationTransitionInfo(recDir);
        ItemInvoked?.Invoke(this, ea);
    }

    private void SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(object selItem)
    {
        SelectedItem = selItem;
    }

    private void ChangeSelectStatusForItem(object item, bool selected)
    {
        var container = NavigationViewItemOrSettingsContentFromData(item);
        if (container != null)
        {
            // If we unselect an item, ListView doesn't tolerate setting the SelectedItem to nullptr. 
            // Instead we remove IsSelected from the item itself, and it make ListView to unselect it.
            // If we select an item, we follow the unselect to simplify the code.
            container.IsSelected = selected;
        }
        else if (selected)
        {
            // If we are selecting an item and have not found a realized container for it,
            // we may need to manually resolve a container for this in order to update the
            // SelectionModel's selected IndexPath.
            var ip = GetIndexPathOfItem(item);
            if (ip != IndexPath.Unselected && ip.GetSize() > 0)
            {
                // The SelectedItem property has already been updated. So we want to block any logic from executing
                // in the SelectionModel selection changed callback.
                try
                {
                    _shouldIgnoreNextSelectionChange = true;
                    UpdateSelectionModelSelection(ip);
                }
                finally
                {
                    _shouldIgnoreNextSelectionChange = false;
                }
            }
        }
    }

    private void UnselectPrevItem(object prevItem, object nextItem)
    {
        if (prevItem != null && prevItem != nextItem)
        {
            try
            {
                _shouldIgnoreNextSelectionChange = true;
                ChangeSelectStatusForItem(prevItem, false);
            }
            finally
            {
                _shouldIgnoreNextSelectionChange = false;
            }
        }
    }

    private void UndoSelectionAndRevertSelectionTo(object prevSelectedItem, object nextItem)
    {
        object selItem = null;
        if (prevSelectedItem != null)
        {
            if (IsSelectionSuppressed(prevSelectedItem))
            {
                AnimateSelectionChanged(null);
            }
            else
            {
                ChangeSelectStatusForItem(prevSelectedItem, true);
                AnimateSelectionChangedToItem(prevSelectedItem);
                selItem = prevSelectedItem;
            }
        }
        else
        {
            // Bug 18033309, A SelectsOnInvoked=false item is clicked, if we don't unselect it from listview, the second click will not raise ItemClicked
            // because listview doesn't raise SelectionChange.
            ChangeSelectStatusForItem(nextItem, false);
        }

        SelectedItem = selItem;
    }

    private void SelectOverflowItem(object item, IndexPath ip)
    {
        object itemBeingMoved = item;
        if (ip.GetSize() > 2)
        {
            itemBeingMoved = GetItemFromIndex(_topNavRepeaterOverflowView, _topDataProvider.ConvertOriginalIndexToIndex(ip.GetAt(1)));
        }

        // Calculate selected overflow item size.
        var selOverflowItemIndex = _topDataProvider.IndexOf(itemBeingMoved);
        if (selOverflowItemIndex == _itemNotFound) { return; }
        var selOverflowItemWidth = _topDataProvider.GetWidthForItem(selOverflowItemIndex);

        bool needInvalidMeasure = !_topDataProvider.IsValidWidthForItem(selOverflowItemIndex);

        if (!needInvalidMeasure)
        {
            var actWid = GetTopNavigationViewActualWidth;
            var desWid = MeasureTopNavigationViewDesiredWidth(Size.Infinity);
            
            if (desWid > actWid) { Debug.WriteLine($"desWid={desWid}, actWid={actWid}"); }

            // Calculate selected item size
            var selItemIndex = _itemNotFound;
            var selItemWidth = 0d;
            if (SelectedItem != null)
            {
                selItemIndex = _topDataProvider.IndexOf(SelectedItem);
                if (selItemIndex != _itemNotFound)
                {
                    selItemWidth = _topDataProvider.GetWidthForItem(selItemIndex);
                }
            }

            var widthAtLeastToBeRemoved = desWid + selOverflowItemWidth - actWid;

            // calculate items to be removed from primary because a overflow item is selected. 
            // SelectedItem is assumed to be removed from primary first, then added it back if it should not be removed
            var itemsToBeRemoved = FindMovableItemsToBeRemovedFromPrimaryList(widthAtLeastToBeRemoved, new List<int>(0));

            // calculate the size to be removed
            var topBeRemovedItemWidth = _topDataProvider.CalculateWidthForItems(itemsToBeRemoved);

            var widthAvailableToRecover = topBeRemovedItemWidth - widthAtLeastToBeRemoved;
            var itemsToBeAdded = FindMovableItemsRecoverToPrimaryList(widthAvailableToRecover, new int[] { selOverflowItemIndex });

            itemsToBeAdded.Add(selOverflowItemIndex);

            // Keep track of the item being moved in order to know where to animate selection indicator
            _lastSelectedItemPendingAnimationInTopNav = itemBeingMoved;
            if (ip != IndexPath.Unselected && ip.GetSize() > 0)
            {
                for (int i = 0; i < itemsToBeRemoved.Count; i++)
                {
                    if (ip.GetAt(1) == itemsToBeRemoved[i])
                    {
                        if (_activeIndicator != null)
                        {
                            // If the previously selected item is being moved into overflow, hide its indicator
                            // as we will no longer need to animate from its location.
                            AnimateSelectionChanged(null);
                        }
                        break;
                    }
                }
            }

            if (_topDataProvider.HasInvalidWidth(itemsToBeAdded))
            {
                needInvalidMeasure = true;
            }
            else
            {
                // Exchange items between Primary and Overflow
                {
                    _topDataProvider.MoveItemsToPrimaryList(itemsToBeAdded);
                    _topDataProvider.MoveItemsOutOfPrimaryList(itemsToBeRemoved);
                }

                if (NeedRearrangeOfTopElementsAfterOverflowSelectionChange(selOverflowItemIndex))
                {
                    needInvalidMeasure = true;
                }

                if (!needInvalidMeasure)
                {
                    SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(item);
                    InvalidateMeasure();
                }
            }
        }

        // (WinUI) TODO: Verify that this is no longer needed and delete
        if (needInvalidMeasure)
        {
            // not all items have known width, need to redo the layout
            _topDataProvider.MoveAllItemsToPrimaryList();
            SetSelectedItemAndExpectItemInvokeWhenSelectionChangedIfNotInvokedFromAPI(item);
            InvalidateTopNavPrimaryLayout();
        }
    }

    private void UpdateSelectionForMenuItems()
    {
        // Allow customer to set selection by NavigationViewItem.IsSelected.
        // If there are more than two items are set IsSelected=true, the first one is actually selected.
        // If SelectedItem is set, IsSelected is ignored.
        //         <NavigationView.MenuItems>
        //              <NavigationViewItem Content = "Collection" IsSelected = "True" / >
        //         </NavigationView.MenuItems>
        if (SelectedItem == null)
        {
            bool foundFirstSelected = false;

            // firstly check Menu items
            foundFirstSelected = UpdateSelectedItemFromMenuItems(_menuItems);

            UpdateSelectedItemFromMenuItems(_footerMenuItems, foundFirstSelected);
        }
    }

    private bool UpdateSelectedItemFromMenuItems(IEnumerable menuItems, bool foundFirstSelected = false)
    {
        for (int i = 0; i < menuItems.Count(); i++)
        {
            if (menuItems.ElementAt(i) is NavigationViewItem nvi)
            {
                if (nvi.IsSelected)
                {
                    if (!foundFirstSelected)
                    {
                        try
                        {
                            _shouldIgnoreNextSelectionChange = true;
                            SelectedItem = nvi;
                            foundFirstSelected = true;
                        }
                        finally
                        {
                            _shouldIgnoreNextSelectionChange = false;
                        }
                    }
                    else
                    {
                        nvi.IsSelected = false;
                    }
                }
            }
        }

        return foundFirstSelected;
    }





    /////////////////////////////////////////////////
    //////// NAVIGATIONVIEWITEM RELATED ////////////
    ///////////////////////////////////////////////

    private void SetNavigationViewItemBaseRevokers(NavigationViewItemBase nvib)
    {
        var disp = new FACompositeDisposable(
            nvib.GetPropertyChangedObservable(IsVisibleProperty).Subscribe(OnNavigationViewItemBaseVisibilityPropertyChanged));

        nvib.SetValue(NavigationViewItemBaseRevokersProperty, disp);
        // Note: I'm not doing this since this list is for the destructor in the C++ side of the control
        // Since that is unnecessary here, skip it
        //_itemsWithRevokers.Add(nvib);
    }

    private void SetNavigationViewItemRevokers(NavigationViewItem nvi)
    {
        var revokers = nvi.GetValue(NavigationViewItemBaseRevokersProperty);
        // Technically this shouldn't happen b/c BaseRevokers should be called first
        if (revokers == null)
        {
            revokers = new FACompositeDisposable();
            nvi.SetValue(NavigationViewItemBaseRevokersProperty, revokers);
        }

        revokers.Add(nvi.AddDisposableHandler(KeyDownEvent, OnNavigationViewItemKeyDown));
        revokers.Add(nvi.AddDisposableHandler(GotFocusEvent, OnNavigationViewItemGotFocus));
        revokers.Add(nvi.AddDisposableHandler(TappedEvent, OnNavigationViewItemTapped));
        revokers.Add(nvi.GetPropertyChangedObservable(ListBoxItem.IsSelectedProperty).Subscribe(OnNavigationViewItemIsSelectedPropertyChanged));
        revokers.Add(nvi.GetPropertyChangedObservable(NavigationViewItem.IsExpandedProperty).Subscribe(OnNavigationViewItemExpandedPropertyChanged));
    }

    private void ClearNavigationViewItemBaseRevokers(NavigationViewItemBase nvib)
    {
        //RevokeNavigationViewItemBaseRevokers;
        var revokers = nvib.GetValue(NavigationViewItemBaseRevokersProperty);
        revokers?.Dispose();
        nvib.SetValue(NavigationViewItemBaseRevokersProperty, null);
        //_itemsWithRevokers.Remove(nvib);
    }

    private void OnNavigationViewItemIsSelectedPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is NavigationViewItem nvi)
        {
            // Check whether the container that triggered this call back is the selected container
            bool isContainerSelectedInModel = IsContainerTheSelectedItemInTheSelectionModel(nvi);
            bool isSelectedInContainer = nvi.IsSelected;

            if (isSelectedInContainer && !isContainerSelectedInModel)
            {
                var indexPath = GetIndexPathForContainer(nvi);
                UpdateSelectionModelSelection(indexPath);
            }
            else if (!isSelectedInContainer && isContainerSelectedInModel)
            {
                var indexPath = GetIndexPathForContainer(nvi);
                var indexPathFromModel = _selectionModel.SelectedIndex;

                if (indexPathFromModel != IndexPath.Unselected)
                {
                    if (indexPath.CompareTo(indexPathFromModel) == 0)
                    {
                        _selectionModel.DeselectAt(indexPath);
                    }
                    else if (!IsPaneOpen && indexPath.GetSize() == 0)
                    {
                        UpdateIsChildSelected(indexPathFromModel, IndexPath.Unselected);
                        if (_prevIndicator == null && _nextIndicator == null && _activeIndicator != null)
                        {
                            ResetElementAnimationProperties(_activeIndicator, 0);
                            _activeIndicator = null;
                        }

                        _selectionModel.DeselectAt(indexPathFromModel);
                    }
                }
            }

            if (isSelectedInContainer)
            {
                nvi.IsChildSelected = false;
            }
        }
    }

    private void OnNavigationViewItemExpandedPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Sender is NavigationViewItem nvi)
        {
            if (nvi.IsExpanded)
            {
                RaiseExpandingEvent(nvi);
            }

            ShowHideChildrenItemsRepeater(nvi);

            if (!nvi.IsExpanded)
            {
                RaiseCollapsedEvent(nvi);
            }
        }
    }

    private void OnNavigationViewItemBaseVisibilityPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        UpdatePaneLayout();
    }

    private void RaiseItemInvokedForNavigationViewItem(NavigationViewItem nvi)
    {
        object nextItem = null;
        var prevItem = SelectedItem;
        var parentIR = GetParentItemsRepeaterForContainer(nvi);

        if (parentIR.ItemsSourceView != null)
        {
            var itemIndex = parentIR.GetElementIndex(nvi);

            // Check that index is NOT -1, meaning it is actually realized
            if (itemIndex != -1)
            {
                // Something went wrong, item might not be realized yet.
                nextItem = parentIR.ItemsSourceView.GetAt(itemIndex);
            }
        }

        var recDir = NavigationRecommendedTransitionDirection.Default;
        if (IsTopNavigationView && nvi.SelectsOnInvoked)
        {
            bool isInOverflow = parentIR == _topNavRepeaterOverflowView;
            if (isInOverflow)
            {
                recDir = NavigationRecommendedTransitionDirection.FromOverflow;
            }
            else if (prevItem != null)
            {
                recDir = GetRecommendedTransitionDirection(NavigationViewItemBaseOrSettingsContentFromData(prevItem), nvi);
            }
        }

        RaiseItemInvoked(nextItem, IsSettingsItem(nvi), nvi, recDir);
    }

    private void OnNavigationViewItemInvoked(NavigationViewItem nvi)
    {
        _shouldRaiseItemInvokedAfterSelection = true;
        var selItem = SelectedItem;
        bool updateSelection = _selectionModel != null && nvi.SelectsOnInvoked;

        if (updateSelection)
        {
            var ip = GetIndexPathForContainer(nvi);

            // Determine if we will update collapse/expand which will happen if the item has children
            if (DoesNavigationViewItemHaveChildren(nvi))
            {
                //UIA stuff...
            }
            UpdateSelectionModelSelection(ip);
        }

        // Item was invoked but already selected, so raise event here
        if (selItem == SelectedItem)
        {
            RaiseItemInvokedForNavigationViewItem(nvi);
        }

        ToggleIsExpandedNavigationViewItem(nvi);
        ClosePaneIfNecessaryAfterItemIsClicked(nvi);

        if (updateSelection)
        {
            CloseFlyoutIfRequired(nvi);
        }
    }

    private void OnNavigationViewItemGotFocus(object sender, FocusChangedEventArgs e)
    {
        var nvi = (NavigationViewItem)sender;

        // In WinUI, Focus isn't given to an item until AFTER the Tapped event, which differs
        // from how Avalonia handles it. Which means here, this is called first, and the NVI
        // hasn't yet been selected, which means OnNVIInvoked gets called twice and will open
        // and close an item if it has child items. So disable here if focus was given via
        // the pointer to prevent that from occuring
        if (SelectionFollowsFocus && e.NavigationMethod != NavigationMethod.Pointer)
        {
            // if nvi is already selected we don't need to invoke it again
            // otherwise ItemInvoked fires twice when item was tapped
            // or fired when window gets focus
            if (nvi.SelectsOnInvoked && !nvi.IsSelected)
            {
                if (IsTopNavigationView)
                {
                    var parentIR = GetParentItemsRepeaterForContainer(nvi);
                    if (parentIR != null && parentIR != _topNavRepeaterOverflowView)
                    {
                        OnNavigationViewItemInvoked(nvi);
                    }
                }
                else
                {
                    OnNavigationViewItemInvoked(nvi);
                }
            }
        }
    }

    private void OnNavigationViewItemKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.Enter || args.Key == Key.Space)
        {
            //need args.KeyStatus.WasKeyDown
            /*
                // Only handle those keys if the key is not being held down!
                if (!args.KeyStatus().WasKeyDown)
                {
                    if (auto nvi = sender.try_as<winrt::NavigationViewItem>())
                    {
                        HandleKeyEventForNavigationViewItem(nvi, args);
                    }
                }
             */

            if (sender is NavigationViewItem nvi)
            {
                HandleKeyEventForNavigationViewItem(nvi, args);
            }
        }
        else
        {
            if (sender is NavigationViewItem nvi)
            {
                HandleKeyEventForNavigationViewItem(nvi, args);
            }
        }
    }

    private void HandleKeyEventForNavigationViewItem(NavigationViewItem nvi, KeyEventArgs args)
    {
        //NOTE: Key logic diverges from WinUI to compensate for the lack of
        //      XYKeyboardFocus in Avalonia, which handles most of the logic
        switch (args.Key)
        {
            case Key.Enter:
            case Key.Space:
                args.Handled = true;
                OnNavigationViewItemInvoked(nvi);
                break;

            case Key.Home: //Go to very first top-level MenuItem in list
                args.Handled = true;
                KeyboardFocusFirstItemFromItem(nvi);
                break;

            case Key.End: //Go to very last top-level MenuItem in list
                args.Handled = true;
                KeyboardFocusLastItemFromItem(nvi);
                break;

            case Key.Right:
                if (IsTopNavigationView)
                    FocusNextDownItem(nvi, args);
                break;

            case Key.Down: //next item or subitem down
                if (!IsTopNavigationView)
                    FocusNextDownItem(nvi, args);
                break;

            case Key.Left:
                if (IsTopNavigationView)
                    FocusNextUpItem(nvi, args);
                break;

            case Key.Up: //next item or subitem up
                if (!IsTopNavigationView)
                    FocusNextUpItem(nvi, args);
                break;
        }
    }

    private void KeyboardFocusFirstItemFromItem(NavigationViewItemBase nvib)
    {
        ItemsRepeater parentIR = null;
        if (_lastItemExpandedIntoFlyout != null)
        {
            parentIR = _lastItemExpandedIntoFlyout.GetRepeater;
        }
        else
        {
            parentIR = GetParentRootItemsRepeaterForContainer(nvib);
        }

        if (GetFirstFocusableElement(parentIR) is Control c)
        {
            c.Focus(NavigationMethod.Directional);
        }
    }

    private void KeyboardFocusLastItemFromItem(NavigationViewItemBase nvib)
    {
        ItemsRepeater parentIR = null;
        if (_lastItemExpandedIntoFlyout != null)
        {
            parentIR = _lastItemExpandedIntoFlyout.GetRepeater;
        }
        else
        {
            parentIR = GetParentRootItemsRepeaterForContainer(nvib);
        }

        if (GetLastFocusableElement(parentIR) is Control c)
        {
            c.Focus(NavigationMethod.Directional);
        }
    }

    private void FocusNextUpItem(NavigationViewItem nvi, KeyEventArgs args)
    {
        if (args.Source != nvi)
            return;

        bool shouldHandleFocus = false;
        var options = new FindNextElementOptions() { SearchRoot = TopLevel.GetTopLevel(this)?.Content as InputElement };
        var nextFocusableElement = TopLevel.GetTopLevel(this)?.FocusManager.FindNextElement(NavigationDirection.Up, options);

        if (nextFocusableElement is NavigationViewItem nextNVI)
        {
            if (nextNVI.Depth == nvi.Depth)
            {
                // If we not at the top of the list for our current depth and the item above us has children, check whether we should move focus onto a child
                if (DoesNavigationViewItemHaveChildren(nextNVI))
                {
                    // Focus on last lowest level visible container
                    if (nextNVI.GetRepeater is ItemsRepeater ir)
                    {
                        if (FocusManager.FindLastFocusableElement(ir) is Control lastFocusableElement)
                        {
                            args.Handled = lastFocusableElement.Focus(NavigationMethod.Directional);
                        }
                        else
                        {
                            args.Handled = nextNVI.Focus(NavigationMethod.Directional);
                        }
                    }
                }
                else
                {
                    // Traversing up a list where XYKeyboardFocus will result in correct behavior
                    shouldHandleFocus = false;
                }
            }
        }

        // We are at the top of the list, focus on parent
        if (shouldHandleFocus && !args.Handled && nvi.Depth > 0)
        {
            if (GetParentNavigationViewItemForContainer(nvi) is NavigationViewItem parent)
            {
                args.Handled = parent.Focus(NavigationMethod.Directional);
            }
        }
    }

    private void FocusNextDownItem(NavigationViewItem nvi, KeyEventArgs args)
    {
        if (args.Source != nvi)
            return;

        if (DoesNavigationViewItemHaveChildren(nvi))
        {
            if (nvi.GetRepeater is ItemsRepeater ir)
            {
                var first = FocusManager.FindFirstFocusableElement(ir);
                if (first != null)
                {
                    args.Handled = first.Focus(NavigationMethod.Directional);
                }
            }
        }
    }

    private Control GetFirstFocusableElement(ItemsRepeater ir)
    {
        if (ir == null)
            return null;

        if (ir.ItemsSourceView is ItemsSourceView isv)
        {
            var lastIndex = isv.Count - 1;
            int index = 0;
            while (index <= lastIndex)
            {
                if (ir.TryGetElement(index) is Control c && c.Focusable)
                {
                    return c;
                }

                index++;
            }
        }

        return null;
    }

    private Control GetLastFocusableElement(ItemsRepeater ir)
    {
        if (ir == null)
            return null;

        if (ir.ItemsSourceView is ItemsSourceView isv)
        {
            int index = isv.Count - 1;
            while (index >= 0)
            {
                if (ir.TryGetElement(index) is Control c && c.Focusable)
                {
                    return c;
                }

                index--;
            }
        }

        return null;
    }

    private void OnNavigationViewItemTapped(object sender, RoutedEventArgs e)
    {
        var nvi = (NavigationViewItem)sender;
        OnNavigationViewItemInvoked(nvi);
        nvi.Focus();
        e.Handled = true;
    }

    private void Expand(NavigationViewItem nvi)
    {
        ChangeIsExpandedNavigationViewItem(nvi, true);
    }

    private void Collapse(NavigationViewItem nvi)
    {
        ChangeIsExpandedNavigationViewItem(nvi, false);
    }

    private void ToggleIsExpandedNavigationViewItem(NavigationViewItem nvi)
    {
        ChangeIsExpandedNavigationViewItem(nvi, !nvi.IsExpanded);
    }

    private void ChangeIsExpandedNavigationViewItem(NavigationViewItem nvi, bool isExpanded)
    {
        if (DoesNavigationViewItemHaveChildren(nvi))
        {
            nvi.IsExpanded = isExpanded;
        }
    }

    private void ShowHideChildrenItemsRepeater(NavigationViewItem nvi)
    {
        nvi.ShowHideChildren();

        if (nvi.ShouldRepeaterShowInFlyout)
        {
            _lastItemExpandedIntoFlyout = nvi.IsExpanded ? nvi : null;
        }

        // If SelectedItem is being hidden/shown, animate SelectionIndicator
        if (!nvi.IsSelected && nvi.IsChildSelected)
        {
            if (!nvi.IsRepeaterVisible && nvi.IsChildSelected)
            {
                AnimateSelectionChanged(nvi);
            }
            else
            {
                AnimateSelectionChanged(FindLowestLevelContainerToDisplaySelectionIndicator());
            }
        }

        nvi.RotateExpandCollapseChevron(nvi.IsExpanded);
    }

    private void CollapseTopLevelMenuItems(NavigationViewPaneDisplayMode oldDMode)
    {
        if (oldDMode == NavigationViewPaneDisplayMode.Top)
        {
            CollapseMenuItemsInRepeater(_topNavRepeater);
            CollapseMenuItemsInRepeater(_topNavRepeaterOverflowView);
        }
        else
        {
            CollapseMenuItemsInRepeater(_leftNavRepeater);
        }
    }

    private void CollapseMenuItemsInRepeater(ItemsRepeater ir)
    {
        for (int i = 0; i < GetContainerCountInRepeater(ir); i++)
        {
            if (ir.TryGetElement(i) is NavigationViewItem nvi)
            {
                ChangeIsExpandedNavigationViewItem(nvi, false);
            }
        }
    }

    private void RaiseExpandingEvent(NavigationViewItemBase nvib)
    {
        var ea = new NavigationViewItemExpandingEventArgs(this);
        ea.ExpandingItemContainer = nvib;
        ItemExpanding?.Invoke(this, ea);
    }

    private void RaiseCollapsedEvent(NavigationViewItemBase nvib)
    {
        var ea = new NavigationViewItemCollapsedEventArgs(this);
        ea.CollapsedItemContainer = nvib;
        ItemCollapsed?.Invoke(this, ea);
    }

    private void OnSelectedItemLayoutUpdated(object sender, EventArgs args)
    {
        if (_isSelectionChangedPending)
        {
            _isSelectionChangedPending = false;

            var item = _pendingSelectionChangedItem;
            var direction = _pendingSelectionChangedDirection;

            _pendingSelectionChangedItem = null;
            _pendingSelectionChangedDirection = NavigationRecommendedTransitionDirection.Default;

            (sender as Control).LayoutUpdated -= OnSelectedItemLayoutUpdated;

            var nvi = NavigationViewItemOrSettingsContentFromData(item);
            if (nvi != null)
            {
                AnimateSelectionChanged(nvi);
            }

            RaiseSelectionChangedEvent(item, IsSettingsItem(item), direction);
        }
    }





    ///////////////////////////////////////
    //////// LEFT NAV RELATED ////////////
    /////////////////////////////////////

    private void UpdateAdaptiveLayout(double width, bool forceSetDisplayMode = false)
    {
        // In top nav, this is no adaptive pane layout
        if (IsTopNavigationView || _splitView == null)
            return;

        // If we decide we want it to animate open/closed when you resize the
        // window we'll have to change how we figure out the initial state
        // instead of this:
        // _initialListSizeStateSet = false; // see UpdateIsClosedCompact

        NavigationViewDisplayMode dMode = NavigationViewDisplayMode.Compact;

        var paneDisplayMode = PaneDisplayMode;
        if (paneDisplayMode == NavigationViewPaneDisplayMode.Auto)
        {
            if (width >= ExpandedModeThresholdWidth)
            {
                dMode = NavigationViewDisplayMode.Expanded;
            }
            else if (width > 0 && width < CompactModeThresholdWidth)
            {
                dMode = NavigationViewDisplayMode.Minimal;
            }
        }
        else if (paneDisplayMode == NavigationViewPaneDisplayMode.Left)
        {
            dMode = NavigationViewDisplayMode.Expanded;
        }
        else if (paneDisplayMode == NavigationViewPaneDisplayMode.LeftCompact)
        {
            dMode = NavigationViewDisplayMode.Compact;
        }
        else if (paneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal)
        {
            dMode = NavigationViewDisplayMode.Minimal;
        }

        if (!forceSetDisplayMode && _initialNonForcedModeUpdate)
        {
            if (dMode == NavigationViewDisplayMode.Minimal ||
                dMode == NavigationViewDisplayMode.Compact)
            {
                ClosePane();
            }
            _initialNonForcedModeUpdate = false;
        }

        var prev = DisplayMode;
        SetDisplayMode(dMode, forceSetDisplayMode);

        if (dMode == NavigationViewDisplayMode.Expanded && IsPaneVisible)
        {
            if (!_wasForceClosed)
            {
                OpenPane();
            }
        }

        if (prev == NavigationViewDisplayMode.Expanded &&
            dMode == NavigationViewDisplayMode.Compact)
        {
            // _initialListSizeStateSet = false;
            ClosePane();
        }

        if (dMode == NavigationViewDisplayMode.Minimal)
        {
            ClosePane();
        }
    }

    private void UpdatePaneLayout()
    {
        if (IsTopNavigationView)
            return;

        double totalAvailableHeight()
        {
            if (_itemsContainerRow != null)
            {
                double itemsContMargin = _itemsContainer?.Margin.Vertical() ?? 0d;

                return _itemsContainerRow.ActualHeight - itemsContMargin;
            }
            return 0;
        }

        var totalHeight = totalAvailableHeight();

        // Only continue if we have a positive amount of space to manage.
        if (totalHeight > 0)
        {
            double heightForMenuItems()
            {
                // We need this value more than twice, so cache it.
                var totalHeightHalf = totalHeight / 2;

                if (_footerItemsScrollViewer != null)
                {
                    if (_leftNavFooterMenuRepeater != null)
                    {
                        // We know the actual height of footer items, so use that to determine how to split pane.
                        if (_leftNavRepeater != null)
                        {
                            double footerDesiredHeight = 0;
                            {
                                double footerItemsRepeaterTopBottomMargin = 0;
                                if (_leftNavFooterMenuRepeater.IsVisible)
                                    footerItemsRepeaterTopBottomMargin = _leftNavFooterMenuRepeater.Margin.Vertical();

                                footerDesiredHeight = footerItemsRepeaterTopBottomMargin +
                                                      LayoutHelper.MeasureChild(_leftNavFooterMenuRepeater, Size.Infinity, default).Height;
                            }

                            double paneFooterActualHeight = 0;
                            {
                                if (_leftNavFooterContentBorder != null)
                                {
                                    double paneFooterTopBottomMargin = 0;
                                    if (_leftNavFooterContentBorder.IsVisible)
                                        paneFooterTopBottomMargin = _leftNavFooterContentBorder.Margin.Vertical();

                                    paneFooterActualHeight = _leftNavFooterContentBorder.Bounds.Height +
                                                             paneFooterTopBottomMargin;
                                }
                            }


                            // This is the value computed during the measure pass of the layout process. This will be the value used to determine
                            // the partition logic between menuItems and footerGroup, since the ActualHeight may be taller if there's more space.
                            var menuItemsDesiredHeight = _leftNavRepeater.DesiredSize.Height;

                            // This is what the height ended up being, so will be the value that is used to calculate the partition
                            // between menuItems and footerGroup.
                            double menuItemsActualHeight =
                                _leftNavRepeater.Bounds.Height + (_leftNavRepeater.IsVisible ?
                                    _leftNavRepeater.Margin.Vertical() : 0);

                            // Footer and PaneFooter are included in the footerGroup to calculate available height for menu items.
                            var footerGroupDesiredHeight = footerDesiredHeight + paneFooterActualHeight;

                            if (_footerItemsSource.Count == 0 && !IsSettingsVisible)
                            {
                                return totalHeight;
                            }
                            else if (_menuItemsSource.Count == 0)
                            {
                                _footerItemsScrollViewer.MaxHeight = totalHeight;
                                return 0d;
                            }
                            else if (totalHeight >= menuItemsDesiredHeight + footerGroupDesiredHeight)
                            {
                                // We have enough space for two so let everyone get as much as they need
                                _footerItemsScrollViewer.MaxHeight = footerDesiredHeight;
                                return totalHeight - footerDesiredHeight;
                            }
                            else if (menuItemsDesiredHeight <= totalHeightHalf)
                            {
                                // Footer items exceed over the half, so let's limit them.
                                _footerItemsScrollViewer.MaxHeight = (totalHeight - menuItemsActualHeight);
                                return menuItemsActualHeight;
                            }
                            else if (footerDesiredHeight <= totalHeightHalf)
                            {
                                // Menu items exceed over the half, so let's limit them.
                                _footerItemsScrollViewer.MaxHeight = footerDesiredHeight;
                                return totalHeight - footerDesiredHeight;
                            }
                            else
                            {
                                // Both are more than half the height, so split evenly.
                                _footerItemsScrollViewer.MaxHeight = totalHeightHalf;
                                return totalHeightHalf;
                            }
                        }
                        else
                        {
                            // Couldn't determine the menuItems.
                            // Let's just take all the height and let the other repeater deal with it.
                            return totalHeight - _leftNavFooterMenuRepeater.Bounds.Height;
                        }
                    }

                    // We have no idea how much space to occupy as we are not able to get the size of the footer repeater.
                    // Stick with 50% as backup.
                    _footerItemsScrollViewer.MaxHeight = totalHeightHalf;
                }

                // We couldn't find a good strategy, so limit to 50% percent for the menu items.
                return totalHeightHalf;
            }

            // Footer items should have precedence as that usually contains very
            // important items such as settings or the profile.
            if (_menuItemsScrollViewer != null)
            {
                _menuItemsScrollViewer.MaxHeight = heightForMenuItems();
            }
        }
    }

    private void OnPaneTitleHolderSizeChanged(Rect r)
    {
        UpdateBackAndCloseButtonsVisibility();
    }

    private void OpenPane()
    {
        try
        {
            _isOpenPaneForInteraction = true;
            IsPaneOpen = true;
        }
        finally
        {
            _isOpenPaneForInteraction = false;
        }
    }

    // Call this when you want an uncancellable close
    private void ClosePane()
    {
        try
        {
            _isOpenPaneForInteraction = true;
            IsPaneOpen = false;
        }
        finally
        {
            _isOpenPaneForInteraction = false;
        }
    }

    // Call this when NavigationView itself is going to trigger a close
    // where you will stop the close if the cancel is triggered
    private bool AttemptClosePaneLightly()
    {
        bool pendingCancel = false;

        var ea = new NavigationViewPaneClosingEventArgs();
        PaneClosing?.Invoke(this, ea);
        pendingCancel = ea.Cancel;

        if (!pendingCancel || _wasForceClosed)
        {
            _blockNextClosingEvent = true;
            ClosePane();
            return true;
        }

        return false;
    }

    private void UpdatePaneTabFocusNavigation()
    {
        if (!_appliedTemplate)
            return;

        //TODO...
    }

    private void ClosePaneIfNecessaryAfterItemIsClicked(NavigationViewItem item)
    {
        if (IsPaneOpen &&
            DisplayMode != NavigationViewDisplayMode.Expanded &&
            !DoesNavigationViewItemHaveChildren(item) &&
            !_shouldIgnoreNextSelectionChange)
        {
            ClosePane();
        }
    }




    //////////////////////////////////////
    //////// TOP NAV RELATED ////////////
    ////////////////////////////////////

    private void InvalidateTopNavPrimaryLayout()
    {
        if (_appliedTemplate && IsTopNavigationView)
        {
            InvalidateMeasure();
        }
    }

    private void ResetAndRearrangeTopNavItems(Size availableSize)
    {
        if (HasTopNavigationViewItemNotInPrimaryList())
            _topDataProvider.MoveAllItemsToPrimaryList();

        ArrangeTopNavItems(availableSize);
    }

    private void HandleTopNavigationMeasureOverride(Size availableSize)
    {
        // Determine if TopNav is in Overflow
        if (HasTopNavigationViewItemNotInPrimaryList())
        {
            HandleTopNavigationMeasureOverrideOverflow(availableSize);
        }
        else
        {
            HandleTopNavigationMeasureOverrideNormal(availableSize);
        }

        if (_topNavigationMode == TopNavigationViewLayoutState.Uninitialized)
        {
            _topNavigationMode = TopNavigationViewLayoutState.Initialized;
        }
    }

    private void HandleTopNavigationMeasureOverrideNormal(Size availableSize)
    {
        var desWidth = MeasureTopNavigationViewDesiredWidth(Size.Infinity);
        if (desWidth > availableSize.Width)
            ResetAndRearrangeTopNavItems(availableSize);
    }

    private void HandleTopNavigationMeasureOverrideOverflow(Size availableSize)
    {
        var desWidth = MeasureTopNavigationViewDesiredWidth(Size.Infinity);
        if (desWidth > availableSize.Width)
        {
            ShrinkTopNavigationSize(desWidth, availableSize);
        }
        else if (desWidth < availableSize.Width)
        {
            var fullyRecoverWidth = _topDataProvider.WidthRequiredToRecoveryAllItemsToPrimary();
            if (availableSize.Width >= desWidth + fullyRecoverWidth + _topNavigationRecoveryGracePeriodWidth)
            {
                // It's possible to recover from Overflow to Normal state, so we restart the MeasureOverride from first step
                ResetAndRearrangeTopNavItems(availableSize);
            }
            else
            {
                var moveItems = FindMovableItemsRecoverToPrimaryList(availableSize.Width - desWidth, new List<int>(0));
                _topDataProvider.MoveItemsToPrimaryList(moveItems);
            }
        }
    }

    private void ArrangeTopNavItems(Size availableSize)
    {
        SetOverflowButtonVisibility(false);
        var desWidth = MeasureTopNavigationViewDesiredWidth(Size.Infinity);
        if (!(desWidth < availableSize.Width))
        {
            SetOverflowButtonVisibility(true);
            var desWidthForOB = MeasureTopNavigationViewDesiredWidth(Size.Infinity);
            _topDataProvider.OverflowButtonWidth = desWidthForOB - desWidth;

            ShrinkTopNavigationSize(desWidthForOB, availableSize);
        }
    }

    private bool NeedRearrangeOfTopElementsAfterOverflowSelectionChange(int selOriginalIndex)
    {
        bool needRearrange = false;

        var primaryList = _topDataProvider.GetPrimaryItems();
        var primaryListSize = _topDataProvider.PrimaryListSize;
        var indexInPrimary = _topDataProvider.ConvertOriginalIndexToIndex(selOriginalIndex);
        // We need to verify that through various overflow selection combinations, the primary
        // items have not been put into a state of non-logical item layout (aka not in proper sequence).
        // To verify this, if the newly selected item has items following it in the primary items:
        // - we verify that they are meant to follow the selected item as specified in the original order
        // - we verify that the preceding item is meant to directly precede the selected item in the original order
        // If these two conditions are not met, we move all items to the primary list and trigger a re-arrangement of the items.
        if (indexInPrimary < primaryListSize - 1)
        {
            var nextIndexInPrimary = indexInPrimary + 1;
            var nextIndexInOriginal = selOriginalIndex + 1;
            var prevIndexInOriginal = selOriginalIndex - 1;

            // Check whether item preceding the selected is not directly preceding
            // in the original.
            if (indexInPrimary > 0)
            {
                var prevOriginalIndexOfPrevPrimaryItem = _topDataProvider.ConvertPrimaryIndexToIndex(new int[] { nextIndexInPrimary - 1 });
                if (prevOriginalIndexOfPrevPrimaryItem[0] != prevIndexInOriginal)
                {
                    needRearrange = true;
                }
            }

            // Check whether items following the selected item are out of order
            while (!needRearrange && nextIndexInPrimary < primaryListSize)
            {
                var originalIndex = _topDataProvider.ConvertPrimaryIndexToIndex(new int[] { nextIndexInPrimary });
                if (nextIndexInOriginal != originalIndex[0])
                {
                    needRearrange = true;
                    break;
                }
                nextIndexInPrimary++;
                nextIndexInOriginal++;
            }
        }

        return needRearrange;
    }

    private void ShrinkTopNavigationSize(double desWidth, Size availableSize)
    {
        UpdateTopNavigationWidthCache();

        var selItemIndex = SelectedItemIndex;

        var possibleWidthForPrimaryList = MeasureTopNavMenuItemsHostDesiredWidth(Size.Infinity) - (desWidth - availableSize.Width);
        if (possibleWidthForPrimaryList >= 0)
        {
            // Remove all items which is not visible except first item and selected item.
            var itemToBeRemoved = FindMovableItemsBeyondAvailableWidth(possibleWidthForPrimaryList);
            // should keep at least one item in primary
            KeepAtLeastOneItemInPrimaryList(itemToBeRemoved, true);
            _topDataProvider.MoveItemsOutOfPrimaryList(itemToBeRemoved);
        }

        // measure again to make sure SelectedItem is realized
        desWidth = MeasureTopNavigationViewDesiredWidth(Size.Infinity);

        var widthAtLeastToBeRemoved = desWidth - availableSize.Width;
        if (widthAtLeastToBeRemoved > 0)
        {
            var itemToBeRemoved = FindMovableItemsToBeRemovedFromPrimaryList(widthAtLeastToBeRemoved, new int[] { selItemIndex });

            // At least one item is kept on primary list
            KeepAtLeastOneItemInPrimaryList(itemToBeRemoved, false);

            _topDataProvider.MoveItemsOutOfPrimaryList(itemToBeRemoved);
        }
    }

    private IList<int> FindMovableItemsRecoverToPrimaryList(double availableWidth, IList<int> includeItems)
    {
        List<int> toBeMoved = new List<int>(includeItems.Count + 4);
        var size = _topDataProvider.Size;

        // Included Items take high priority, all of them are included in recovery list
        for (int index = 0; index < includeItems.Count; index++)
        {
            toBeMoved.Add(includeItems[index]);
            availableWidth -= _topDataProvider.GetWidthForItem(includeItems[index]);
        }

        int i = 0;
        while (i < size && availableWidth > 0)
        {
            if (!_topDataProvider.IsItemInPrimaryList(i) && !includeItems.Contains(i))
            {
                var wid = _topDataProvider.GetWidthForItem(i);
                if (availableWidth >= wid)
                {
                    toBeMoved.Add(i);
                    availableWidth -= wid;
                }
                else
                {
                    break;
                }
            }
            i++;
        }

        // Keep at one item is not in primary list. Two possible reason: 
        //  1, Most likely it's caused by m_topNavigationRecoveryGracePeriod
        //  2, virtualization and it doesn't have cached width
        if (i == size && !(toBeMoved.Count == 0))
        {
            toBeMoved.RemoveAt(toBeMoved.Count - 1);
        }

        return toBeMoved;
    }

    private IList<int> FindMovableItemsToBeRemovedFromPrimaryList(double widthAtLeastToBeRemoved, IList<int> excludeItems)
    {
        List<int> toBeMoved = new List<int>();
        int i = _topDataProvider.Size - 1;
        while (i >= 0 && widthAtLeastToBeRemoved > 0)
        {
            if (_topDataProvider.IsItemInPrimaryList(i))
            {
                if (!excludeItems.Contains(i))
                {
                    toBeMoved.Add(i);
                    widthAtLeastToBeRemoved -= _topDataProvider.GetWidthForItem(i);
                }
            }
            i--;
        }

        return toBeMoved;
    }

    private IList<int> FindMovableItemsBeyondAvailableWidth(double availableWidth)
    {
        List<int> toBeMoved = new List<int>();
        if (_topNavRepeater != null)
        {
            int selItemIndexInPrimary = _topDataProvider.IndexOf(SelectedItem, NavigationViewSplitVectorID.PrimaryList);
            int size = _topDataProvider.PrimaryListSize;

            double requiredWidth = 0;

            for (int i = 0; i < size; i++)
            {
                if (i != selItemIndexInPrimary)
                {
                    bool shouldMove = true;
                    if (requiredWidth <= availableWidth)
                    {
                        var cont = _topNavRepeater.TryGetElement(i);
                        if (cont != null)
                        {
                            requiredWidth += cont.DesiredSize.Width;
                            shouldMove = requiredWidth > availableWidth;
                        }
                        else
                        {
                            // item in virtualized but not realized
                        }
                    }

                    if (shouldMove)
                    {
                        toBeMoved.Add(i);
                    }
                }
            }
        }

        return _topDataProvider.ConvertPrimaryIndexToIndex(toBeMoved);
    }

    private void KeepAtLeastOneItemInPrimaryList(IList<int> itemInPrimaryToBeRemoved, bool shouldKeepFirst)
    {
        if (itemInPrimaryToBeRemoved.Count > 0 && itemInPrimaryToBeRemoved.Count == _topDataProvider.PrimaryListSize)
        {
            if (shouldKeepFirst)
            {
                itemInPrimaryToBeRemoved.RemoveAt(0);
            }
            else
            {
                itemInPrimaryToBeRemoved.RemoveAt(itemInPrimaryToBeRemoved.Count - 1);
            }
        }
    }

    private void UpdateTopNavigationWidthCache()
    {
        var size = _topDataProvider.PrimaryListSize;
        if (_topNavRepeater != null)
        {
            for (int i = 0; i < size; i++)
            {
                if (_topNavRepeater.TryGetElement(i) is Control c)
                {
                    _topDataProvider.UpdateWidthForPrimaryItem(i, c.DesiredSize.Width);
                }
                else
                {
                    break;
                }
            }
        }
    }





    ////////////////////////////////////////
    //////// SPLITVIEW RELATED ////////////
    //////////////////////////////////////

    private void OnSplitViewClosedCompactChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == SplitView.IsPaneOpenProperty ||
            args.Property == SplitView.DisplayModeProperty)
        {
            UpdateIsClosedCompact();
        }
    }

    private void OnSplitViewPaneClosed(object sender, RoutedEventArgs e)
    {
        if (e.Source != _splitView)
            return;

        PaneClosed?.Invoke(this, EventArgs.Empty);
    }

    private void OnSplitViewPaneClosing(object sender, CancelRoutedEventArgs e)
    {
        if (e.Source != _splitView)
            return;

        bool pendingCancel = false;

        if (!_blockNextClosingEvent)
        {
            var ea = new NavigationViewPaneClosingEventArgs();
            ea.SplitViewClosingArgs = e;
            PaneClosing?.Invoke(this, ea);
            pendingCancel = ea.Cancel;
        }
        else
        {
            _blockNextClosingEvent = false;
        }

        if (!pendingCancel)
        {
            if (_splitView != null && _leftNavRepeater != null)
            {
                if (_splitView.DisplayMode == SplitViewDisplayMode.CompactInline ||
                    _splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    PseudoClasses.Set(s_pcListSizeCompact, true);
                    UpdatePaneToggleSize();
                }
                else
                {
                    PseudoClasses.Set(s_pcListSizeCompact, false);
                }
            }
        }
    }

    private void OnSplitViewPaneOpened(object sender, RoutedEventArgs e)
    {
        if (e.Source != _splitView)
            return;
        
        PaneOpened?.Invoke(this, EventArgs.Empty);
    }

    private void OnSplitViewPaneOpening(object sender, RoutedEventArgs e)
    {
        if (e.Source != _splitView)
            return;

        if (_leftNavRepeater != null)
        {
            PseudoClasses.Set(s_pcListSizeCompact, false);
        }

        PaneOpening?.Invoke(this, EventArgs.Empty);
    }

    
    

    ///////////////////////////////////
    //////// PANE BUTTONS ////////////
    /////////////////////////////////

    private void OnPaneToggleButtonClick(object sender, RoutedEventArgs e)
    {
        if (IsPaneOpen)
        {
            _wasForceClosed = true;
            ClosePane();
        }
        else
        {
            _wasForceClosed = false;
            OpenPane();
        }
    }

    private void OnPaneSearchButtonClick(object sender, RoutedEventArgs e)
    {
        _wasForceClosed = false;
        OpenPane();

        if (AutoCompleteBox != null)
        {
            AutoCompleteBox.Focus(NavigationMethod.Tab);
        }
    }

    private void OnBackButtonClicked(object sender, RoutedEventArgs e)
    {
        var ea = new NavigationViewBackRequestedEventArgs();
        BackRequested?.Invoke(this, ea);
        BackCommand?.CanExecute(null);
    }

    private void UpdatePaneButtonWidths()
    {
        TemplateSettings.PaneToggleButtonWidth = CompactPaneLength;
        TemplateSettings.SmallerPaneToggleButtonWidth = CompactPaneLength - 8;
    }

    private void UpdatePaneToggleButtonVisibility()
    {
        TemplateSettings.PaneToggleButtonVisibility = IsPaneToggleButtonVisible && !IsTopNavigationView;
    }

    private void UpdatePaneToggleSize()
    {
        if (_splitView != null)
        {
            double width = TemplateSettings.PaneToggleButtonWidth;
            double toggleWidth = width;

            if (ShouldShowBackButton && _splitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                width += _backButton?.Width ?? _backButtonWidth;
            }

            if (!_isClosedCompact && !string.IsNullOrEmpty(PaneTitle))
            {
                if (_splitView.DisplayMode == SplitViewDisplayMode.Overlay && IsPaneOpen)
                {
                    width = OpenPaneLength;
                    toggleWidth = OpenPaneLength - ((ShouldShowBackButton || ShouldShowCloseButton) ? _backButtonWidth : 0);
                }
                else if (!(_splitView.DisplayMode == SplitViewDisplayMode.Overlay && !IsPaneOpen))
                {
                    width = OpenPaneLength;
                    toggleWidth = OpenPaneLength;
                }
            }

            _paneToggleButton?.Width = toggleWidth;
        }
    }

    private void UpdateBackAndCloseButtonsVisibility()
    {
        if (!_appliedTemplate)
            return;

        bool showBack = ShouldShowBackButton;
        var vsdm = GetVisualStateDisplayMode(DisplayMode);
        bool useLeftPadding =
            (vsdm == NavigationViewVisualStateDisplayMode.Minimal && !IsTopNavigationView) ||
            vsdm == NavigationViewVisualStateDisplayMode.MinimalWithBackButton;
        double leftPadding = 0;
        double paneHeaderPaddingForToggle = 0;
        double paneHeaderPaddingForClose = 0;
        double paneHeaderContentBorderRowMinHeight = 0;

        TemplateSettings.BackButtonVisibility = showBack;

        if (_paneToggleButton != null && IsPaneToggleButtonVisible)
        {
            paneHeaderContentBorderRowMinHeight = GetPaneToggleButtonHeight();
            paneHeaderPaddingForToggle = GetPaneToggleButtonWidth();

            if (useLeftPadding)
            {
                leftPadding = paneHeaderPaddingForToggle;
            }
        }

        if (_backButton != null)
        {
            if (useLeftPadding && showBack)
            {
                leftPadding += _backButton.Width;
            }
        }

        if (_closeButton != null)
        {
            _closeButton.IsVisible = ShouldShowCloseButton;

            if (ShouldShowCloseButton)
            {
                paneHeaderContentBorderRowMinHeight = Math.Max(paneHeaderContentBorderRowMinHeight, _closeButton.Height);
                if (useLeftPadding)
                {
                    paneHeaderPaddingForClose = _closeButton.Width;
                    leftPadding += paneHeaderPaddingForClose;
                }
            }
        }

        if (_contentLeftPadding != null)
        {
            _contentLeftPadding.Width = leftPadding;
        }

        if (_paneHeaderToggleButtonColumn != null)
        {
            // Account for the PaneToggleButton's width in the PaneHeader's placement.
            _paneHeaderToggleButtonColumn.Width = new GridLength(paneHeaderPaddingForToggle);
        }

        if (_paneHeaderCloseButtonColumn != null)
        {
            _paneHeaderCloseButtonColumn.Width = new GridLength(paneHeaderPaddingForClose);
        }

        if (_paneTitleHolderFrameworkElement != null)
        {
            if (paneHeaderContentBorderRowMinHeight == 0 && _paneTitleHolderFrameworkElement.IsVisible)
            {
                // Handling the case where the PaneTottleButton is collapsed and the PaneTitle's height needs to push the rest of the NavigationView's UI down.
                paneHeaderContentBorderRowMinHeight = _paneTitleHolderFrameworkElement.Bounds.Height;
            }
        }

        if (_paneHeaderContentBorderRow != null)
        {
            _paneHeaderContentBorderRow.MinHeight = paneHeaderContentBorderRowMinHeight;
        }

        if (_paneContentGrid != null)
        {
            if (_paneContentGrid.RowDefinitions.Count >= _backButtonRowDefinition)
            {
                int backButtonRowHeight = 0;
                if (!IsOverlay && showBack)
                {
                    backButtonRowHeight = _backButtonHeight;
                }
                else if (_backButton == null)
                { 
                    backButtonRowHeight = c_toggleButtonHeightWithNoBackButton;
                }

                _paneContentGrid.RowDefinitions[_backButtonRowDefinition].Height = new GridLength(backButtonRowHeight);
            }
        }

        PseudoClasses.Set(s_pcBackButtonCollapsed, !showBack);
        UpdateTitleBarPadding();
    }





    /////////////////////////////////////////////////////////
    //////// DISPLAYMODE & VISUAL STATE RELATED ////////////
    ///////////////////////////////////////////////////////

    private void SetDisplayMode(NavigationViewDisplayMode dMode, bool forceSetDisplayMode = false)
    {
        // Need to keep the VisualStateGroup "DisplayModeGroup" updated even if the actual
        // display mode is not changed. This is due to the fact that there can be a transition between
        // 'Minimal' and 'MinimalWithBackButton'.
        UpdateVisualStateForDisplayModeGroup(dMode);

        if (forceSetDisplayMode || DisplayMode != dMode)
        {
            // Update header visibility based on what the new display mode will be
            UpdateHeaderVisibility(dMode);

            UpdatePaneTabFocusNavigation();

            UpdatePaneToggleSize();

            RaiseDisplayModeChanged(dMode);
        }
    }

    // To support TopNavigationView, DisplayModeGroup in visualstate(We call it VisualStateDisplayMode) is decoupled with DisplayMode.
    // The VisualStateDisplayMode is the combination of TopNavigationView, DisplayMode, PaneDisplayMode.
    // Here is the mapping:
    //    TopNav -> Minimal
    //    PaneDisplayMode::Left || (PaneDisplayMode::Auto && DisplayMode::Expanded) -> Expanded
    //    PaneDisplayMode::LeftCompact || (PaneDisplayMode::Auto && DisplayMode::Compact) -> Compact
    //    Map others to Minimal or MinimalWithBackButton 
    private NavigationViewVisualStateDisplayMode GetVisualStateDisplayMode(NavigationViewDisplayMode dMode)
    {
        var pdm = PaneDisplayMode;

        if (IsTopNavigationView)
            return NavigationViewVisualStateDisplayMode.Minimal;

        if (pdm == NavigationViewPaneDisplayMode.Left ||
            (pdm == NavigationViewPaneDisplayMode.Auto && dMode == NavigationViewDisplayMode.Expanded))
        {
            return NavigationViewVisualStateDisplayMode.Expanded;
        }

        if (pdm == NavigationViewPaneDisplayMode.LeftCompact ||
            (pdm == NavigationViewPaneDisplayMode.Auto && dMode == NavigationViewDisplayMode.Compact))
        {
            return NavigationViewVisualStateDisplayMode.Compact;
        }

        // In minimal mode, when the NavView is closed, the HeaderContent doesn't have
        // its own dedicated space, and must 'share' the top of the NavView with the 
        // pane toggle button ('hamburger' button) and the back button.
        // When the NavView is open, the close button is taking space instead of the back button.
        if (ShouldShowBackButton || ShouldShowCloseButton)
        {
            return NavigationViewVisualStateDisplayMode.MinimalWithBackButton;
        }
        else
        {
            return NavigationViewVisualStateDisplayMode.Minimal;
        }
    }

    private void UpdateVisualStateForDisplayModeGroup(NavigationViewDisplayMode dMode)
    {
        if (_splitView == null)
            return;

        var vsdm = GetVisualStateDisplayMode(dMode);
        var svdm = SplitViewDisplayMode.Overlay;

        switch (vsdm)
        {
            case NavigationViewVisualStateDisplayMode.MinimalWithBackButton:
                PseudoClasses.Set(s_pcMinimalWithBack, true);
                PseudoClasses.Set(s_pcMinimal, false);
                PseudoClasses.Set(s_pcTopNavMinimal, false);
                PseudoClasses.Set(s_pcCompact, false);
                PseudoClasses.Set(s_pcExpanded, false);
                svdm = SplitViewDisplayMode.Overlay;
                break;

            case NavigationViewVisualStateDisplayMode.Minimal:
                PseudoClasses.Set(s_pcMinimalWithBack, false);
                PseudoClasses.Set(s_pcMinimal, true);
                PseudoClasses.Set(s_pcTopNavMinimal, false);
                PseudoClasses.Set(s_pcCompact, false);
                PseudoClasses.Set(s_pcExpanded, false);
                svdm = SplitViewDisplayMode.Overlay;
                break;

            case NavigationViewVisualStateDisplayMode.Compact:
                PseudoClasses.Set(s_pcMinimalWithBack, false);
                PseudoClasses.Set(s_pcMinimal, false);
                PseudoClasses.Set(s_pcTopNavMinimal, false);
                PseudoClasses.Set(s_pcCompact, true);
                PseudoClasses.Set(s_pcExpanded, false);
                svdm = SplitViewDisplayMode.CompactOverlay;
                break;

            case NavigationViewVisualStateDisplayMode.Expanded:
                PseudoClasses.Set(s_pcMinimalWithBack, false);
                PseudoClasses.Set(s_pcMinimal, false);
                PseudoClasses.Set(s_pcTopNavMinimal, false);
                PseudoClasses.Set(s_pcCompact, false);
                PseudoClasses.Set(s_pcExpanded, true);
                svdm = SplitViewDisplayMode.CompactInline;
                break;
        }

        // When the pane is made invisible we need to collapse the pane part of the SplitView
        if (!IsPaneVisible)
        {
            svdm = SplitViewDisplayMode.CompactOverlay;
        }

        if (IsTopNavigationView)
        {
            PseudoClasses.Set(s_pcMinimalWithBack, false);
            PseudoClasses.Set(s_pcMinimal, false);
            PseudoClasses.Set(s_pcTopNavMinimal, true);
            PseudoClasses.Set(s_pcCompact, false);
            PseudoClasses.Set(s_pcExpanded, false);
        }

        // Updating the splitview 'DisplayMode' property in some diplaymodes causes children to be added to the popup root.
        // This causes an exception if the NavigationView is in the popup root itself (as SplitView is trying to add children to the tree while it is being measured).
        // Due to this, we want to defer updating this property for all calls coming from `OnApplyTemplate`to the OnLoaded function.
        if (_fromOnApplyTemplate)
        {
            _updateVisualStateForDisplayModeFromOnLoaded = true;
        }
        else
        {
            _splitView.DisplayMode = svdm;
        }
    }

    private void UpdateVisualState()
    {
        if (!_appliedTemplate)
            return;

        PseudoClasses.Set(s_pcAutoSuggestCollapsed, AutoCompleteBox == null);
        PseudoClasses.Set(s_pcSettingsCollapsed, IsSettingsVisible);

        if (IsTopNavigationView)
        {
            //UpdateVisualStateForOverflowButton(); [Discontinued]
        }
        else
        {
            //UpdateLeftNavigationOnlyVisualState(); [Zero point in having a dedicated method for this]
            PseudoClasses.Set(s_pcPaneToggleCollapsed, !IsPaneToggleButtonVisible || _isLeftPaneTitleEmpty);
        }
    }

    private void RaiseDisplayModeChanged(NavigationViewDisplayMode mode)
    {
        DisplayMode = mode;
        var ea = new NavigationViewDisplayModeChangedEventArgs(mode);
        DisplayModeChanged?.Invoke(this, ea);
    }

    private void UpdatePaneOverlayGroup()
    {
        if (_splitView != null)
        {
            if (IsPaneOpen && (_splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay ||
                _splitView.DisplayMode == SplitViewDisplayMode.Overlay))
            {
                PseudoClasses.Set(s_pcPaneNotOverlaying, false);
            }
            else
            {
                //PaneNotOverlaying VisualState
                PseudoClasses.Set(s_pcPaneNotOverlaying, true);
            }
        }
    }




    //////////////////////////////////////////
    //////// SELECTION INDICATOR ////////////
    ////////////////////////////////////////

    private void AnimateSelectionChangedToItem(object selItem)
    {
        if (selItem != null && !IsSelectionSuppressed(selItem))
        {
            AnimateSelectionChanged(selItem);
        }
    }

    // Please clear the field m_lastSelectedItemPendingAnimationInTopNav when calling this method to prevent garbage value and incorrect animation
    // when the layout is invalidated as it's called in OnLayoutUpdated.
    private void AnimateSelectionChanged(object nextItem)
    {
        // If we are delaying animation due to item movement in top nav overflow or
        // the template is not applied, dont do anything
        if (_lastSelectedItemPendingAnimationInTopNav != null || !_appliedTemplate)
            return;

        var prevIndicator = _activeIndicator;
        var nextIndicator = FindSelectionIndicator(nextItem);

        // It seems we can sometimes have this called before an NVI is fully loaded and the
        // SelectionIndicator isn't available - so add callback to try to get it in the future
        // Can be seen in SampleApp where selection indicator won't load first time
        // This is probably an issue somewhere else and this control needs a review for WinUI 1.5
        // so this is a temporary fix until I get around to comparing the code
        if (_activeIndicator == null && nextItem != null && nextIndicator == null)
        {
            Dispatcher.UIThread.Post(() => AnimateSelectionChanged(nextItem));
        }

        bool haveValidAnimation = false;
        // It's possible that AnimateSelectionChanged is called multiple times before the first animation is complete.
        // To have better user experience, if the selected target is the same, keep the first animation
        // If the selected target is not the same, abort the first animation and launch another animation.
        if (_prevIndicator != null || _nextIndicator != null)
        {
            if (nextIndicator != null && _nextIndicator == nextIndicator)
            {
                if (prevIndicator != null && _prevIndicator == null)
                {
                    ResetElementAnimationProperties(prevIndicator, 0f);
                }
                haveValidAnimation = true;
            }
            else
            {
                // If the last animation is still playing, force it to complete.
                OnAnimationComplete();
            }
        }

        if (!haveValidAnimation)
        {
            var paneContentGrid = _paneContentGrid;

            if ((prevIndicator != nextIndicator) && paneContentGrid != null && prevIndicator != null && 
                nextIndicator != null && FAUISettings.AreAnimationsEnabled())
            {
                // Make sure both indicators are visible and in their original locations
                ResetElementAnimationProperties(prevIndicator, 1f);
                ResetElementAnimationProperties(nextIndicator, 1f);

                // get the item positions in the pane
                Point point = default;
                double prevPos, nextPos;

                var t1 = prevIndicator.TransformToVisual(_paneContentGrid) ?? Matrix.Identity;
                var t2 = nextIndicator.TransformToVisual(_paneContentGrid) ?? Matrix.Identity;
                Point prevPosPoint = t1.Transform(point);
                Point nextPosPoint = t2.Transform(point);
                Size prevSize = prevIndicator.Bounds.Size;
                Size nextSize = nextIndicator.Bounds.Size;

                bool areElementsAtSameDepth = false;
                if (IsTopNavigationView)
                {
                    prevPos = prevPosPoint.X;
                    nextPos = nextPosPoint.X;
                    areElementsAtSameDepth = prevPosPoint.Y == nextPosPoint.Y;
                }
                else
                {
                    prevPos = prevPosPoint.Y;
                    nextPos = nextPosPoint.Y;
                    areElementsAtSameDepth = prevPosPoint.X == nextPosPoint.X;
                }

                var visual = ElementComposition.GetElementVisual(this);
                // CreateScopedBatch

                if (!areElementsAtSameDepth)
                {
                    bool isNextBelow = prevPosPoint.Y < nextPosPoint.Y;
                    if (prevIndicator.Bounds.Height > prevIndicator.Bounds.Width)
                    {
                        PlayIndicatorNonSameLevelAnimations(prevIndicator, true, isNextBelow ? false : true);
                    }
                    else
                    {
                        PlayIndicatorNonSameLevelTopPrimaryAnimation(prevIndicator, true);
                    }

                    if (nextIndicator.Bounds.Height > nextIndicator.Bounds.Width)
                    {
                        PlayIndicatorNonSameLevelAnimations(nextIndicator, false, isNextBelow ? true : false);
                    }
                    else
                    {
                        PlayIndicatorNonSameLevelTopPrimaryAnimation(nextIndicator, false);
                    }
                }
                else
                {
                    double outgoingEndPosition = nextPos - prevPos;
                    double incomingStartPosition = prevPos - nextPos;

                    // Play the animation on both the previous and next indicators
                    PlayIndicatorAnimations(prevIndicator, 0, outgoingEndPosition, prevSize, nextSize, true);
                    PlayIndicatorAnimations(nextIndicator, incomingStartPosition, 0, prevSize, nextSize, false);
                }

                // End Scoped Batch
                _prevIndicator = prevIndicator;
                _nextIndicator = nextIndicator;

                // On ScopedBatch Completed:
                // OnAnimationComplete();
                // The animation is 600ms, we'll set our callback to just above that
                DispatcherTimer.RunOnce(OnAnimationComplete, TimeSpan.FromMilliseconds(700), DispatcherPriority.Render);
            }
            else if (prevIndicator != nextIndicator)
            {
                // if all else fails, or if animations are turned off, attempt to correctly set the positions and opacities of the indicators.
                ResetElementAnimationProperties(prevIndicator, 0f);
                ResetElementAnimationProperties(nextIndicator, 1f);
            }

            _activeIndicator = nextIndicator;
        }
    }

    private void PlayIndicatorNonSameLevelAnimations(Control indicator, bool isOutgoing, bool fromTop)
    {
        var visual = ElementComposition.GetElementVisual(indicator);
        if (visual == null)
            return;
        var comp = visual.Compositor;

        // Determine scaling of indicator (whether it is appearing or dissapearing)
        double beginScale = isOutgoing ? 1 : 0;
        double endScale = isOutgoing ? 0 : 1;

        var scaleAnim = comp.CreateVector3DKeyFrameAnimation();
        scaleAnim.InsertKeyFrame(0f, new Vector3D(1, beginScale, 1));
        scaleAnim.InsertKeyFrame(1f, new Vector3D(1, endScale, 1));
        scaleAnim.Duration = TimeSpan.FromMilliseconds(600);

        // Determine where the indicator is animating from/to
        var size = indicator.Bounds.Size;
        var dimension = IsTopNavigationView ? size.Width : size.Height;
        var newCenter = fromTop ? 0 : dimension;
        var indicatorCenterPoint = visual.CenterPoint;

        indicatorCenterPoint = new Vector3D(indicatorCenterPoint.X, newCenter, indicatorCenterPoint.Z);
        visual.CenterPoint = indicatorCenterPoint;

        visual.StartAnimation("Scale", scaleAnim);

        // HACK: Note we don't need this opacity animation, but if we leave it off the scale animation
        // may not trigger the first time if
        // - Open parent item without selecting it
        // - Select child item
        // - Close parent
        // - Reopen Parent
        // After the first time, it runs fine. Having another animation kicks everything and makes it 
        // work...so...yeah... there's issues with the composition animation system...will they get fixed
        // who knows. Maybe one day, if we're lucky...
        if (isOutgoing)
        {
            var opacityAnim = comp.CreateScalarKeyFrameAnimation();
            opacityAnim.InsertKeyFrame(0.0f, 1.0f);
            opacityAnim.Duration = TimeSpan.FromMilliseconds(600);

            visual.StartAnimation("Opacity", opacityAnim);
        }
    }

    private void PlayIndicatorNonSameLevelTopPrimaryAnimation(Control indicator, bool isOutgoing)
    {
        var visual = ElementComposition.GetElementVisual(indicator);
        if (visual == null)
            return;
        var comp = visual.Compositor;

        // Determine scaling of indicator (whether it is appearing or dissapearing)
        double beginScale = isOutgoing ? 1 : 0;
        double endScale = isOutgoing ? 0 : 1;

        var scaleAnim = comp.CreateVector3DKeyFrameAnimation();
        scaleAnim.InsertKeyFrame(0, new Vector3D(beginScale, visual.Scale.Y, visual.Scale.Z));
        scaleAnim.InsertKeyFrame(1, new Vector3D(endScale, visual.Scale.Y, visual.Scale.Z));
        scaleAnim.Duration = TimeSpan.FromMilliseconds(600);

        var size = indicator.Bounds.Size;
        var newCenter = size.Width / 2;
        var indicatorCenterPoint = visual.CenterPoint;
        indicatorCenterPoint = new Vector3D(indicatorCenterPoint.X, newCenter, indicatorCenterPoint.Z);
        visual.CenterPoint = indicatorCenterPoint;

        visual.StartAnimation("Scale", scaleAnim);
    }

    private void PlayIndicatorAnimations(Control indicator, double from, double to, Size beginSize, Size endSize, bool isOutgoing)
    {
        var visual = ElementComposition.GetElementVisual(indicator);
        if (visual == null)
            return;
        var comp = visual.Compositor;

        Size size = indicator.Bounds.Size;
        double dimension = IsTopNavigationView ? size.Width : size.Height;

        double beginScale = 1f;
        double endScale = 1f;
        if (IsTopNavigationView && Math.Abs(size.Width) > 0.001)
        {
            beginScale = beginSize.Width / size.Width;
            endScale = endSize.Width / size.Width;
        }

        // StepEasingFunction

        //winrt::float2 c_frame1point1 = winrt::float2(0.9f, 0.1f);
        //winrt::float2 c_frame1point2 = winrt::float2(1.0f, 0.2f);
        //winrt::float2 c_frame2point1 = winrt::float2(0.1f, 0.9f);
        //winrt::float2 c_frame2point2 = winrt::float2(0.2f, 1.0f);
        var easing1 = new SplineEasing(0.9, 0.1, 1, 0.2);
        var easing2 = new SplineEasing(0.1, 0.9, 0.2, 1.0);
        var step = new StepEasingFunction { Steps = 5 };
        if (isOutgoing)
        {
            // fade the outgoing indicator so it looks nice when animating over the scroll area
            var opacityAnim = comp.CreateScalarKeyFrameAnimation();
            opacityAnim.InsertKeyFrame(0.0f, 1.0f);
            opacityAnim.InsertKeyFrame(0.333f, 1.0f, step);
            opacityAnim.InsertKeyFrame(1.0f, 0.0f, easing2);
            opacityAnim.Duration = TimeSpan.FromMilliseconds(600);

            visual.StartAnimation("Opacity", opacityAnim);
        }

        // TODO: If Avalonia ever supports Animation targets like "Offset.X" "Scale.Y", then this can be consolidated
        // and more closely match WinUI. For now though, we need to duplicate code...
        if (!IsTopNavigationView)
        {
            var posAnim = comp.CreateVector3DKeyFrameAnimation();
            posAnim.InsertKeyFrame(0.0f, new Vector3D(visual.Offset.X, from < to ? from : (from + (dimension * (beginScale - 1))), visual.Offset.Z));
            posAnim.InsertKeyFrame(0.333f, new Vector3D(visual.Offset.X, from < to ? (to + (dimension * (endScale - 1))) : to, visual.Offset.Z), step);
            posAnim.Duration = TimeSpan.FromMilliseconds(600);

            var scaleAnim = comp.CreateVector3DKeyFrameAnimation();
            scaleAnim.InsertKeyFrame(0.0f, new Vector3D(1, beginScale, 1));
            scaleAnim.InsertKeyFrame(0.333f, new Vector3D(1, Math.Abs(to - from) / dimension + (from < to ? endScale : beginScale), 1), easing1);
            scaleAnim.InsertKeyFrame(1.0f, new Vector3D(1, endScale, endScale), easing2);
            scaleAnim.Duration = TimeSpan.FromMilliseconds(600);

            var centerAnim = comp.CreateVector3DKeyFrameAnimation();
            centerAnim.InsertKeyFrame(0.0f, new Vector3D(visual.CenterPoint.X, from < to ? 0.0f : dimension, visual.CenterPoint.Z));
            centerAnim.InsertKeyFrame(1.0f, new Vector3D(visual.CenterPoint.X, from < to ? dimension : 0.0f, visual.CenterPoint.Z), step);
            centerAnim.Duration = TimeSpan.FromMilliseconds(200);

            visual.StartAnimation("Offset", posAnim);
            visual.StartAnimation("Scale", scaleAnim);
            visual.StartAnimation("CenterPoint", centerAnim);
        }
        else
        {
            var posAnim = comp.CreateVector3DKeyFrameAnimation();
            posAnim.InsertKeyFrame(0.0f, new Vector3D(from < to ? from : (from + (dimension * (beginScale - 1))), visual.Offset.Y, visual.Offset.Z));
            posAnim.InsertKeyFrame(0.333f, new Vector3D(from < to ? (to + (dimension * (endScale - 1))) : to, visual.Offset.Y, visual.Offset.Z), step);
            posAnim.Duration = TimeSpan.FromMilliseconds(600);

            var scaleAnim = comp.CreateVector3DKeyFrameAnimation();
            scaleAnim.InsertKeyFrame(0.0f, new Vector3D(beginScale, 1, 1));
            scaleAnim.InsertKeyFrame(0.333f, new Vector3D(Math.Abs(to - from) / dimension + (from < to ? endScale : beginScale), 1, 1), easing1);
            scaleAnim.InsertKeyFrame(1.0f, new Vector3D(1, endScale, endScale), easing2);
            scaleAnim.Duration = TimeSpan.FromMilliseconds(600);

            var centerAnim = comp.CreateVector3DKeyFrameAnimation();
            centerAnim.InsertKeyFrame(0.0f, new Vector3D(from < to ? 0.0f : dimension, visual.CenterPoint.Y, visual.CenterPoint.Z));
            centerAnim.InsertKeyFrame(1.0f, new Vector3D(from < to ? dimension : 0.0f, visual.CenterPoint.Y, visual.CenterPoint.Z), step);
            centerAnim.Duration = TimeSpan.FromMilliseconds(200);

            visual.StartAnimation("Offset", posAnim);
            visual.StartAnimation("Scale", scaleAnim);
            visual.StartAnimation("CenterPoint", centerAnim);
        }
    }

    private void OnAnimationComplete()
    {
        var indicator = _prevIndicator;
        ResetElementAnimationProperties(indicator, 0f);
        _prevIndicator = null;

        indicator = _nextIndicator;
        ResetElementAnimationProperties(indicator, 1);
        _nextIndicator = null;
    }

    private void ResetElementAnimationProperties(Control element, double desiredOpacity)
    {
        if (element != null)
        {
            element.Opacity = desiredOpacity;
            if (ElementComposition.GetElementVisual(element) is CompositionVisual cv)
            {
                cv.Offset = new Vector3D(0, 0, 0);
                cv.Scale = new Vector3D(1, 1, 1);
                cv.Opacity = (float)desiredOpacity;
            }
        }
    }
        
    private Control FindSelectionIndicator(object item)
    {
        if (item != null)
        {
            var cont = NavigationViewItemOrSettingsContentFromData(item);
            if (cont != null)
            {
                var indicator = cont.SelectionIndicator;
                if (indicator != null)
                {
                    return indicator;
                }
                else
                {
                    cont.UpdateLayout();
                    //cont.ApplyTemplate();
                    return cont.SelectionIndicator;
                }
            }
        }

        return null;
    }

    private NavigationViewItem FindLowestLevelContainerToDisplaySelectionIndicator()
    {
        var indexIntoIndex = 1;
        var selIndex = _selectionModel.SelectedIndex;
        if (selIndex != IndexPath.Unselected && selIndex.GetSize() > 1)
        {
            var cont = GetContainerForIndex(selIndex.GetAt(indexIntoIndex), selIndex.GetAt(0) == _footerMenuBlockIndex);
            if (cont is NavigationViewItem nvi)
            {
                bool isRepVis = nvi.IsRepeaterVisible;
                while (nvi != null && isRepVis && !nvi.IsSelected && nvi.IsChildSelected)
                {
                    indexIntoIndex++;
                    if (indexIntoIndex >= selIndex.GetSize()) { break; }

                    isRepVis = false;
                    if (nvi.GetRepeater != null)
                    {
                        if (nvi.GetRepeater.TryGetElement(selIndex.GetAt(indexIntoIndex)) is NavigationViewItem childNVI)
                        {
                            nvi = childNVI;
                            isRepVis = nvi.IsRepeaterVisible;
                        }
                        else
                        {
                            nvi = null;
                        }
                    }
                }
                return nvi;
            }
        }

        return null;
    }



    ////////////////////////////
    //////// OTHER ////////////
    //////////////////////////

    private void CreateAndHookEventsToSettings()
    {
        if (_settingsItem == null)
            return;

        _settingsItem.IconSource = _settingsIconSource;

        var localizedSettingsName = LocalizationService.Instance.GetString(SR_SettingsButtonName);

        // _settingsItem.Tag = localizedSettingsName;
        _settingsItem.Tag = "Settings";
        UpdateSettingsItemToolTip();

        // Add the name only in case of horizontal nav
        if (!IsTopNavigationView)
        {
            _settingsItem.Content = localizedSettingsName;
        }
        else
        {
            _settingsItem.Content = null;
        }

        SettingsItem = _settingsItem;
    }

    private void UpdateIsClosedCompact()
    {
        if (_splitView == null)
            return;

        var svdm = _splitView.DisplayMode;
        _isClosedCompact = !_splitView.IsPaneOpen && (svdm == SplitViewDisplayMode.CompactInline ||
            svdm == SplitViewDisplayMode.CompactOverlay);

        PseudoClasses.Set(s_pcClosedCompact, _isClosedCompact);
        //PseudoClasses.Set(":notclosedcompact", !_isClosedCompact); (default)


        // _initialListSizeStateSet = true;

        PseudoClasses.Set(s_pcListSizeCompact, _isClosedCompact);
        //PseudoClasses.Set(":listsizefull", !_isClosedCompact); (default)


        UpdateTitleBarPadding();
        UpdateBackAndCloseButtonsVisibility();
        // UpdatePaneTitleMargins();
        UpdatePaneToggleSize();
    }

    private void UpdateSettingsItemToolTip()
    {
        if (_settingsItem != null)
        {
            if (!IsTopNavigationView && IsPaneOpen)
            {
                ToolTip.SetTip(_settingsItem, null);
            }
            else
            {
                ToolTip.SetTip(_settingsItem, LocalizationService.Instance.GetString(SR_SettingsButtonName));
            }
        }
    }

    private void UpdatePaneTitleFrameworkElementParents()
    {
        if (_paneTitleHolderFrameworkElement == null)
            return;

        bool isPaneTBVis = IsPaneToggleButtonVisible;
        bool isTopNav = IsTopNavigationView;

        _isLeftPaneTitleEmpty = (isPaneTBVis ||
            isTopNav ||
            string.IsNullOrEmpty(PaneTitle) ||
            (PaneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal && !IsPaneOpen));

        _paneTitleHolderFrameworkElement.IsVisible = _isLeftPaneTitleEmpty ? false : true;

        if (_paneTitleFrameworkElement != null)
        {
            var first = SetPaneTitleFrameworkElementParent(_paneToggleButton, _paneTitleFrameworkElement, isTopNav || !isPaneTBVis);
            var second = SetPaneTitleFrameworkElementParent(_paneTitlePresenter, _paneTitleFrameworkElement, isTopNav || isPaneTBVis);
            var third = SetPaneTitleFrameworkElementParent(_paneTitleOnTopPane, _paneTitleFrameworkElement, !isTopNav || isPaneTBVis);

            if (first != null)
            {
                first();
                _paneTitleOnTopPane.IsVisible = false;
            }
            else if (second != null)
            {
                second();
                _paneTitleOnTopPane.IsVisible = false;
            }
            else if (third != null)
            {
                third();

                if (_paneTitleOnTopPane != null)
                    _paneTitleOnTopPane.IsVisible = !string.IsNullOrEmpty(PaneTitle) && PaneTitle.Length != 0;
            }
        }
    }

    private Action SetPaneTitleFrameworkElementParent(ContentControl parent, Control paneTitle, bool shouldNotContainPaneTitle)
    {
        if (parent != null)
        {
            if ((parent.Content == paneTitle) == shouldNotContainPaneTitle)
            {
                if (shouldNotContainPaneTitle)
                {
                    parent.Content = null;
                }
                else
                {
                    return () => parent.Content = paneTitle;
                }
            }
        }
        return null;
    }

    private void OnSettingsInvoked()
    {
        if (_settingsItem != null)
        {
            OnNavigationViewItemInvoked(_settingsItem);
        }
    }

    internal void TopNavigationViewItemContentChanged()
    {
        if (_appliedTemplate)
        {
            if (MenuItemsSource == null) // WinUI #5558
            {
                _topDataProvider.InvalidWidthCache();
            }
            InvalidateMeasure();
        }
    }

    private void CloseTopNavigationViewFlyout()
    {
        _topNavOverflowButton?.Flyout?.Hide();
    }

    private void OnFlyoutClosing(object sender, CancelEventArgs args)
    {
        // If the user selected an parent item in the overflow flyout then the item has not been moved to top primary yet.
        // So we need to move it.
        if (_moveTopNavOverflowItemOnFlyoutClose && !_selectionChangeFromOverflowMenu)
        {
            _moveTopNavOverflowItemOnFlyoutClose = false;

            var selIndex = _selectionModel.SelectedIndex;
            if (selIndex.GetSize() > 0)
            {
                if (GetContainerForIndex(selIndex.GetAt(1), false) is NavigationViewItem nvi)
                {
                    // We want to collapse the top level item before we move it
                    nvi.IsExpanded = false;
                }

                SelectAndMoveOverflowItem(SelectedItem, selIndex, false);
            }
        }
    }

    private void UpdatePaneDisplayMode()
    {
        if (!_appliedTemplate)
            return;

        if (!IsTopNavigationView)
        {
            UpdateAdaptiveLayout(Bounds.Width, true);

            SwapPaneHeaderContent(_leftNavPaneHeaderContentBorder, _paneHeaderOnTopPane, PaneHeaderProperty);
            SwapPaneHeaderContent(_leftNavPaneCustomContentBorder, _paneCustomContentOnTopPane, PaneCustomContentProperty);
            SwapPaneHeaderContent(_leftNavFooterContentBorder, _paneFooterOnTopPane, PaneFooterProperty);

            CreateAndHookEventsToSettings();
        }
        else
        {
            ClosePane();
            SetDisplayMode(NavigationViewDisplayMode.Minimal, true);

            SwapPaneHeaderContent(_paneHeaderOnTopPane, _leftNavPaneHeaderContentBorder, PaneHeaderProperty);
            SwapPaneHeaderContent(_paneCustomContentOnTopPane, _leftNavPaneCustomContentBorder, PaneCustomContentProperty);
            SwapPaneHeaderContent(_paneFooterOnTopPane, _leftNavFooterContentBorder, PaneFooterProperty);

            CreateAndHookEventsToSettings();
        }

        UpdateContentBindingsForPaneDisplayMode();
        UpdateRepeaterItemsSource(false);
        UpdateFooterRepeaterItemsSource(false, false);

        if (SelectedItem != null)
        {
            _orientationChangedPendingAnimation = true;
        }
    }

    private void UpdatePaneDisplayMode(NavigationViewPaneDisplayMode oldMode, NavigationViewPaneDisplayMode newMode)
    {
        if (!_appliedTemplate)
            return;

        UpdatePaneDisplayMode();

        // For better user experience, We help customer to Open/Close Pane automatically when we switch between LeftMinimal <-> Left.
        // From other navigation PaneDisplayMode to LeftMinimal, we expect pane is closed.
        // From LeftMinimal to Left, it is expected the pane is open. For other configurations, this seems counterintuitive.
        // See #1702 and #1787

        if (!IsTopNavigationView)
        {
            // In rare cases it is possible to end up in a state where two calls to OnPropertyChanged for PaneDisplayMode can end up on the stack
            // Calls above to UpdatePaneDisplayMode() can result in further property updates.
            // As a result of this reentrancy, we can end up with an incorrect result for IsPaneOpen as the later OnPropertyChanged for PaneDisplayMode
            // will complete during the OnPropertyChanged of the earlier one.
            // To avoid this, we only call OpenPane()/ClosePane() if PaneDisplayMode has not changed.
            
            if (newMode == PaneDisplayMode)
            {
                if (IsPaneOpen)
                {
                    if (newMode == NavigationViewPaneDisplayMode.LeftMinimal)
                    {
                        ClosePane();
                    }
                }
                else
                {
                    if (oldMode == NavigationViewPaneDisplayMode.LeftMinimal
                        && newMode == NavigationViewPaneDisplayMode.Left)
                    {
                        OpenPane();
                    }
                }
            }
        }
    }

    private void UpdatePaneVisibility()
    {
        if (IsPaneVisible)
        {
            if (IsTopNavigationView)
            {
                TemplateSettings.LeftPaneVisibility = false;
                TemplateSettings.TopPaneVisibility = true;
            }
            else
            {
                TemplateSettings.LeftPaneVisibility = true;
                TemplateSettings.TopPaneVisibility = false;
            }

            PseudoClasses.Set(s_pcPaneCollapsed, false);
        }
        else
        {
            TemplateSettings.LeftPaneVisibility = false;
            TemplateSettings.TopPaneVisibility = false;

            PseudoClasses.Set(s_pcPaneCollapsed, true);
        }
    }

    private void SwapPaneHeaderContent(ContentControl newParent, ContentControl oldParent, AvaloniaProperty targetProperty)
    {
        if (newParent != null)
        {
            if (oldParent != null)
            {
                oldParent.SetValue(ContentControl.ContentProperty, null);
            }

            newParent[!ContentControl.ContentProperty] = this[!targetProperty];
        }
    }

    private void UpdateContentBindingsForPaneDisplayMode()
    {
        ContentControl asb = null;
        ContentControl not = null;
        if (!IsTopNavigationView)
        {
            asb = _leftNavAutoSuggestBoxPresenter;
            not = _topNavAutoSuggestBoxPresenter;
        }
        else
        {
            asb = _topNavAutoSuggestBoxPresenter;
            not = _leftNavAutoSuggestBoxPresenter;
        }

        if (asb != null)
        {
            if (not != null)
            {
                not.SetValue(ContentControl.ContentProperty, null);
            }

            asb[!ContentControl.ContentProperty] = this[!AutoCompleteBoxProperty];
        }
    }

    private void UpdateHeaderVisibility()
    {
        if (!_appliedTemplate)
            return;

        UpdateHeaderVisibility(DisplayMode);
    }

    private void UpdateHeaderVisibility(NavigationViewDisplayMode dMode)
    {
        // Ignore AlwaysShowHeader property in case DisplayMode is Minimal and it's not Top NavigationView
        bool showHeader = Header != null && (AlwaysShowHeader || (!IsTopNavigationView && dMode == NavigationViewDisplayMode.Minimal));

        PseudoClasses.Set(s_pcHeaderCollapsed, !showHeader);
    }

    //OnTitleBarMetricsChanged

    //OnTitleBarIsVisibleChanged

    private void UpdateTitleBarPadding()
    {
        if (!_appliedTemplate)
            return;

        //Do titlebar stuff

        bool setPaneTitleHolderFEMargin = _paneTitleHolderFrameworkElement != null && _paneTitleHolderFrameworkElement.IsVisible;
        bool setPaneToggleButtonMargin = !setPaneTitleHolderFEMargin && _paneToggleButton.IsVisible;

        if (setPaneTitleHolderFEMargin || setPaneToggleButtonMargin)
        {
            Thickness thickness = new Thickness();

            if (ShouldShowBackButton)
            {
                if (IsOverlay)
                {
                    thickness = new Thickness(_backButtonWidth, 0, 0, 0);
                }
                else
                {
                    thickness = new Thickness(0, _backButtonHeight, 0, 0);
                }
            }
            else if (ShouldShowCloseButton && IsOverlay)
            {
                thickness = new Thickness(_backButtonWidth, 0, 0, 0);
            }

            if (setPaneTitleHolderFEMargin)
            {
                // The PaneHeader is hosted by PaneTitlePresenter and PaneTitleHolder.
                _paneTitleHolderFrameworkElement.Margin = thickness;
            }
            else
            {
                _paneToggleButton.Margin = thickness;
            }
        }

        //Set TemplateSettings.TopPadding
    }

    //OnAutoSuggestBoxSuggestionChosen

    private void UpdatePaneShadow() { }

    private T GetContainerForData<T>(object data) where T : Control
    {
        if (data == null)
            return default;

        if (data is T nvi)
            return nvi;

        // First conduct a basic top level search in main menu, which should succeed for a lot of scenarios.
        var mainRepeater = IsTopNavigationView ? _topNavRepeater : _leftNavRepeater;
        var itemIndex = GetIndexFromItem(mainRepeater, data);
        if (itemIndex >= 0)
        {
            var cont = mainRepeater.TryGetElement(itemIndex);
            if (cont != null)
            {
                return cont is T t ? t : default;
            }
        }

        // then look in footer menu
        var footerRepeater = IsTopNavigationView ? _topNavFooterMenuRepeater : _leftNavFooterMenuRepeater;
        itemIndex = GetIndexFromItem(footerRepeater, data);
        if (itemIndex >= 0)
        {
            var cont = footerRepeater.TryGetElement(itemIndex);
            if (cont != null)
            {
                return cont is T t ? t : default;
            }
        }

        // If unsuccessful, unfortunately we are going to have to search through the whole tree
        // (WinUI) TODO: Either fix or remove implementation for TopNav.
        // It may not be required due to top nav rarely having realized children in its default state.
        var container = SearchEntireTreeForContainer(mainRepeater, data);
        if (container != null)
        {
            return container is T t ? t : default;
        }

        container = SearchEntireTreeForContainer(footerRepeater, data);
        if (container != null)
        {
            return container is T t ? t : default;
        }

        return default;
    }

    private Control SearchEntireTreeForContainer(ItemsRepeater ir, object data)
    {
        // (WinUI) TODO: Temporary inefficient solution that results in unnecessary time complexity, fix.
        var index = GetIndexFromItem(ir, data);
        if (index != -1)
        {
            return ir.TryGetElement(index);
        }

        for (int i = 0; i < GetContainerCountInRepeater(ir); i++)
        {
            if (ir.TryGetElement(i) is NavigationViewItem nvi)
            {
                if (nvi.GetRepeater != null)
                {
                    var foundElement = SearchEntireTreeForContainer(nvi.GetRepeater, data);
                    if (foundElement != null)
                    {
                        return foundElement;
                    }
                }
            }
        }

        return null;
    }

    private IndexPath SearchEntireTreeForIndexPath(ItemsRepeater ir, object data, bool isFooterRepeater)
    {
        for (int i = 0; i < GetContainerCountInRepeater(ir); i++)
        {
            if (ir.TryGetElement(i) is NavigationViewItem nvi)
            {
                var ip = new IndexPath(new int[] { isFooterRepeater ? _footerMenuBlockIndex : _mainMenuBlockIndex, i });
                var indexPath = SearchEntireTreeForIndexPath(nvi, data, ip);
                if (indexPath != IndexPath.Unselected)
                {
                    return indexPath;
                }
            }
        }

        return IndexPath.Unselected;
    }

    // There are two possibilities here if the passed in item has children. Either the children of the passed in container have already been realized,
    // in which case we simply just iterate through the children containers, or they have not been realized yet and we have to iterate through the data
    // and manually realize each item.
    private IndexPath SearchEntireTreeForIndexPath(NavigationViewItem nviParent, object data, IndexPath ip)
    {
        bool areChildrenRealized = false;
        var childRepeater = nviParent.GetRepeater;
        if (childRepeater != null)
        {
            if (DoesRepeaterHaveRealizedContainers(childRepeater))
            {
                areChildrenRealized = true;
                for (int i = 0; i < GetContainerCountInRepeater(childRepeater); i++)
                {
                    if (childRepeater.TryGetElement(i) is NavigationViewItem nvi)
                    {
                        var newIP = ip.CloneWithChildIndex(i);
                        if (nvi.Content == data)
                        {
                            return newIP;
                        }
                        else
                        {
                            var foundIP = SearchEntireTreeForIndexPath(nvi, data, newIP);
                            if (foundIP != IndexPath.Unselected)
                            {
                                return foundIP;
                            }
                        }
                    }
                    else
                    {
                        // We found an unrealized child, so we'll want to manually realize and search if we don't find the item.
                        areChildrenRealized = false;
                    }
                }
            }
        }

        //If children are not realized, manually realize and search.
        if (!areChildrenRealized)
        {
            var childrenData = GetChildren(nviParent);
            if (childrenData != null)
            {
                // Get children data in an enumarable form
                //WinUI goes to ItemsSourceView for this, but we're already in an IEnumerable form (IList), so skip

                for (int i = 0; i < childrenData.Count(); i++)
                {
                    var newIP = ip.CloneWithChildIndex(i);
                    var childData = childrenData.ElementAt(i);
                    if (childData == data)
                    {
                        return newIP;
                    }
                    else
                    {
                        var nvib = ResolveContainerForItem(childData, i);
                        if (nvib is NavigationViewItem nvi)
                        {
                            //TODO...
                        }
                    }
                }
            }
        }

        return IndexPath.Unselected;
    }

    private NavigationViewItemBase ResolveContainerForItem(object item, int index)
    {
        var args = new ElementFactoryGetArgs();
        args.Data = item;
        args.Index = index;

        var container = _itemsFactory.GetElement(args);
        if (container is NavigationViewItemBase nvib)
        {
            return nvib;
        }

        return null;
    }

    private void RecycleContainer(Control container)
    {
        var args = new ElementFactoryRecycleArgs();
        args.Element = container;
        _itemsFactory.RecycleElement(args);
    }

    private Control GetContainerForIndex(int index, bool inFooter)
    {
        if (IsTopNavigationView)
        {
            // Get the repeater that is presenting the first item
            var ir = inFooter ? _topNavFooterMenuRepeater :
                (_topDataProvider.IsItemInPrimaryList(index) ? _topNavRepeater : _topNavRepeaterOverflowView);

            // Get the index of the item in the repeater
            var irIndex = inFooter ? index : _topDataProvider.ConvertOriginalIndexToIndex(index);

            return ir.TryGetElement(irIndex);
        }
        else
        {
            var container = inFooter ? _leftNavFooterMenuRepeater.TryGetElement(index) :
                _leftNavRepeater.TryGetElement(index);

            return container as NavigationViewItemBase;
        }
    }

    private NavigationViewItemBase GetContainerForIndexPath(IndexPath ip, bool lastVisible = false, bool forceRealize = false)
    {
        if (ip != IndexPath.Unselected && ip.GetSize() > 0)
        {
            var cont = GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == _footerMenuBlockIndex);
            if (cont != null)
            {
                if (lastVisible)
                {
                    if (cont is NavigationViewItem nvi)
                    {
                        if (!nvi.IsExpanded)
                        {
                            return nvi;
                        }
                    }
                }

                // TODO: Fix below for top flyout scenario once the flyout is introduced in the XAML.
                // We want to be able to retrieve containers for items that are in the flyout.
                // This will return nullptr if requesting children containers of
                // items in the primary list, or unrealized items in the overflow popup.
                // However this should not happen.
                return GetContainerForIndexPath(cont, ip, lastVisible, forceRealize);
            }
        }

        return null;
    }

    private NavigationViewItemBase GetContainerForIndexPath(Control first, IndexPath ip, bool lastVisible, bool forceRealize)
    {
        var cont = first;
        if (ip.GetSize() > 2)
        {
            for (int i = 2; i < ip.GetSize(); i++)
            {
                bool succeed = false;
                if (cont is NavigationViewItem nvi)
                {
                    if (lastVisible && !nvi.IsExpanded)
                    {
                        return nvi;
                    }
        
                    var nviRepeater = nvi.GetRepeater;
                    if (nviRepeater != null)
                    {
                        var index = ip.GetAt(i);
        
                        var nextCont = forceRealize ? nviRepeater.GetOrCreateElement(index) : nviRepeater.TryGetElement(index);
                        if (nextCont != null)
                        {
                            cont = nextCont;
                            succeed = true;
                        }
                    }
                }
                // If any of the above checks failed, it means something went wrong and we have an index for a non-existent repeater.
                if (!succeed)
                {
                    return null;
                }
            }
        }

        return cont as NavigationViewItemBase;
    }

    private IEnumerable GetChildrenForItemInIndexPath(IndexPath ip, bool forceRealize)
    {
        if (ip != IndexPath.Unselected && ip.GetSize() > 1)
        {
            var cont = GetContainerForIndex(ip.GetAt(1), ip.GetAt(0) == _footerMenuBlockIndex);
            if (cont != null)
            {
                return GetChildrenForItemInIndexPath(cont, ip, forceRealize);
            }
        }

        return null;
    }

    private IEnumerable GetChildrenForItemInIndexPath(Control first, IndexPath ip, bool forceRealize)
    {
        var container = first;
        bool shouldRecycle = false;
        if (ip.GetSize() > 2)
        {
            for (int i = 2; i < ip.GetSize(); i++)
            {
                bool succeed = false;
                if (container is NavigationViewItem nvi)
                {
                    var nextContIndex = ip.GetAt(i);
                    var nviRep = nvi.GetRepeater;
                    if (nviRep != null && DoesRepeaterHaveRealizedContainers(nviRep))
                    {
                        var nextCont = nviRep.TryGetElement(nextContIndex);
                        if (nextCont != null)
                        {
                            container = nextCont;
                            succeed = true;
                        }
                    }
                    else if (forceRealize)
                    {
                        var childrenData = GetChildren(nvi);
                        if (childrenData != null)
                        {
                            if (shouldRecycle)
                            {
                                RecycleContainer(nvi);
                                shouldRecycle = false;
                            }

                            //Already in enumerable for, skip convert to ItemsSourceView

                            var data = childrenData.ElementAt(nextContIndex);
                            if (data != null)
                            {
                                var nvib = ResolveContainerForItem(data, nextContIndex);
                                if (nvib is NavigationViewItem nextNVI)
                                {
                                    container = nextNVI;
                                    shouldRecycle = true;
                                    succeed = true;
                                }
                            }
                        }
                    }
                }
                // If any of the above checks failed, it means something went wrong and we have an index for a non-existent repeater.
                if (!succeed)
                {
                    return null;
                }
            }
        }

        if (container is NavigationViewItem finalNVI)
        {
            var children = GetChildren(finalNVI);
            if (shouldRecycle)
            {
                RecycleContainer(finalNVI);
            }
            return children;
        }

        return null;
    }

    private void UpdateOpenPaneWidth(double width) // WinUI #5800
    {
        if (!IsTopNavigationView && _splitView != null)
        {
            _openPaneWidth = Math.Max(0, Math.Min(width, OpenPaneLength));

            TemplateSettings.OpenPaneWidth = _openPaneWidth;
        }
    }

    private void SetPaneToggleButtonAutomationName()
    {
        // TODO: Automation
        string navigationName;
        if (IsPaneOpen)
        {
            navigationName = LocalizationService.Instance.GetString(SR_NavigationButtonOpenName);
        }
        else
        {
            navigationName = LocalizationService.Instance.GetString(SR_NavigationButtonClosedName);
        }


        if (_paneToggleButton != null)
        {
            ToolTip.SetTip(_paneToggleButton, navigationName);
        }
    }

    private class StepEasingFunction : Easing
    {
        // TODO: What is the default step count for WinUI's StepEasingFunction
        public int Steps { get; set; }

        public override double Ease(double progress)
        {
            return Math.Round(progress * Steps) * (1 / Steps);
        }
    }
}
