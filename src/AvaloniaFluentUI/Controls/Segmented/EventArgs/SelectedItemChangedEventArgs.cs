namespace AvaloniaFluentUI.Controls;

public class SelectedItemChangedEventArgs
{
    /// <summary>
    /// 初始化 <see cref="SelectedItemChangedEventArgs"/>。
    /// </summary>
    /// <param name="oldIndex">变化前的索引，首次选中时为 -1。</param>
    /// <param name="newIndex">变化后的索引。</param>
    /// <param name="oldItem">变化前选中的分段项（<see cref="SegmentedViewBase.SegmentedItems"/> 中的原始元素）。</param>
    /// <param name="newItem">变化后选中的分段项（<see cref="SegmentedViewBase.SegmentedItems"/> 中的原始元素）。</param>
    public SelectedItemChangedEventArgs(int oldIndex, int newIndex, object? oldItem, object? newItem)
    {
        OldIndex = oldIndex;
        NewIndex = newIndex;
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>变化前的索引，首次选中时为 -1。</summary>
    public int OldIndex { get; }

    /// <summary>变化后的索引。</summary>
    public int NewIndex { get; }

    /// <summary>变化前选中的分段项。</summary>
    public object? OldItem { get; }

    /// <summary>变化后选中的分段项。</summary>
    public object? NewItem { get; }
}
