using System;
using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaFluentUI.Core;
using AvaloniaFluentUI.Data;
using AvaloniaFluentUI.Styling;

namespace AvaloniaFluentUI.Controls.Primitives;

/// <summary>
/// Represents the ListView used in the TabStrip of a <see cref="TabView"/>
/// </summary>
/// <remarks>
/// This control should not be used outside of a TabView
/// </remarks>
[PseudoClasses(PC_REORDER)]
[TemplatePart(Name = SCROLL_VIEWER, Type = typeof(SingleDirectionScrollViewer))]
public sealed class TabViewListView : ListBox
{
    public TabViewListView()
    {
        ItemsView.CollectionChanged += OnItemsChanged;


        Tapped += (s, e) =>
        {
            if (e.Source is Visual v && v.FindAncestorOfType<TabViewItem>(true) is TabViewItem tvi)
            {
                var index = IndexFromContainer(tvi);
                UpdateSelection(index, true);
                e.Handled = true;
            }
        };

        // Because of event differences in WinUI vs Avalonia, we need more control over the drag events
        // so we are translating them into CLR events for the TabView to subscribe to
        // See OnApplyTemplate for more info
        AddHandler(DragDrop.DragOverEvent, OnListViewDragOver);
        AddHandler(DragDrop.DropEvent, OnListViewDrop);
    }

    /// <summary>
    /// Defines the <see cref="CanReorderItems"/> property
    /// </summary>
    public static readonly StyledProperty<bool> CanReorderItemsProperty =
        AvaloniaProperty.Register<TabViewListView, bool>(nameof(CanReorderItems));

    /// <summary>
    /// Gets or sets whether this ListView can reorder items. This is the single switch for
    /// dragging: when true, items can be dragged (both reordered within the strip and torn
    /// out as a data payload); when false, dragging is disabled entirely.
    /// </summary>
    public bool CanReorderItems
    {
        get => GetValue(CanReorderItemsProperty);
        set => SetValue(CanReorderItemsProperty, value);
    }

    internal SingleDirectionScrollViewer? Scroller { get; private set; }

    /// <summary>
    /// Gets whether a reorder drag initiated by this ListView is currently in progress.
    /// </summary>
    internal bool IsInReorder => _isInReorder;

    internal event EventHandler<DragEventArgs>? DragEnter;
    internal event EventHandler<DragEventArgs>? DragOver;
    internal event EventHandler<DragEventArgs>? DragLeave;
    internal event EventHandler<DragEventArgs>? Drop;

    /// <summary>
    /// Occurs when a drag operation that involves one of the items in the view is initiated.
    /// </summary>
    public event DragItemsStartingEventHandler DragItemsStarting;

    /// <summary>
    /// Occurs when a drag operation that involves one of the items in the view is ended.
    /// </summary>
    public event TypedEventHandler<TabViewListView, DragItemsCompletedEventArgs> DragItemsCompleted;
    
    private TabViewItem? _dragItem;
    private int _dragIndex = -1;
    private bool _isDragItemFocused;
    private bool _isDragItemSelected;
    private bool _isInDrag = false;
    private bool _isInReorder = false;
    private Point? _initialPoint;
    private double _cxDrag = double.NaN;
    private double _cyDrag = double.NaN;
    private Control? _parent;
    private bool _isDragWithinTabStrip;

    private LiveReorderHelper? _liveReorderHelper;    
    private Point? _lastDragOverPoint;
    private long _lastReorderProcessStamp;

    // Drag ghost (a floating snapshot of the dragged tab that follows the cursor)
    private Border? _dragGhost;
    private OverlayLayer? _dragGhostLayer;
    private Size _dragGhostSize;
    private TranslateTransform? _dragGhostTransform;
    private Point? _lastGhostPoint;
    private Point? _pendingGhostPoint;
    private bool _ghostUpdateQueued;

    // Cursor management while a drag is over this list. The two cursors are cached
    // statically so the per-DragOver hot path never allocates a new Cursor (which also
    // re-triggers a hover refresh on every event and was a measurable source of jank).
    private static readonly Cursor DragMoveCursor = new Cursor(StandardCursorType.DragMove);
    private static readonly Cursor DragNoCursor = new Cursor(StandardCursorType.No);

    private bool _dragCursorApplied;
    private bool _dragCursorAccepted;
    private Cursor? _previousCursor;
    private Cursor? _previousTabViewCursor;

    // Reorder drag: the dragged data item is removed from the list while dragging, and
    // re-inserted on drop (or restored to its original position if the drag is cancelled).
    private object? _dragData;
    private bool _dragItemRemoved;

    // For 12.0/v3 - Avalonia has decided to make the decision that the lowest common denominator
    // in the platform backends decides the entire public API. As part of this, DoDragDrop now
    // requires the initial pressed args, so we have to store them away so we can start DragDrop.
    // I tried to object, and failed (https://github.com/AvaloniaUI/Avalonia/pull/20988)
    // And you guessed it, freakin' Wayland
    private PointerPressedEventArgs? _initArgs;
    
    private DispatcherTimer? _scrollTimer;
    private Vector _currentAutoPanVelocity;

    private const string SCROLL_VIEWER = "ScrollViewer";

