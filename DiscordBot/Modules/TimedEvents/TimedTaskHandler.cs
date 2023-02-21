using DiscordBot.Modules.TimedEvents.Interfaces;

namespace DiscordBot.Modules.TimedEvents;

public class TimedTaskHandler
{
    private readonly List<ITimedTask> _timedTasks;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _taskActive;
    private readonly IServiceProvider _serviceProvider;
    private readonly object _lock = new();

    public TimedTaskHandler(IServiceProvider serviceProvider)
    {
        _timedTasks = new List<ITimedTask>();
        _taskActive = false;
        _serviceProvider = serviceProvider;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start()
    {
        if (!_taskActive)
        {
            _taskActive = true;
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => Run(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }
    }

    private Task Run(CancellationToken token)
    {
        // Initialize the tasks
        lock (_lock)
        {
            Task.WaitAll(_timedTasks.Select(task => task.InitializeTask(_serviceProvider)).ToArray());
        }

        // Continue our loop until we are asked to stop the tasks
        while (!token.IsCancellationRequested)
        {
            lock (_lock)
            {
                var tasks = _timedTasks.Select(HandleTask);
                Task.WaitAll(tasks.ToArray(), token);
            }
        }

        return Task.CompletedTask;
    }

    private async Task HandleTask(ITimedTask task)
    {
        if (task.CanExecuteTask().Result)
        {
            await task.ExecuteTask();
            await task.ResetTask();
        }
    }
    
    public void Stop()
    {
        if (_taskActive)
        {
            _taskActive = true;
            _cancellationTokenSource.Cancel();
        }
    }

    public void AddTask(ITimedTask newTask)
    {
        lock (_lock)
        {
            _timedTasks.Add(newTask);
            newTask.InitializeTask(_serviceProvider);
        }
    }

    public void AddTasks(IEnumerable<ITimedTask> tasks)
    {
        lock (_lock)
        {
            var newTasks = tasks.ToArray();
            _timedTasks.AddRange(newTasks);
            foreach (var task in newTasks)
            {
                task.InitializeTask(_serviceProvider);
            }
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
}