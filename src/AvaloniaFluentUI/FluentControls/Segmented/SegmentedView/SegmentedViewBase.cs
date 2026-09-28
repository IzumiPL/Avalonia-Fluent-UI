using System;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AvaloniaFluentUI.Controls;

public abstract class SegmentedViewBase : TemplatedControl 
{
    /// <summary>
    /// Defines the <see cref="SegmentedItems"/> property.
    /// </summary>
    public static readonly StyledProperty<IList?> SegmentedItemsProperty =
        AvaloniaProperty.Register<SegmentedViewBase, IList?>(nameof(SegmentedItems));

    /// <summary>
    ///     Defines the <see cref="BarHorizontalAlignment" /> property.
    /// </summary>
    public static readonly StyledProperty<HorizontalAlignment> BarHorizontalAlignmentProperty =
        AvaloniaProperty.Register<SegmentedViewBase, HorizontalAlignment>(nameof(BarHorizontalAlignment));

    /// <summary>
    ///     Defines the <see cref="Spacing" /> property.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<SegmentedViewBase, double>(nameof(Spacing), 3.0);

    /// <summary>
    /// 获取或设置滑动栏和底部内容的间距
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public HorizontalAlignment BarHorizontalAlignment
    {
        get => GetValue(BarHorizontalAlignmentProperty);
        set => SetValue(BarHorizontalAlignmentProperty, value);
    }

    [Content]
    public IList? SegmentedItems
    {
        get => GetValue(SegmentedItemsProperty);
        set => SetValue(SegmentedItemsProperty, value);
    }

    protected SegmentedViewBase()
    {
        SegmentedItems = new AvaloniaList<object>();
    }
}


public abstract class SegmentedViewBase<TBar> : SegmentedViewBase where TBar : SegmentedBar
{
    public const string PART_CONTENT_PRESENTER = "PART_ContentPresenter";

    private TBar? _segmentedBar;
    private ContentPresenter? _contentPresenter;
    private INotifyCollectionChanged? _observedItems;
    private int _previousSelectedIndex = -1;

    /// <summary>
    /// 获取当前模板中的标题栏控件。
    /// </summary>
    public TBar? SegmentedBar => _segmentedBar;

    public event EventHandler<SelectedItemChangedEventArgs>? SelectedItemChanged;

    /// <summary>
    /// 从模板中查找标题栏。
    /// </summary>
    protected abstract TBar? FindSegmentedBar(TemplateAppliedEventArgs e);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _segmentedBar?.SelectionChanged -= OnSegmentedBarSelectionChanged;
        _contentPresenter?.PropertyChanged -= OnContentPresenterContentChanged;

        base.OnApplyTemplate(e);

        _segmentedBar = FindSegmentedBar(e);
        _contentPresenter = e.NameScope.Find<ContentPresenter>(PART_CONTENT_PRESENTER);
        
        _segmentedBar?.SelectionChanged += OnSegmentedBarSelectionChanged;
        _contentPresenter?.PropertyChanged += OnContentPresenterContentChanged;

