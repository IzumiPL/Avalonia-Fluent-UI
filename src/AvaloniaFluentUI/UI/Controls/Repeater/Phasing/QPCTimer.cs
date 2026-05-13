using System.Diagnostics;

namespace AvaloniaFluentUI.UI.Controls;

internal class QPCTimer
{
    public void Reset()
    {
        _stopwatch.Restart();
    }

    public long DurationInMilliseconds()
    {
        return _stopwatch.ElapsedMilliseconds;
    }

    private readonly Stopwatch _stopwatch = new Stopwatch();
}
