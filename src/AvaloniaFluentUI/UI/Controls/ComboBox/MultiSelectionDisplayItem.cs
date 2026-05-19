using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.UI.Controls;

public class MultiSelectionDisplayItem : ContentControl
{
    public static readonly RoutedEvent<RoutedEventArgs> RemoveClickEvent =
        RoutedEvent.Register<MultiSelectionDisplayItem, RoutedEventArgs>(nameof(RemoveClick), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs> RemoveClick
    {
        add => AddHandler(RemoveClickEvent, value);
        remove => RemoveHandler(RemoveClickEvent, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (e.NameScope.Find<Button>("PART_RemoveButton") is { } removeButton)
        {
            removeButton.Click += OnRemoveButtonClick;
        }
    }

    private void OnRemoveButtonClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(RemoveClickEvent));
    }
}
