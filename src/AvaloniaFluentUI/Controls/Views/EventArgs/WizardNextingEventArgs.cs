using System.ComponentModel;

namespace AvaloniaFluentUI.Controls;

public class WizardNextingEventArgs : CancelEventArgs
{
    public int CurrentIndex { get; set; }
    public int NextIndex { get; set; }

    public WizardNextingEventArgs(int currentIndex, int nextIndex)
    {
        CurrentIndex = currentIndex;
        NextIndex = nextIndex;
    }
}
