using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaFluentUI.Controls;

public enum OutlineButtonSelectionMode
{
    Single,
    Multiple
}

public class OutlineButtonGroup
{
    private readonly List<WeakReference<OutlineButtonBase>> _members = new();

    public OutlineButtonSelectionMode SelectionMode { get; set; } = OutlineButtonSelectionMode.Multiple;

    public IReadOnlyList<OutlineButtonBase> SelectedItems =>
        _members.Select(w => w.TryGetTarget(out var b) ? b : null)
            .Where(b => b is { IsChecked: true })
            .ToList()!;

    public OutlineButtonBase? SelectedItem => SelectedItems.FirstOrDefault();

    public event Action<OutlineButtonGroup>? SelectionChanged;

    internal void Register(OutlineButtonBase button)
    {
        foreach (var weak in _members)
        {
            if (weak.TryGetTarget(out var existing) && existing == button) return;
        }
        _members.Add(new WeakReference<OutlineButtonBase>(button));
    }

    internal void Unregister(OutlineButtonBase button)
    {
        _members.RemoveAll(w => !w.TryGetTarget(out _) || (w.TryGetTarget(out var b) && b == button));
    }

    internal void OnButtonCheckedChanged(OutlineButtonBase source)
    {
        if (SelectionMode == OutlineButtonSelectionMode.Single && source.IsChecked == true)
            UncheckOthers(source);

        SelectionChanged?.Invoke(this);
    }

    private void UncheckOthers(OutlineButtonBase exclude)
    {
        foreach (var weak in _members)
        {
            if (weak.TryGetTarget(out var btn) && btn != exclude && btn.IsChecked == true)
                btn.SetCurrentValue(OutlineButtonBase.IsCheckedProperty, false);
        }
    }
}
