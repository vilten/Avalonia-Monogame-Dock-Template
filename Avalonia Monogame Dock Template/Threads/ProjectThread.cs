using System;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia_Monogame_Dock_Template.Threads;

public class ProjectThread
{
    private readonly Thread _thread;
    private readonly SingleThreadSynchronizationContext _syncContext = new();
    private TaskScheduler _taskScheduler;

    public static ProjectThread Instance { get; } = new ProjectThread();

    private ProjectThread()
    {
        _thread = new Thread(ThreadLoop)
        {
            IsBackground = true,
            Name = "ProjectThread"
        };
        _thread.Start();

        // Počkáme, kým sa task scheduler inicializuje
        while (_taskScheduler == null)
        {
            Thread.Sleep(10);
        }
    }

    private void ThreadLoop()
    {
        SynchronizationContext.SetSynchronizationContext(_syncContext);
        _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        _syncContext.RunOnCurrentThread();
    }

    public void Run(Action action)
    {
        Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    public async Task RunVoidAsync(Func<Task> func)
    {
        await Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, _taskScheduler).Unwrap();
    }

    public async Task<T> RunAsync<T>(Func<T> func)
    {
        return await Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }
}
