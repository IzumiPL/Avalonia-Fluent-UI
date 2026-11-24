using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// 分段视图：组合一个 <see cref="SegmentedBar"/>（标题栏）与一个内容区。
/// 每个 <see cref="SegmentedItem"/> 的 Header 显示在 SegmentedBar 中；
/// 切换 SegmentedBarItem 时，对应 SegmentedItem 的 Content 自动显示到内容区。
/// </summary>
public class SegmentedView : SegmentedViewBase<SegmentedBar>
{
    private const string PART_SEGMENTED_BAR = "PART_SegmentedBar";

    protected override SegmentedBar? FindSegmentedBar(TemplateAppliedEventArgs e)
    {
        return e.NameScope.Find<SegmentedBar>(PART_SEGMENTED_BAR);
    }
}


[PseudoClasses(PC_SELECTED)]
public class SegmentedItem : HeaderedContentControl, ISegmentedItem
{
    public const string PC_SELECTED = ":selected";

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<SegmentedItem>();

    /// <summary>获取或设置当前分段项是否被选中。</summary>
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsSelectedProperty)
        {
            PseudoClasses.Set(PC_SELECTED, change.GetNewValue<bool>());
        }
    }
}
