using System;
using System.Runtime.InteropServices;
using Avalonia.Threading;

namespace Gallery;

internal static class MemoryTrim
{
    private const long MAX_WORKING_SET = 250L * 1024 * 1024;
    /// <summary>Freed bytes below which returning the pages is not worth the page faults.</summary>
    private const long WORTH_RETURING = 4L * 1024 * 1024;
    
    /// <summary>How long the client has to be left alone before a pass runs.</summary>
    private static readonly TimeSpan QuietPeriod = TimeSpan.FromMilliseconds(600);

    /// <summary>Closest two passes may be. Housekeeping must never become the busy work.</summary>
    private static readonly TimeSpan MinimumGap = TimeSpan.FromSeconds(15);

    private static DispatcherTimer? _timer;
    private static DateTime _lastPass = DateTime.MinValue;

    /// <summary>
    /// Asks for a pass, from any thread. Repeated requests inside the quiet period are folded
    /// into the one that runs when it ends, so closing ten tabs collects once.
    /// </summary>
    public static void Request()
    {
        // Nothing to trim before the dispatcher exists (design-time previews, tests).
        if (Avalonia.Application.Current is null)
        {
            return;
        }

        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(Request);
            return;
        }

        if (_timer is null)
        {
            _timer = new DispatcherTimer { Interval = QuietPeriod };
            _timer.Tick += OnQuiet;
        }
        else
        {
            // Restarting the timer is what makes the requests fold together: the pass runs
            // one quiet period after the last of them, not one period after the first.
            _timer.Stop();
        }

        _timer.Start();
    }

    private static void OnQuiet(object? sender, EventArgs e)
    {
        _timer?.Stop();

        if (DateTime.UtcNow - _lastPass < MinimumGap)
        {
            // Asked again sooner than is polite; the memory is not going anywhere, and the
            // next request after the gap will pick it up.
            return;
        }

        _lastPass = DateTime.UtcNow;

        var before = GC.GetTotalMemory(forceFullCollection: false);

        // A full collection on the UI thread stops it, so it is deliberately not compacting:
        // compaction is the expensive half and the freed segments are given back anyway.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: false);
        GC.WaitForPendingFinalizers();

        var after = GC.GetTotalMemory(forceFullCollection: true);
        if (before - after < WORTH_RETURING)
        {
            return;
        }

        ReturnPages();
    }

    /// <summary>
    /// Empties the working set: the pages the process still maps but has not touched are
    /// dropped, and come back only if they are used again. -1 for both sizes is the documented
    /// way of saying "work it out yourself".
    /// </summary>
    private static void ReturnPages()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            using var process = System.Diagnostics.Process.GetCurrentProcess();

            if (process.WorkingSet64 > MAX_WORKING_SET)
            {
                SetProcessWorkingSetSize(GetCurrentProcess(), -1, -1); 
            }
        }
        catch (Exception)
        {
            // A refused trim is a missed optimisation, never a failure the user should see.
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessWorkingSetSize(nint process, nint minimum, nint maximum);
}
