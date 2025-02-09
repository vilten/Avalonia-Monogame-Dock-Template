using System.Collections.Concurrent;
using System.Threading;

namespace Avalonia_Monogame_Dock_Template.Threads;

public class SingleThreadSynchronizationContext : SynchronizationContext
{
    private readonly BlockingCollection<(SendOrPostCallback, object?)> _queue = new();

    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Add((d, state));
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        var waitHandle = new ManualResetEvent(false);
        _queue.Add((s =>
        {
            d(s);
            waitHandle.Set();
        }, state));

        waitHandle.WaitOne();
    }

    public void RunOnCurrentThread()
    {
        while (_queue.TryTake(out var workItem, Timeout.Infinite))
        {
            workItem.Item1(workItem.Item2);
        }
    }
}