        ObserveItems();
        SynchronizeBarItems();
        UpdateContentPresenter();
    }

    private CancellationTokenSource? _cts;
    private double _targetOffset;

    private void OnContentPresenterContentChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_contentPresenter != null && e.Property == ContentPresenter.ContentProperty && e.NewValue != e.OldValue)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Dispatcher.UIThread.Post(() => RunSlideAnimation(_contentPresenter, token), DispatcherPriority.Render);
        }
    }

    private async void RunSlideAnimation(Control control, CancellationToken cancellationToken)
    {
        if (_targetOffset == 0d)
            return;

        var animation = new Animation
        {
            Easing = new SplineEasing(0.1, 0.9, 0.2, 1.0),
            Duration = TimeSpan.FromMilliseconds(300),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = { new Setter(TranslateTransform.XProperty, _targetOffset) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(TranslateTransform.XProperty, 0d) }
                }
            }
        };

        await animation.RunAsync(control, cancellationToken);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SegmentedItemsProperty)
        {
            ObserveItems();
            SynchronizeBarItems();
            UpdateContentPresenter();
        }
    }

    private void ObserveItems()
    {
        if (ReferenceEquals(_observedItems, SegmentedItems))
            return;

        _observedItems?.CollectionChanged -= OnSegmentedItemsCollectionChanged;

        _observedItems = SegmentedItems as INotifyCollectionChanged;
        _observedItems?.CollectionChanged += OnSegmentedItemsCollectionChanged;
    }

    private void OnSegmentedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SynchronizeBarItems();
        UpdateContentPresenter();
    }

    private void SynchronizeBarItems()
    {
        if (_segmentedBar is null)
            return;

        var items = SegmentedItems;
        var oldSelectedIndex = _segmentedBar.SelectedIndex;

        _segmentedBar.Items.Clear();

        if (items is not null)
        {
            foreach (var item in items)
            {
                _segmentedBar.Items.Add(GetHeader(item));
            }
        }

        if (_segmentedBar.Items.Count == 0)
        {
            _segmentedBar.SelectedIndex = -1;
            return;
        }

        if (oldSelectedIndex >= 0 && oldSelectedIndex < _segmentedBar.Items.Count)
        {
            _segmentedBar.SelectedIndex = oldSelectedIndex;
        }
        else
        {
            _segmentedBar.SelectedIndex = FindInitiallySelectedIndex();
        }
    }

    private int FindInitiallySelectedIndex()
    {
        var items = SegmentedItems;
        if (items is not null)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i] is ISegmentedItem { IsSelected: true })
                {
                    return i;
                }
            }
        }

        return 0;
    }

    private void UpdateContentPresenter()
    {
        if (_contentPresenter is null || _segmentedBar is null) 
            return;

        var index = _segmentedBar.SelectedIndex;
        var items = SegmentedItems;

        object? content = null;

        if (items is not null && index >= 0 && index < items.Count)
        {
            content = GetContent(items[index]);
        }

        _contentPresenter.Content = content;

        UpdateItemsSelection();
    }

    /// <summary>
    /// 根据当前选中索引同步每个分段项的 <see cref="ISegmentedItem.IsSelected"/>。
    /// </summary>
    private void UpdateItemsSelection()
    {
        var items = SegmentedItems;
        if (items is null)
        {
            return;
        }

        var selectedIndex = _segmentedBar?.SelectedIndex ?? -1;

        for (var i = 0; i < items.Count; i++)
        {
            if (items[i] is ISegmentedItem segItem)
            {
                segItem.IsSelected = i == selectedIndex;
            }
        }
    }
    
    protected static object? GetHeader(object? item)
    {
        return item is HeaderedContentControl headered ? headered.Header : item;
    }

    protected static object? GetContent(object? item)
    {
        return item is HeaderedContentControl headered ? headered.Content : item;
    }

    private object? GetSegmentedItem(int index)
    {
        var items = SegmentedItems;
        if (items is not null && index >= 0 && index < items.Count)
        {
            return items[index];
        }
        return null;
    }

    private void OnSegmentedBarSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_segmentedBar is null) 
            return;

        var newIndex = _segmentedBar.SelectedIndex;
        var oldIndex = _previousSelectedIndex;

        _targetOffset = 0; 
        var value = _contentPresenter?.Bounds.Width ?? 96;
        if (oldIndex >= 0 && newIndex >= 0)
        {
            _targetOffset = newIndex > oldIndex ? value : -value;
        }

        var oldItem = GetSegmentedItem(oldIndex);
        var newItem = GetSegmentedItem(newIndex);

        _previousSelectedIndex = newIndex;

        if (oldIndex != newIndex)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(oldIndex, newIndex, oldItem, newItem));
        }

        UpdateContentPresenter();
    }
}
