using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaFluentUI.Core;

/// <summary>
/// 可控的托管内存回收请求调度器
/// 不修改进程工作集，由 .NET 运行时管理物理内存
/// </summary>
public static class MemoryTrim
{
    /// <summary>
    /// 最后一次请求后等待多久，才执行回收
    /// 连续请求会被合并
    /// </summary>
    private static readonly TimeSpan QuietPeriod = TimeSpan.FromMilliseconds(600);

    /// <summary>
    /// 两次回收之间的最小间隔。
    /// </summary>
    private static readonly TimeSpan MinimumGap = TimeSpan.FromSeconds(15);

    private static readonly object SyncRoot = new();

    private static CancellationTokenSource? _pendingCts;
    private static long _lastTrimTimestamp;

    /// <summary>
    /// 请求回收内存
    /// 适用于页面切换、关闭大型视图或释放大量资源之后
    /// 多次连续调用会自动合并
    /// </summary>
    public static void RequestTrim()
    {
        lock (SyncRoot)
        {
            _pendingCts?.Cancel();

            var cts = new CancellationTokenSource();
            _pendingCts = cts;

            _ = TrimAfterDelayAsync(cts);
        }
    }

    private static async Task TrimAfterDelayAsync(CancellationTokenSource cts)
    {
        var token = cts.Token;

        try
        {
            // 等待请求静默期，避免短时间内重复回收
            await Task.Delay(QuietPeriod, token).ConfigureAwait(false);

            while (true)
            {
                token.ThrowIfCancellationRequested();
                TimeSpan remaining;

                lock (SyncRoot)
                {
                    if (!ReferenceEquals(_pendingCts, cts))
                        return;

                    if (_lastTrimTimestamp == 0)
                    {
                        remaining = TimeSpan.Zero;
                    }
                    else
                    {
                        var elapsed = StopwatchElapsedSince(_lastTrimTimestamp);

                        remaining = MinimumGap - elapsed;
                    }
                }

                if (remaining <= TimeSpan.Zero)
                    break;

                // 如果最小间隔尚未满足，等待剩余时间
                // 等待期间的新请求可以取消当前任务
                await Task.Delay(remaining, token).ConfigureAwait(false);
            }

            token.ThrowIfCancellationRequested();

            lock (SyncRoot)
            {
                if (!ReferenceEquals(_pendingCts, cts))
                    return;

                _lastTrimTimestamp = Environment.TickCount64;
                _pendingCts = null;
            }

            // 在后台线程发起非阻塞 GC 请求
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, blocking: false, compacting: false);
        }
        catch (OperationCanceledException)
        {
            //
        }
        finally
        {
            lock (SyncRoot)
            {
                if (ReferenceEquals(_pendingCts, cts))
                    _pendingCts = null;
            }

            cts.Dispose();
        }
    }

    private static TimeSpan StopwatchElapsedSince(long timestamp)
    {
        var elapsedMilliseconds = Environment.TickCount64 - timestamp;

        return TimeSpan.FromMilliseconds(Math.Max(0, elapsedMilliseconds));
    }
}
