using System.Collections.Generic;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;

namespace AvaloniaFluentUI.Controls;

public sealed class NavigationViewAutomationPeer : ControlAutomationPeer, ISelectionProvider
{
    public NavigationViewAutomationPeer(Control owner) : base(owner)
    {
    }

    public bool CanSelectMultiple => false;
    public bool IsSelectionRequired => false;

    public IReadOnlyList<AutomationPeer>? GetSelection()
    {
        if (Owner is NavigationView nv)
        {
            var nvi = nv.GetSelectedContainer();
            var peer = CreatePeerForElement(nvi);
            return new[] { peer };
        }

        return null;
    }

    internal void RaiseSelectionChangedEvent(object oldSelection, object newSelection)
    {
        if (Owner is NavigationView nv && nv.GetSelectedContainer() is NavigationViewItem nvi)
        {
            var peer = CreatePeerForElement(nvi);
            peer.RaisePropertyChangedEvent(SelectionPatternIdentifiers.SelectionProperty, oldSelection, newSelection);
        }
    }
}
