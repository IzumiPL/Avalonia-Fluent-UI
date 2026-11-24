using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// 分段切换视图：与 <see cref="SegmentedView"/> 类似，但标题栏使用 <see cref="SegmentedToggleBar"/>。
/// 切换 SegmentedToggleBarItem 时，对应 SegmentedToggleItem 的 Content 自动显示到内容区。
/// </summary>
public class SegmentedToggleView : SegmentedViewBase<SegmentedToggleBar>
{
    private const string PART_SEGMENTED_TOGGLE_BAR = "PART_SegmentedToggleBar";

    protected override SegmentedToggleBar? FindSegmentedBar(TemplateAppliedEventArgs e)
    {
        return e.NameScope.Find<SegmentedToggleBar>(PART_SEGMENTED_TOGGLE_BAR);
    }
}

public class SegmentedToggleItem : SegmentedItem 
{
}
