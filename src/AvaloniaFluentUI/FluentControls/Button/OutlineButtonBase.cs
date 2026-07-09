using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AvaloniaFluentUI.Controls;

public class OutlineButtonBase : ToggleButton
{
    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<OutlineButtonBase, double>(nameof(IconWidth), 18);

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<OutlineButtonBase, double>(nameof(IconHeight), 18);

    public static readonly StyledProperty<OutlineButtonGroup?> GroupProperty =
        AvaloniaProperty.Register<OutlineButtonBase, OutlineButtonGroup?>(nameof(Group));

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    public OutlineButtonGroup? Group
    {
        get => GetValue(GroupProperty);
        set => SetValue(GroupProperty, value);
    }

    private static readonly List<WeakReference<OutlineButtonBase>> Ungrouped = new();

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Register();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == GroupProperty)
        {
            var oldGroup = change.OldValue as OutlineButtonGroup;
            Unregister(oldGroup);
            Register();

            if (Group == null && IsChecked == true)
            {
                UncheckOtherUngrouped();
            }
        }
        else if (change.Property == IsCheckedProperty)
        {
            HandleCheckedChanged();
        }
    }

    protected override void Toggle()
    {
        if (Group?.SelectionMode == OutlineButtonSelectionMode.Multiple)
        {
            SetCurrentValue(IsCheckedProperty, !(IsChecked ?? false));
        }
        else
        {
            if (IsChecked == true) return;
            SetCurrentValue(IsCheckedProperty, true);
        }
    }

    private void Register()
    {
        if (Group != null)
        {
            Group.Register(this);
        }
        else
        {
            PruneDead(Ungrouped);
            foreach (var weak in Ungrouped)
            {
                if (weak.TryGetTarget(out var existing) && existing == this) return;
            }
            Ungrouped.Add(new WeakReference<OutlineButtonBase>(this));
        }
    }

    private void Unregister(OutlineButtonGroup? group)
    {
        if (group != null)
        {
            group.Unregister(this);
        }
        else
        {
            Ungrouped.RemoveAll(w => !w.TryGetTarget(out _) || (w.TryGetTarget(out var b) && b == this));
        }
    }

    private void HandleCheckedChanged()
    {
        if (Group == null)
        {
            if (IsChecked == true)
                UncheckOtherUngrouped();
        }
        else
        {
            Group.OnButtonCheckedChanged(this);
        }
    }

    /// <summary>
    /// 取消其他未分组按钮的选中状态
    /// </summary>
    private void UncheckOtherUngrouped()
    {
        PruneDead(Ungrouped);
        foreach (var weak in Ungrouped)
        {
            if (weak.TryGetTarget(out var btn) && btn != this && btn.IsChecked == true)
                btn.SetCurrentValue(IsCheckedProperty, false);
        }
    }

    private static void PruneDead(List<WeakReference<OutlineButtonBase>> members)
    {
        members.RemoveAll(w => !w.TryGetTarget(out _));
    }
}
