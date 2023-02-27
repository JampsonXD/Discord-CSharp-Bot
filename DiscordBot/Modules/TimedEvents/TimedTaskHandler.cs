using System.Collections.Concurrent;
using DiscordBot.Modules.TimedEvents.Interfaces;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Modules.TimedEvents;

public class TimedTaskHandler: BackgroundService
{
    private readonly List<ITimedTask> _timedTasks;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentBag<ITimedTask> _newTasks;
    private readonly object _lock = new();

    public TimedTaskHandler(List<ITimedTask> tasks, IServiceProvider serviceProvider)
    {
        _timedTasks = tasks;
        _serviceProvider = serviceProvider;
        _newTasks = new ConcurrentBag<ITimedTask>();
    }

    public TimedTaskHandler(IServiceProvider serviceProvider) : this(new List<ITimedTask>(), serviceProvider)
    {
        
    }

    private async Task HandleTask(ITimedTask task)
    {
        if (task.CanExecuteTask().Result)
        {
            await task.ExecuteTask();
            await task.ResetTask();
        }
    }

    public void AddTask(ITimedTask newTask)
    {
        _newTasks.Add(newTask);
    }

    public void AddTasks(IEnumerable<ITimedTask> tasks)
    {
        foreach (var task in tasks)
        {
            _newTasks.Add(task);
        }
    }

    public bool FindTask(Func<ITimedTask, bool> func, out ITimedTask? foundTask)
    {
        lock (_lock)
        {
            foundTask = null;
            foreach (var task in _timedTasks)
            {
                if (func(task))
                {
                    foundTask = task;
                    return true;
                }
            }
        }

        return false;
    }
    
    public void RemoveTask(ITimedTask task)
    {
        lock (_lock)
        {
            _timedTasks.Remove(task);
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Initialize the tasks
        lock (_lock)
        {
            Task.WaitAll(_timedTasks.Select(task => task.InitializeTask(_serviceProvider)).ToArray());
        }

        // Continue our loop until we are asked to stop the tasks
        while (!stoppingToken.IsCancellationRequested)
        {
            // Any exceptions thrown from tasks should not stop our handler from running, log the exceptions
            try
            {
                lock (_lock)
                {
                    while(!_newTasks.IsEmpty)
                    {
                        if (_newTasks.TryTake(out var newTask))
                        {
                            newTask.InitializeTask(_serviceProvider);
                            _timedTasks.Add(newTask);
                        }

                        break;
                    }
                    
                    var tasks = _timedTasks.Select(HandleTask);
                    Task.WaitAll(tasks.ToArray(), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        return Task.CompletedTask;
    }
}