using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls.Primitives;

/// <summary>
/// Represents the visual elements of a NavigationViewItem.
/// </summary>
[PseudoClasses(PC_EXPANDED)]
[PseudoClasses(PC_CLOSED_COMPACT_TOP, PC_NOT_COLOSED_COMPACT_TOP)]
[PseudoClasses(SharedPseudoclasses.s_pcLeftNav, SharedPseudoclasses.s_pcTopNav, SharedPseudoclasses.s_pcTopOverflow)]
[PseudoClasses(SharedPseudoclasses.s_pcChevronOpen, SharedPseudoclasses.s_pcChevronClosed, SharedPseudoclasses.s_pcChevronHidden)]
[PseudoClasses(SharedPseudoclasses.s_pcIconLeft, SharedPseudoclasses.s_pcIconOnly, SharedPseudoclasses.s_pcContentOnly)]
[PseudoClasses(SharedPseudoclasses.s_pcPressed)]
public class NavigationViewItemPresenter : ContentControl
{
    /// <summary>
    /// Defines the <see cref="IconSource"/> property
    /// </summary>
    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<NavigationViewItem, object?>(nameof(IconSource));

    /// <summary>
    /// Defines the <see cref="InfoBadge"/> property
    /// </summary>
    public static readonly StyledProperty<InfoBadge?> InfoBadgeProperty =
        NavigationViewItem.InfoBadgeProperty.AddOwner<NavigationViewItemPresenter>();

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<NavigationViewItemPresenter, double>(nameof(IconSize), 16);

    public static readonly StyledProperty<double> SmallerIconSizeProperty =
        AvaloniaProperty.Register<NavigationViewItemPresenter, double>(nameof(SmallerIconSize));

    public double SmallerIconSize
    {
        get => GetValue(SmallerIconSizeProperty);
        set => SetValue(SmallerIconSizeProperty, value);
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon in a NavigationView item.
    /// </summary>
    public object? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the InfoBadge used in the NavigationViewItemPresenter
    /// </summary>
    public InfoBadge? InfoBadge
    {
        get => GetValue(InfoBadgeProperty);
        set => SetValue(InfoBadgeProperty, value);
    }

    internal NavigationViewItem? GetNavigationViewItem => this.FindAncestorOfType<NavigationViewItem>();

    internal Control? SelectionIndicator => _selectionIndicator;
    
    private Panel? _contentGrid;
    private Panel? _expandCollapseChevron;
    private Control? _selectionIndicator;
    private ContentPresenter? _infoBadgePresenter;
    private double _compactPaneLengthValue = 40;
    private double _leftIndentation;

    private const string SELECTION_INDICATOR = "SelectionIndicator";
    private const string PRESENTER_CONTENT_ROOT_GRID = "PresenterContentRootGrid";
    private const string INFO_BADGE_PRESENTER = "InfoBadgePresenter";
    private const string EXPAND_COLLAPSE_CHEVRON = "ExpandCollapseChevron";

    private const string PC_CLOSED_COMPACT_TOP = ":closedcompacttop";
    private const string PC_NOT_COLOSED_COMPACT_TOP = ":notclosedcompacttop";
    private const string PC_EXPANDED = ":expanded";
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _selectionIndicator = e.NameScope.Find<Border>(SELECTION_INDICATOR);

        //This doesn't exist in the TopPane template, so use Find and allow it to be null
        _contentGrid = e.NameScope.Find<Panel>(PRESENTER_CONTENT_ROOT_GRID);

        _infoBadgePresenter = e.NameScope.Find<ContentPresenter>(INFO_BADGE_PRESENTER);

        var nvi = GetNavigationViewItem;
        if (nvi != null)
        {
            _expandCollapseChevron = e.NameScope.Find<Panel>(EXPAND_COLLAPSE_CHEVRON);

            if (_expandCollapseChevron != null)
            {
                _expandCollapseChevron.Tapped += nvi.OnExpandCollapseChevronTapped;
            }
            nvi.UpdateVisualState();

            // We probably switched displaymode, so restore width now, otherwise the next time we will restore is when the CompactPaneLength changes
            var navView = nvi.GetNavigationView;
            if (navView != null)
            {
                if (navView.PaneDisplayMode != NavigationViewPaneDisplayMode.Top)
                {
                    UpdateCompactPaneLength(_compactPaneLengthValue, true);
                }
            }
        }

        UpdateMargin();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcPressed, true);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased
            && e.InitialPressMouseButton == MouseButton.Left)
        {
            PseudoClasses.Set(SharedPseudoclasses.s_pcPressed, false);
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        PseudoClasses.Set(SharedPseudoclasses.s_pcPressed, false);
    }

    internal void RotateExpandCollapseChevron(bool isExpanded)
    {
        PseudoClasses.Set(PC_EXPANDED, isExpanded);
    }

    internal void UpdateContentLeftIndentation(double leftIndent)
    {
        _leftIndentation = leftIndent;
        UpdateMargin();
    }

    private void UpdateMargin()
    {
        if (_contentGrid != null)
        {
            var oldMargin = _contentGrid.Margin;
            _contentGrid.Margin = new Thickness(_leftIndentation, oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
        }
    }

    internal void UpdateCompactPaneLength(double len, bool update)
    {
        _compactPaneLengthValue = len;

        if (update)
        {
            // IconSize = len;
            SmallerIconSize = len - 24;
        }
    }

    internal void UpdateClosedCompactVisualState(bool topLevel, bool isClosedCompact)
    {
        // We increased the ContentPresenter margin to align it visually with the expand/collapse chevron. This updated margin is even applied when the
        // NavigationView is in a visual state where no expand/collapse chevrons are shown, leading to more content being cut off than necessary.
        // This is the case for top-level items when the NavigationView is in a compact mode and the NavigationView pane is closed. To keep the original
        // cutoff visual experience intact, we restore  the original ContentPresenter margin for such top-level items only (children shown in a flyout
        // will use the updated margin).

        //states :closedcompacttop, :notclosedcompacttop

        PseudoClasses.Set(PC_CLOSED_COMPACT_TOP, isClosedCompact && topLevel);
        PseudoClasses.Set(PC_NOT_COLOSED_COMPACT_TOP, !isClosedCompact && topLevel);
    }
}