    private const string PC_REORDER = ":reorder";
    private const string PC_LEFT_SHORT = ":leftShort";
    private const string PC_RIGHT_SHORT = ":rightShort";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _parent?.RemoveHandler(DragDrop.DragLeaveEvent, OnParentDragEnter);
        _parent = null;

        base.OnApplyTemplate(e);
        Scroller = e.NameScope.Find<SingleDirectionScrollViewer>(SCROLL_VIEWER);

        // HACK: DragDrop events work differently in Avalonia than WinUI. In WinUI, they aren't true
        // routed events, so the OriginalSource parameter returns TabView or TabViewItem, etc., not
        // template items. In Avalonia, we get template items, but only like 1 or 2. This means that
        // we can't actually detect if we've left the TabStrip during a drag operation. As of me writing
        // this, the only things we detect from the local Drag handlers is some template items in the
        // TabViewItems - ABSOLUTELY NOTHING FROM TABVIEWLISTVIEW, arghhhhh...That's annoying
        // So this seems to work - if we grab a drag enter handler on the TabView itself and do a bounds
        // check on this, we know if the pointer left the tab strip or not. 
        _parent = this.FindAncestorOfType<TabView>();
        _parent?.AddHandler(DragDrop.DragLeaveEvent, OnParentDragEnter);
        _parent?.AddHandler(DragDrop.DragOverEvent, OnParentDragOver);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedIndexProperty)
        {
            UpdateBottomBorderVisualState();
        }
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        bool isItem = item is TabViewItem;
        recycleKey = isItem ? null : nameof(TabViewItem);
        return !isItem;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        var cont = this.FindDataTemplate(item, ItemTemplate)?.Build(item);
        
        if (cont is TabViewItem tvi)
        {
            tvi.IsContainerFromTemplate = true;
            // Remember what the template declared as content - Avalonia's container preparation
            // may replace it with the data item (it does so when recycling containers), and we
            // need to be able to put it back. See ContainerForItemPreparedOverride.
            tvi.TemplateContent = tvi.Content;
            return tvi;
        }

        return new TabViewItem();
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        // NOTE: BE CAREFUL HERE! Avalonia has two separate preparation events
        // PrepareContainerForItemOverride - does not have the container connected yet
        // This one does - I've raised an issue b/c this is dumb
        var tvi = container as TabViewItem;

        // WinUI: Due to virtualization, a TabViewItem might be recycled to display a different tab data item.
        //        In that case, there is no need to set the TabWidthMode of the TabViewItem or its parent TabView
        //        as they are already set correctly here.
        //
        //        We know we are currently looking at a TabViewItem being recycled if its parent TabView has
        //        already been set.
        var tabLocation = TabViewTabStripLocation.Top; // Default to top
        if (tvi?.ParentTabView == null)
        {
            var tabView = container.FindAncestorOfType<TabView>();
            if (tabView != null)
            {
                tvi?.OnTabViewWidthModeChanged(tabView.TabWidthMode);
                tvi?.ParentTabView = tabView;
                tabLocation = tabView.TabStripLocation;
            }
        }
        else
        {
            tabLocation = tvi.ParentTabView.TabStripLocation;
        }

        tvi.HandleTabStripLocationChanged(tabLocation);

        // Special b/c we use the header and not Content. Somehow this *just works* in
        // WinUI b/c they don't have to do this.
        if (container == item || tvi.IsContainerFromTemplate)
        {
            // PrepareContainerForItemOverride will set the ContentTemplate of the item, but it will default
            // to ItemTemplate, which only should be applied to the header (since its a TVI). In this case,
            // undo that and search elsewhere for an appropriate template without defaulting to the
            // ItemTemplate passed down from the TabView - fixes GH 739
            var template = ItemTemplate;
            if (tvi.ContentTemplate == template)
            {
                tvi.ContentTemplate = this.FindDataTemplate(item);
            }

            if (tvi.IsContainerFromTemplate)
            {
                // The same preparation can also have replaced the content declared inside the
                // TabItemTemplate with the raw data item, which would leave the tab showing the
                // data item's ToString(). Put the declared content back.
                if (!ReferenceEquals(tvi.Content, tvi.TemplateContent))
                {
                    tvi.Content = tvi.TemplateContent;
                    tvi.ContentTemplate = null;
                }

                // TabView moves the selected item's Content into its own content presenter
                // (see TabView.UpdateTabContent). Once the content is no longer a visual child of
                // this container, its inherited DataContext resolves to the TabView's one instead
                // of the data item - silently breaking every binding inside the template.
                // Pin it so the content keeps binding to its own tab (and re-pin when a recycled
                // container is prepared for a different item).
                if (tvi.Content is Control declaredContent &&
                    (!declaredContent.IsSet(DataContextProperty) ||
                     ReferenceEquals(declaredContent.GetValue(DataContextProperty), tvi.TemplateContentOwner)))
                {
                    tvi.TemplateContentOwner = item;
                    declaredContent.SetValue(DataContextProperty, item);
                }
            }

            base.ContainerForItemPreparedOverride(container, item, index);

            ApplyCurrentTabWidth(tvi);
            return;
        }

        tvi.Header = item;

        var itemTemplate = this.FindDataTemplate(item, ItemTemplate);

        if (itemTemplate != null)
            tvi.HeaderTemplate = itemTemplate;

        base.ContainerForItemPreparedOverride(container, item, index);

        ApplyCurrentTabWidth(tvi);

        if (!tvi.IsSelected)
        {
            // Bug Fix: When containers are being virtualized, they may "come back online" with
            // old state left over.
            // This is also a bug in WinUI, but it materializes differently. In Avalonia, we can see
            // this very clearly because WinUI pins and does not recycle the SelectedItem container,
            // but Avalonia does. Thus, when the previously selected container is reused, it still
            // has the visual state we apply to selected items, specifically the :noborder pseudoclass
            // Because WinUI pins the container, we never see this issue
            // HOWEVER, we can see it in in WinUI with the LeftOfSelectedTab/RightOfSelectedTab states
            // If you select an item and then scroll away such that SelIndex-1 and Selindex+1 container
            // are recycled, inspect them, you'll see the margin still applied to the Border line indicating
            // the state was never cleared. If you scroll to those new containers you'll see a tiny little
            // gap in the bottom border because of this. 
            // Fix here by just ensuring this state get's updated. I don't want to call TabView.UpdateBottom...
            // because that iterates over containers and that isn't great in this path. 
            // Also added unit test to ensure this is fixed.

            var selIndex = SelectedIndex;
            int state = -1;
            if (selIndex != -1)
            {
                if (index == selIndex)
                {
                    state = 0;
                }
                else if (index == selIndex - 1)
                {
                    state = 1;
                }
                else if (index == selIndex + 1)
                {
                    state = 2;
                }
            }

            ((IPseudoClasses)tvi.Classes).Set(SharedPseudoclasses.s_pcNoBorder, state == 0);
            ((IPseudoClasses)tvi.Classes).Set(SharedPseudoclasses.s_pcBorderLeft, state == 1);
            ((IPseudoClasses)tvi.Classes).Set(SharedPseudoclasses.s_pcBorderRight, state == 2);
        }
    }

    /// <summary>
    /// Applies the TabView's most recent tab width to a freshly prepared container.
    /// UpdateTabWidths may have run before this container was realized (virtualization /
    /// late materialization of the last item), leaving the container at its natural width -
    /// the visible symptom is the last tab being narrower/wider than the rest.
    /// </summary>
    private void ApplyCurrentTabWidth(TabViewItem tvi)
    {
        if (tvi.ParentTabView is { } tabView)
        {
            tvi.Width = tabView.CurrentTabWidth;
            tvi.RefreshTabGeometry();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs args)
    {
        if (args.Handled)
            return;

        if (CanReorderItems)
        {
            var currentPoint = args.GetCurrentPoint(this);
            if (currentPoint.Properties.IsLeftButtonPressed)
            {
                _initialPoint = currentPoint.Position;
                _dragItem = (args.Source as Visual).FindAncestorOfType<TabViewItem>(true);

                // Clicking in empty space of tab row will still fire this.
                if (_dragItem == null)
                    return;

                _dragIndex = IndexFromContainer(_dragItem);
                _isDragItemFocused = _dragItem.IsFocused;
                _isDragItemSelected = _dragItem.IsSelected;
                _initArgs = args;
                UpdateDragInfo();
                // args.Handled = true;
            }
        }

        base.OnPointerPressed(args);
    }

    protected override void OnPointerMoved(PointerEventArgs args)
    {
        if (args.Handled)
            return;

        if (_initialPoint.HasValue)
        {
            if (!_isInDrag && !_isInReorder)
            {
                var currentPoint = args.GetPosition(this);
                var delta = currentPoint - _initialPoint.Value;

                if (double.Abs(delta.X) > _cxDrag || double.Abs(delta.Y) > _cyDrag)
                {
                    BeginDragReorder();
                    //args.Handled = true;
                }
            }
        }

        base.OnPointerMoved(args);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs args)
    {
        base.OnPointerReleased(args);
        CancelDrag();
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs args)
    {
        base.OnPointerCaptureLost(args);

        // TODO: We really need to handle this, but this fires in cases I don't think it should
        // 1- Mouse Button Release
        // 2- Start of DragDrop
    }
    

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _parent?.RemoveHandler(DragDrop.DragLeaveEvent, OnParentDragEnter);
        _parent?.RemoveHandler(DragDrop.DragOverEvent, OnParentDragOver);
        _parent = null;
        DestroyStartEdgeScrollTimer();
    }

    private async void BeginDragReorder()
    {
        if (_dragIndex < 0 || _initArgs == null)
        {
            CancelDrag();
            return;
        }

        var package = new DataPackage();
        // Avalonia 的 DataPackage.RequestedOperation 默认是 Copy，而重排需要 Move。若不显式
        // 设置，DoDragDropAsync 会用 Copy 启动拖拽，DragOver 时 args.DragEffects 与 Move 位与
        // 会变成 None，导致光标显示"禁止拖放"。这里默认 Move（用户可在 DragItemsStarting 里覆盖）。
        package.RequestedOperation = DragDropEffects.Move;
        DragItemsStartingEventArgs? dragArgs = null;
        object[]? dragItems = null;
        bool canReorder = CanReorderItems;

        // "可重排序"现在是唯一开关：为 true 时才能拖动。拖动同时承担两个职责——
        // 在标签条内重排（drop 回本 TabView），以及作为数据包拖出（drop 到别的 TabView）。
        if (canReorder)
        {
            // 准备数据包并触发 DragItemsStarting，让应用有机会取消本次拖动。
            dragItems = new object[] { ItemsView.GetAt(_dragIndex) };
            dragArgs = new DragItemsStartingEventArgs { Items = dragItems, Data = package };
            DragItemsStarting?.Invoke(this, dragArgs);

            if (dragArgs.Cancel)
            {
                CancelDrag();
                return;
            }

            _isInDrag = true;

            // 重排准备：校验 drop 目标已开启、集合可变更，并记下数据项供 drop 时插回。
            // WinUI 允许在源集合非 INCC 时也发起拖拽，这里按更严谨的方式拒绝。
            if (!DragDrop.GetAllowDrop(this))
            {
                CancelDrag();
                Logger.TryGet(LogEventLevel.Debug, "TabView")?
                    .Log("TabView", "User disabled this TabView as as drop target - canceling drag");
                return;
            }

            // Avalonia 要求 INCC 集合还必须实现非泛型 IList，这里一并校验 IsReadOnly。
            var src = ItemsSource;
            if (src != null && (src is not INotifyCollectionChanged || (src is IList l && l.IsReadOnly)))
            {
                CancelDrag();
                Logger.TryGet(LogEventLevel.Debug, "TabView")?
                    .Log("TabView", "Attempted to initiate Drag/Reorder without INCC / mutable collection");
                return;
            }

            // 记下被拖的数据项，drop 时插回新位置，取消时恢复到原位。
            _dragData = ItemsView.GetAt(_dragIndex);
            _isInReorder = true;
        }

        var effects = dragArgs?.Data?.RequestedOperation ?? DragDropEffects.Move;

        // Mark the source item as being dragged so the TabView skips its "drop-in hole"
        // width adjustment (that is only meant for external drags).
        if (_dragItem is { } draggedTab)
        {
            draggedTab.IsBeingDragged = true;
        }

        // Show a floating snapshot of the dragged tab. It keeps the exact width/height the
        // tab occupied in the list and follows the cursor for the whole drag.
        var dragWidth = _dragItem?.Bounds.Width ?? 0;
        var dragHeight = _dragItem?.Bounds.Height ?? 0;
        ShowDragGhost(_dragItem, dragWidth, dragHeight);

        // For a reorder, remove the dragged item from the list so it disappears while
        // dragging. It is re-inserted on drop, or restored if the drag is cancelled.
        if (_isInReorder)
        {
            RemoveItemAt(_dragIndex);
            _dragItemRemoved = true;
        }

        var dropResult = await DragDrop.DoDragDropAsync(_initArgs, package, effects);

        SetPendingAutoPanVelocity(default);
        DestroyStartEdgeScrollTimer();

        if (_isInReorder)
        {
            _isInReorder = false;
        }

        if (_isInDrag)
        {
            var completedArgs = new DragItemsCompletedEventArgs(dropResult, dragItems);
            DragItemsCompleted?.Invoke(this, completedArgs);
            _isInDrag = false;
        }

        CancelDrag();
        _liveReorderHelper?.ClearContainerBoundsCache(true);
    }

    private void OnListViewDragOver(object? sender, DragEventArgs e)
    {
        // OLE DragDrop seems to fire on a Timer which means we get a constant stream
        // of DragOver events. WinRT DnD doesn't do this. The issue is in the live reorder
        // system where constant events constantly restart the timer so it never actually
        // triggers. So to prevent that, filter the pointer and only proceed with drag
        // if the pointer location changes
        if (_lastDragOverPoint == null)
        {
            _lastDragOverPoint = e.GetPosition(this);
        }
        else
        {
            var pt = e.GetPosition(this);
            // Use a small (0.5px) dead-zone instead of exact equality so the tiny
            // pointer jitter OLE reports on every timer tick doesn't re-run the whole
            // reorder pipeline (closest-element scan + timer restart) each frame.
            if (double.Abs(pt.X - _lastDragOverPoint.Value.X) < 0.5 &&
                double.Abs(pt.Y - _lastDragOverPoint.Value.Y) < 0.5)
            {
                return;
            }
            _lastDragOverPoint = pt;
        }

        bool canReorder = CanReorderItems;
        bool isInReorderFromExternalSource = (!_isInReorder && canReorder);

        if (!_isDragWithinTabStrip)
        {
            _isDragWithinTabStrip = true;
            DragEnter?.Invoke(this, e);
        }
        else
        {
            // 按 WinUI 语义：重排进行中不触发 TabStripDragOver，仅纯拖动时触发。
            if (!_isInReorder)
                DragOver?.Invoke(this, e);
        }
                
        Process(_isInReorder, canReorder, e);

        // Update the drop cursor (accept vs reject). The drag ghost is tracked in
        // OnParentDragOver, which receives the same event as it bubbles up to the
        // TabView — updating it here too would run the position transform twice per
        // event for no reason.
        SetDragCursor(e.DragEffects != DragDropEffects.None);

        if (_scrollTimer == null && (isInReorderFromExternalSource || _isInReorder))
        {
            // Throttle the live-reorder estimation to ~30ms. The per-frame DragOver
            // stream fires far more often than the render loop, and the reorder math
            // (two O(n) closest-element scans + a DispatcherTimer restart) doesn't need
            // to run on every single event — the drop position is recomputed from the
            // final pointer location anyway. This removes the last heavy per-event work.
            var now = Environment.TickCount64;
            if (now - _lastReorderProcessStamp >= 30)
            {
                _lastReorderProcessStamp = now;
                _liveReorderHelper ??= new LiveReorderHelper(this);
                _liveReorderHelper.ProcessLiveReorder(e, -1);
            }
        }

        // Reuse the position already captured in the filter above instead of walking
        // the visual tree again with GetPosition on the same event.
        ComputeEdgeScrollVelocity(_lastDragOverPoint!.Value, out var pVelocity);
        SetPendingAutoPanVelocity(pVelocity);

        static void Process(bool isInReorder, bool canReorder, DragEventArgs args)
        {
            // Let the user handle & set drag effects first, if left unhandled,
            // set a move operation on the args if reordering
            // See ListViewBase::OnDragOver in WinUI
            if (!args.Handled)
            {
                // Reorder operations have this
                var effects = isInReorder || canReorder ? DragDropEffects.Move : DragDropEffects.None;
                args.DragEffects &= effects;
            } 
        }
    }

    // This is actually DragLeave for us
    private void OnParentDragEnter(object? sender, DragEventArgs e)
    {
        if (_isDragWithinTabStrip)
        {
            var bnds = new Rect(Bounds.Size);
            var pt = e.GetPosition(this);
            if (!bnds.Contains(pt))
            {
                SetPendingAutoPanVelocity(default);
                DestroyStartEdgeScrollTimer();
                _isDragWithinTabStrip = false;
                _liveReorderHelper?.ResetAllItemsForLiveReorder();
                // Leaving the strip means we can no longer accept the drop here.
                SetDragCursor(false);
                UpdateDragGhostPosition(e);
                DragLeave?.Invoke(this, e);
            }
        }        
    }

    // Tracks the cursor over the whole TabView (strip + content) so the drag ghost keeps
    // following even outside the tab strip / ListView bounds.
    private void OnParentDragOver(object? sender, DragEventArgs e)
    {
        UpdateDragGhostPosition(e);
    }

    private void OnListViewDrop(object? sender, DragEventArgs e)
    {
        if (e.Handled)
            return;

        if (DropCausesReorder())
        {
            var pt = e.GetPosition(this);
            OnReorderDrop(pt);

            e.DragEffects = DragDropEffects.Move;
            e.Handled = true;
        }
        else
        {
            _liveReorderHelper?.ResetAllItemsForLiveReorder();
        }

        Drop?.Invoke(this, e);
    }

    private void CancelDrag()
    {
        // 在把被拖 item 插回集合之前，先清除 live reorder 的渲染偏移。此时集合仍处于
        // "item 已移除"状态，_movedItems 里的 sourceIndex 映射正确；若先插入再清除，
        // index 会整体后移一位，导致清错容器、偏移残留（item 视觉上被挤到别处/消失）。
        _liveReorderHelper?.ResetAllItemsForLiveReorder();

        // 若拖拽未完成（在条带外释放 / 取消），把被拖 item 恢复到原位。
        if (_dragItemRemoved && _dragData != null)
        {
            InsertItemAt(_dragIndex, _dragData);
            if (_isDragItemSelected)
            {
                SelectedIndex = _dragIndex;
            }
        }
        _dragItemRemoved = false;
        _dragData = null;

        // 确保 ":dragging" 占位状态被清除（reorder-only 拖拽时 DragItemsCompleted /
        // OnTabDragCompleted 不会触发），并释放 "being dragged" 标志，避免 TabView 的
        // 拖入宽度逻辑被遗留。
        if (_dragItem is { } draggedTab)
        {
            draggedTab.ClearDragDropVisualState();
            draggedTab.IsBeingDragged = false;
        }

        _initArgs = null;
        _initialPoint = null;
        _isInDrag = _isInReorder = false;
        _dragIndex = -1;
        _dragItem = null;
        _isDragItemSelected = _isDragItemFocused = false;
        _lastDragOverPoint = null;
        _lastReorderProcessStamp = 0;
        _isDragWithinTabStrip = false;

        HideDragGhost();
        ResetDragCursor();
    }

    private bool DropCausesReorder()
    {
        // Only reorder when this ListView initiated the drag and reordering is allowed.
        return _isInReorder && CanReorderItems && DragDrop.GetAllowDrop(this);
    }

    private void OnReorderDrop(Point dropPoint)
    {
        if (!_dragItemRemoved || _dragData == null)
        {
            return;
        }

        // 被拖 item 在拖起时已从列表移除，其余 item 已闭合。这里根据指针位置计算
        // 应插入的 index。
        UpdateLayout();

        _liveReorderHelper ??= new LiveReorderHelper(this);
        int insertIndex = _liveReorderHelper.GetClosestElement(dropPoint, true);

        int itemCount = ItemsView.Count;
        if (insertIndex < 0)
            insertIndex = 0;
        if (insertIndex > itemCount)
            insertIndex = itemCount;

        // 在插入被拖 item 之前先清除挤开的渲染偏移（此时集合未变，index 映射正确），
        // 否则插入后 index 错位会清错容器、偏移残留，导致 item 视觉错位/消失。
        _liveReorderHelper.ResetAllItemsForLiveReorder();

        InsertItemAt(insertIndex, _dragData);
        _dragItemRemoved = false;

        UpdateLayout();
        ScrollIntoView(insertIndex);

        if (_isDragItemFocused)
        {
            if (ContainerFromIndex(insertIndex) is Control c)
            {
                c.Focus();
            }
        }

        if (_isDragItemSelected)
        {
            SelectedIndex = insertIndex;
        }
    }

    private void RemoveItemAt(int index)
    {
        var itemsSource = ItemsSource;
        if (itemsSource is IList l)
        {
            try { l.RemoveAt(index); } catch { }
        }
        else if (itemsSource == null)
        {
            try { Items.RemoveAt(index); } catch { }
        }
    }

    private void InsertItemAt(int index, object? data)
    {
        var itemsSource = ItemsSource;
        if (itemsSource is IList l)
        {
            try { l.Insert(index, data); } catch { }
        }
        else if (itemsSource == null)
        {
            try { Items.Insert(index, data); } catch { }
        }
    }

    private void ComputeEdgeScrollVelocity(Point dragPoint, out Vector pVelocity)
    {
        bool isVerticalEnabled = false;
        bool isHorizontalEnabled = false;
        Size extent = default;
        Size viewport = default;
        Vector offset = default;

        if (Scroller != null)
        {
            var vertical = Scroller.VerticalScrollBarVisibility;
            var horizontal = Scroller.HorizontalScrollBarVisibility;
            extent = Scroller.Extent;
            viewport = Scroller.Viewport;
            offset = Scroller.Offset;

            isVerticalEnabled = vertical != ScrollBarVisibility.Disabled;
            isHorizontalEnabled = horizontal == ScrollBarVisibility.Visible;
        }

        double hVelocity = 0;
        double vVelocity = 0;
        if (isHorizontalEnabled)
        {
            // WinUI uses a hardcoded 100px as the threshold for where autoscroll starts, but that
            // doesn't seem great with small listviews. So I'm using 20% of the width
            var threshold = Bounds.Size.Width * 0.2;
            double bound = 0;

            // Try Scrolling Left
            hVelocity = -ComputeEdgeScrollVelocityFromEdgeDistance(dragPoint.X, threshold);
            if (hVelocity == 0)
            {
                // Try Scrolling Right
                var width = Bounds.Size.Width;
                hVelocity = ComputeEdgeScrollVelocityFromEdgeDistance(width - dragPoint.X, threshold);
                bound = extent.Width - viewport.Width;
            }

            // Disable if we're right up on the edge
            if (MathHelpers.IsClose(bound, offset.X, 0.05))
            {
                hVelocity = 0;
            }
        }

        if (isVerticalEnabled && hVelocity == 0)
        {
            var threshold = Bounds.Size.Height * 0.2;
            double bound = 0;

            vVelocity = -ComputeEdgeScrollVelocityFromEdgeDistance(dragPoint.Y, threshold);
            if (vVelocity == 0)
            {
                double height = Bounds.Size.Height;
                vVelocity = ComputeEdgeScrollVelocityFromEdgeDistance(height - dragPoint.Y, threshold);
                bound = extent.Height - viewport.Height;
            }

            // Disable if we're right up on the edge
            if (MathHelpers.IsClose(bound, offset.Y, 0.05))
            {
                vVelocity = 0;
            }
        }

        pVelocity = new Vector(hVelocity, vVelocity);
    }

    private static double ComputeEdgeScrollVelocityFromEdgeDistance(in double distFromEdge,
        double edgeDistanceThreshold = 100)
    {
        if (distFromEdge <= edgeDistanceThreshold)
        {
            return 200 - (distFromEdge / edgeDistanceThreshold) * (200 - 25);
        }

        return 0;
    }

    private void SetPendingAutoPanVelocity(Vector velocity)
    {
        if (!IsStationary(velocity))
        {
            if (!IsStationary(_currentAutoPanVelocity))
            {
                _currentAutoPanVelocity = velocity;
                EnsureStartEdgeScrollTimer();
            }
            else
            {
                _currentAutoPanVelocity = velocity;
            }

            // While AutoScrolling, be sure the live reorder manager isn't trying to
            // do anything as container bounds are constantly changing and things
            // won't line up like it expects
            _liveReorderHelper?.ResetAllItemsForLiveReorder();
        }
        else
        {
            // Already stationary: skip the per-event ScrollViewer offset write (which
            // re-raises scroll-changed and can force an extra render pass every frame).
            if (IsStationary(_currentAutoPanVelocity))
                return;

            DestroyStartEdgeScrollTimer();
            _currentAutoPanVelocity = default;
            ScrollWithVelocity(default);
        }
    }

    private void EnsureStartEdgeScrollTimer()
    {
        _scrollTimer ??= new DispatcherTimer(TimeSpan.FromMilliseconds(50),
            DispatcherPriority.Normal, StartEdgeScrollTimerTick);

        _scrollTimer.Start();
    }

    private void DestroyStartEdgeScrollTimer()
    {
        _scrollTimer?.Stop();
        _scrollTimer = null;
    }

    private void StartEdgeScrollTimerTick(object? sender, EventArgs args)
    {
        ScrollWithVelocity(_currentAutoPanVelocity);
    }

    private void ScrollWithVelocity(in Vector velocity)
    {
        var s = Scroller;
        if (s == null)
        {
            return;
        }

        var off = s.Offset;

        off += velocity;

        s.Offset = off;
    }

    private static bool IsStationary(Vector v) =>
        MathHelpers.IsZero(v.X) && MathHelpers.IsZero(v.Y);

    internal Orientation? GetLogicalOrientation()
    {
        var panel = ItemsPanelRoot;
        if (panel is VirtualizingStackPanel vsp)
            return vsp.Orientation;
        else if (panel is StackPanel sp)
            return sp.Orientation;

        return null;
    }

    internal void HandleTabStripLocationChanged(TabViewTabStripLocation newLocation, string? oldClass, string newClass)
    {
        if (oldClass != null)
            PseudoClasses.Set(oldClass, false);

        PseudoClasses.Set(newClass, true);

        if (Scroller != null)
        {
            if (oldClass != null)
                ((IPseudoClasses)Scroller.Classes).Set(oldClass, false);

            ((IPseudoClasses)Scroller.Classes).Set(newClass, true);
        }

        var panel = ItemsPanelRoot;
        if (panel != null)
        {
            foreach (var item in panel.Children)
            {
                if (item is TabViewItem tvi)
                {
                    tvi.HandleTabStripLocationChanged(newLocation);
                }
            }

            // If we have a Stacking Panel, adjust its orientation
            // If user uses any other type of panel, do nothing & log warning
            // User will need to monitor changes and adjust their panel accordingly
            if (panel is VirtualizingStackPanel vsp)
            {
                if (vsp.Orientation == Orientation.Vertical &&
                    (newLocation == TabViewTabStripLocation.Top || newLocation == TabViewTabStripLocation.Bottom))
                {
                    vsp.Orientation = Orientation.Horizontal;
                }
                else if (vsp.Orientation == Orientation.Horizontal &&
                    (newLocation == TabViewTabStripLocation.Left || newLocation == TabViewTabStripLocation.Right))
                {
                    vsp.Orientation = Orientation.Vertical;
                }
            }
            else if (panel is Avalonia.Controls.StackPanel sp)
            {
                if (sp.Orientation == Orientation.Vertical &&
                    (newLocation == TabViewTabStripLocation.Top || newLocation == TabViewTabStripLocation.Bottom))
                {
                    sp.Orientation = Orientation.Horizontal;
                }
                else if (sp.Orientation == Orientation.Horizontal &&
                    (newLocation == TabViewTabStripLocation.Left || newLocation == TabViewTabStripLocation.Right))
                {
                    sp.Orientation = Orientation.Vertical;
                }
            }
            else
            {
                Logger.Sink?.Log(LogEventLevel.Warning, "TabView", this,
                    "User has TabView with non-stacking panel, which may not be compatible with TabStripLocation changes");
            }
        }
    }

    private void UpdateBottomBorderVisualState()
    {
        int count = ItemsView.Count;
        PseudoClasses.Set(PC_LEFT_SHORT, count > 0 && SelectedIndex == 0);
        PseudoClasses.Set(PC_RIGHT_SHORT, count > 0 && SelectedIndex == count - 1);
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var tv = this.FindAncestorOfType<TabView>();
        tv?.OnItemsChanged(e);
    }

    private void UpdateDragInfo()
    {
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        UISettings.GetSystemDragSize(scaling, out _cxDrag, out _cyDrag);
    }

    // ---- Drag ghost & drop cursor ----

    /// <summary>
    /// Shows a floating snapshot of the dragged tab. It is hosted in the
    /// <see cref="OverlayLayer"/> so it renders above everything else and follows the
    /// pointer for the duration of the drag, making it clear which item is being moved.
    /// </summary>
    private void ShowDragGhost(TabViewItem? dragItem, double width, double height)
    {
        if (dragItem == null || _dragGhost != null)
            return;

        var topLevel = TopLevel.GetTopLevel(this);
        var layer = topLevel != null ? OverlayLayer.GetOverlayLayer(topLevel) : null;
        if (layer == null)
            return;

        var ghost = BuildDragGhost(dragItem, width, height);
        _dragGhost = ghost;
        _dragGhostLayer = layer;
        layer.Children.Add(ghost);

        // The ghost keeps the exact size the tab occupied in the list, so no measure is needed.
        _dragGhostSize = new Size(width, height);

        // Position via a render transform so updating it doesn't invalidate layout (which
        // makes the ghost follow the cursor much more smoothly than Canvas.Left/Top).
        _dragGhostTransform = new TranslateTransform(-10000, -10000);
        ghost.RenderTransform = _dragGhostTransform;
        Canvas.SetLeft(ghost, 0);
        Canvas.SetTop(ghost, 0);
    }

    private void HideDragGhost()
    {
        if (_dragGhost != null && _dragGhostLayer != null)
        {
            _dragGhostLayer.Children.Remove(_dragGhost);
        }

        _dragGhost = null;
        _dragGhostLayer = null;
        _dragGhostSize = default;
        _dragGhostTransform = null;
        _lastGhostPoint = null;
        _pendingGhostPoint = null;
        _ghostUpdateQueued = false;
    }

    private void UpdateDragGhostPosition(DragEventArgs e)
    {
        if (_dragGhost == null || _dragGhostLayer == null || _dragGhostTransform == null)
            return;

        var pt = e.GetPosition(_dragGhostLayer);
        var ghostPoint = new Point(pt.X - _dragGhostSize.Width / 2, pt.Y - _dragGhostSize.Height / 2);

        // Skip redundant updates (OLE fires a constant stream of DragOver events even when
        // the pointer hasn't moved).
        if (_lastGhostPoint.HasValue &&
            double.Abs(_lastGhostPoint.Value.X - ghostPoint.X) < 0.5 &&
            double.Abs(_lastGhostPoint.Value.Y - ghostPoint.Y) < 0.5)
        {
            return;
        }
        _lastGhostPoint = ghostPoint;
        _pendingGhostPoint = ghostPoint;

        // Coalesce per-event updates into a single transform write per render pass. OLE
        // delivers DragOver more often than the render loop, so applying the position on
        // every event caused redundant invalidations and stutter.
        if (!_ghostUpdateQueued)
        {
            _ghostUpdateQueued = true;
            Dispatcher.UIThread.Post(ApplyPendingGhostPosition, DispatcherPriority.Render);
        }
    }

    private void ApplyPendingGhostPosition()
    {
        _ghostUpdateQueued = false;

        if (_dragGhostTransform == null || _pendingGhostPoint is not { } pt)
            return;

        _dragGhostTransform.X = pt.X;
        _dragGhostTransform.Y = pt.Y;
    }

    private Border BuildDragGhost(TabViewItem dragItem, double width, double height)
    {
        // Fixed foreground/background colors derived from the current theme variant.
        bool isDark = AvaloniaFluentTheme.Instance.IsDarkTheme;
        var background = isDark ? Color.FromRgb(0x2C, 0x2C, 0x2C) : Color.FromRgb(0xFF, 0xFF, 0xFF);
        var border = isDark ? Color.FromRgb(0x40, 0x40, 0x42) : Color.FromRgb(0xE0, 0xE0, 0xE0);
        var foreground = isDark ? Color.FromRgb(0xFF, 0xFF, 0xFF) : Color.FromRgb(0x1A, 0x1A, 0x1A);

        var ghost = new Border
        {
            Width = width,
            Height = height,
            IsHitTestVisible = false,
            // Opacity 1.0 (instead of ~0.96) avoids forcing Avalonia to render the
            // ghost into its own offscreen surface and re-composite it on every frame
            // of the drag — a measurable source of jank when following the pointer.
            Opacity = 1.0,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(9, 3, 9, 3),
            CornerRadius = new CornerRadius(4),
            Background = new SolidColorBrush(background),
            BorderBrush = new SolidColorBrush(border),
            // A lighter shadow (6px blur vs 16px) keeps the floating look while shrinking
            // the rasterized shadow region the renderer has to produce/sample each frame.
            BoxShadow = BoxShadows.Parse("0 2 6 0 #1F000000"),
            // Bake the ghost (shadow + text + icon) into a cached bitmap once. While
            // the tab is being dragged the ghost content never changes, so the renderer
            // only has to translate that bitmap instead of re-rasterizing the blurred
            // box-shadow on every frame — the single biggest source of drag jank.
            CacheMode = new BitmapCache(),
        };

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
        };

        var icon = dragItem.IconSource;
        if (icon != null && icon is not IconElement)
        {
            panel.Children.Add(new FluentIconElement
            {
                Source = icon,
                MaxWidth = 16,
                MaxHeight = 16,
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(foreground),
            });
        }

        var header = dragItem.Header?.ToString();
        if (!string.IsNullOrEmpty(header))
        {
            panel.Children.Add(new TextBlock
            {
                Text = header,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12,
                Foreground = new SolidColorBrush(foreground),
            });
        }

        ghost.Child = panel;
        return ghost;
    }

    /// <summary>
    /// Applies a distinct cursor while a drag is in progress: a "move" cursor when the
    /// drop can be accepted here, and a "not allowed" cursor when it can't.
    /// </summary>
    private void SetDragCursor(bool accepted)
    {
        if (!_dragCursorApplied)
        {
            _previousCursor = Cursor;
            _previousTabViewCursor = (_parent as TabView)?.Cursor;
            _dragCursorApplied = true;
            _dragCursorAccepted = accepted;
            ApplyDragCursor(accepted ? DragMoveCursor : DragNoCursor);
        }
        else if (_dragCursorAccepted != accepted)
        {
            // Only touch the Cursor property when the accept state actually flips.
            _dragCursorAccepted = accepted;
            ApplyDragCursor(accepted ? DragMoveCursor : DragNoCursor);
        }
    }

    private void ApplyDragCursor(Cursor cursor)
    {
        Cursor = cursor;
        if (_parent is TabView tv)
        {
            tv.Cursor = cursor;
        }
    }

    private void ResetDragCursor()
    {
        if (!_dragCursorApplied)
            return;

        _dragCursorApplied = false;
        Cursor = _previousCursor;
        if (_parent is TabView tv)
        {
            tv.Cursor = _previousTabViewCursor;
        }
    }
}
