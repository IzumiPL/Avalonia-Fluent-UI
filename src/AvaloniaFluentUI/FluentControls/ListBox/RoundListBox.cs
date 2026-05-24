using Avalonia.Controls;

namespace AvaloniaFluentUI.Controls;

public class RoundListBox : ListBox
{
    protected override Control CreateContainerForItemOverride(object item, int index, object recycleKey)
    {
        return new RoundListBoxItem();
    }

    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey)
    {
        return NeedsContainer<RoundListBoxItem>(item, out recycleKey);
    }
}
