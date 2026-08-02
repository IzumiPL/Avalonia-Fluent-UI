using System;
using Avalonia.Controls;

namespace AvaloniaFluentUI.Controls;

public class TimeLine : ItemsControl 
{
    protected override Type StyleKeyOverride => typeof(ItemsControl);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new TimeLineItem();
    }
    
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not TimeLineItem;
    }
}
