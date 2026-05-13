#pragma warning disable
using System;
using Avalonia.Controls;

namespace AvaloniaFluentUI.UI.Controls;

public class ItemCollectionTransitionProgress
{
    public ItemCollectionTransitionProgress(ItemCollectionTransition transition)
    {
        _transition = new WeakReference<ItemCollectionTransition>(transition);
    }

    public Control Element() =>
        _transition.TryGetTarget(out var target) ? target.Element : null;

    public void Complete()
    {
        if (_transition.TryGetTarget(out var target))
        {
            target.OwningProvider.NotifyTransitionComplete(target);
        }
    }

    private WeakReference<ItemCollectionTransition> _transition;
}
