#pragma warning disable
using System;
using Avalonia.Controls;

namespace AvaloniaFluentUI.UI.Controls;

public class ItemCollectionTransitionCompletedEventArgs : EventArgs
{
    public ItemCollectionTransitionCompletedEventArgs(ItemCollectionTransition transition)
    {

    }

    public ItemCollectionTransition Transition { get; }

    public Control Element { get; }
}
