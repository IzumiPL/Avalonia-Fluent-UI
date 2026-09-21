using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents a line that separates menu items in a NavigationView.
/// </summary>
[PseudoClasses(PC_HORIZONTAL, PC_HORIZONTAL_COMPACT, PC_VERTICAL)]
[TemplatePart(Name = ROOT_GRID, Type = typeof(Panel))]
public class NavigationViewItemSeparator : NavigationViewItemBase
{
    private Panel? _rootGrid;
    private bool _appliedTemplate;
    private bool _isClosedCompact;
    private CompositeDisposable? _splitViewRevokers;

    private const string ROOT_GRID = "RootGrid";

    private const string PC_HORIZONTAL = ":horizontal";
    private const string PC_HORIZONTAL_COMPACT = ":horizontalcompact";
    private const string PC_VERTICAL = ":vertical";
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _appliedTemplate = false;

        _splitViewRevokers?.Dispose();

        base.OnApplyTemplate(e);

        _rootGrid = e.NameScope.Find<Panel>(ROOT_GRID);

        var splitView = GetSplitView;
        if (splitView != null)
        {
            _splitViewRevokers = new CompositeDisposable(
                splitView.GetPropertyChangedObservable(SplitView.IsPaneOpenProperty).Subscribe(OnSplitViewPropertyChanged),
                splitView.GetPropertyChangedObservable(SplitView.DisplayModeProperty).Subscribe(OnSplitViewPropertyChanged));

            UpdateIsClosedCompact(false);
        }

        _appliedTemplate = true;
        UpdateVisualState();
        UpdateItemIndentation();
    }

    protected override void OnNavigationViewItemBaseDepthChanged()
    {
        UpdateItemIndentation();
    }

    protected override void OnNavigationViewItemBasePositionChanged()
    {
        UpdateVisualState();
    }

    private void OnSplitViewPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        UpdateIsClosedCompact(true);
    }

    private void UpdateVisualState()
    {
        if (!_appliedTemplate)
            return;

        //States: :horizontalcompact, :horizontal, :vertical
        bool isTop = Position == NavigationViewRepeaterPosition.TopFooter || Position == NavigationViewRepeaterPosition.TopPrimary;

        PseudoClasses.Set(PC_HORIZONTAL, !isTop && !_isClosedCompact);
        PseudoClasses.Set(PC_HORIZONTAL_COMPACT, !isTop && _isClosedCompact);
        PseudoClasses.Set(PC_VERTICAL, isTop);
    }

    private void UpdateItemIndentation()
    {
        if (_rootGrid == null)
            return;

        var oldMargin = _rootGrid.Margin;
        var newLeft = Depth * _itemIndentation;
        _rootGrid.Margin = new Thickness(newLeft, oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
    }

    private void UpdateIsClosedCompact(bool updateVisState)
    {
        var splitView = GetSplitView;
        if (splitView != null)
        {
            _isClosedCompact = !splitView.IsPaneOpen &&
                (splitView.DisplayMode == SplitViewDisplayMode.CompactInline || splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay);

            if (updateVisState)
                UpdateVisualState();
        }
    }
}
