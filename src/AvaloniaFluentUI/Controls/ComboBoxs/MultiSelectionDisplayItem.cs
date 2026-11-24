using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_REMOVE_BUTTON, Type = typeof(Button))]
public class MultiSelectionDisplayItem : ContentControl
{
    public static readonly RoutedEvent<RoutedEventArgs> RemoveClickEvent =
        RoutedEvent.Register<MultiSelectionDisplayItem, RoutedEventArgs>(nameof(RemoveClick), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs> RemoveClick
    {
        add => AddHandler(RemoveClickEvent, value);
        remove => RemoveHandler(RemoveClickEvent, value);
    }

    private Button? _removeButton;
    
    private const string PART_REMOVE_BUTTON =  "PART_RemoveButton";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _removeButton?.Click -= OnRemoveButtonClick;
        _removeButton = e.NameScope.Find<Button>("PART_RemoveButton");
        
        _removeButton?.Click += OnRemoveButtonClick;
    }

    private void OnRemoveButtonClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(RemoveClickEvent));
    }
}
