using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class OutlinePushButton : ToggleButton
{
   public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<OutlinePushButton, Geometry?>(nameof(IconData));

    public Geometry? IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<OutlinePushButton, double>(nameof(IconWidth));

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<OutlinePushButton, double>(nameof(IconHeight));

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
    
    public static readonly StyledProperty<string?> GroupNameProperty =
        AvaloniaProperty.Register<OutlinePushButton, string?>(nameof(GroupName));

    public string? GroupName
    {
        get => GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }

    private static readonly Dictionary<string, List<WeakReference<OutlinePushButton>>> Groups = new();

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        RegisterToGroup();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        UnregisterFromGroup();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == GroupNameProperty)
        {
            RegisterToGroup();
            // UnregisterFromGroup();
            UncheckGroup();
        }
    }

    private void RegisterToGroup()
    {
        if (String.IsNullOrWhiteSpace(GroupName)) { return; }

        if (!Groups.TryGetValue(GroupName, out var list))
        {
            list = new List<WeakReference<OutlinePushButton>>();

            Groups.Add(GroupName, list);
        }

        list.Add(new WeakReference<OutlinePushButton>(this));
    }

    private void UnregisterFromGroup()
    {
        if (String.IsNullOrWhiteSpace(GroupName)) { return; }
        if (!Groups.TryGetValue(GroupName, out var list)) { return; }

        list.RemoveAll(x =>
        {
            if (!x.TryGetTarget(out var button)) { return true; }

            return button == this;
        });

        if (list.Count == 0)
        {
            Groups.Remove(GroupName);
        }
    }

    protected override void Toggle()
    {
        bool newValue = !(IsChecked ?? false);
        SetCurrentValue(IsCheckedProperty, newValue);

        if (newValue && !String.IsNullOrWhiteSpace(GroupName))
        {
            UncheckGroup();
        }
    }

    private void UncheckGroup()
    {
        if (String.IsNullOrWhiteSpace(GroupName)) { return; }

        if (Groups.TryGetValue(GroupName, out var list))
        {
            list.RemoveAll(x => !x.TryGetTarget(out _));
            {
                foreach (var weak in list)
                {
                    if (weak.TryGetTarget(out var button))
                    {
                        if (button == this) { continue; }

                        button.SetCurrentValue(IsCheckedProperty, false);
                    }
                }
            }
        }
    }
}
