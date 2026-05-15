using Avalonia.Controls;

namespace AvaloniaFluentUI.UI.Controls;

public class RoundListBox : ListBox
{
    protected override Control CreateContainerForItemOverride(object item, int index, object recycleKey)
    {
        return new RoundListBoxItem();
        // return base.CreateContainerForItemOverride(item, index, recycleKey);
    }

    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey)
    {
        return NeedsContainer<RoundListBoxItem>(item, out recycleKey);
        // return base.NeedsContainerOverride(item, index, out recycleKey);
    }
}
