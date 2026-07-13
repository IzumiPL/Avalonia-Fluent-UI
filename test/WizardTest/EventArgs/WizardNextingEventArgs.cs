using System.ComponentModel;

namespace WizardTest.EventArgs;

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
