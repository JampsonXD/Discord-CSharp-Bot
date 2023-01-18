using Discord_CSharp_Bot.modules.timed_events.interfaces;

namespace Discord_CSharp_Bot.modules.timed_events;

public class TimedTaskHandler
{
    private List<ITimedTask> _timedTasks;
    private bool _endTask;
    private readonly IServiceProvider _serviceProvider;
    private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

    public TimedTaskHandler(IServiceProvider serviceProvider)
    {
        _timedTasks = new List<ITimedTask>();
        _endTask = false;
        _serviceProvider = serviceProvider;
    }

    public async Task Run()
    {
        // Initialize the tasks
        _timedTasks.ForEach(task => task.InitializeTask(_serviceProvider));
        
        // Continue our loop until we are asked to stop the tasks
        while (!_endTask)
        {
            await _mutex.WaitAsync();
            try
            {
                foreach (ITimedTask task in _timedTasks)
                {
                    if (task.CanExecuteTask().Result)
                    {
                        task.ExecuteTask();
                        task.InitializeTask(_serviceProvider);
                    }
                }
            }
            finally
            {
                _mutex.Release();
            }
        }
    }

    public void Stop()
    {
        _endTask = true;
    }

    public void AddTask(ITimedTask newTask)
    {
        _mutex.Wait();
        try
        {
            _timedTasks.Add(newTask);
            newTask.InitializeTask(_serviceProvider);
        }
        finally
        {
            _mutex.Release();
        }
    }

    public bool FindTask(Func<ITimedTask, bool> func, out ITimedTask? foundTask)
    {
        _mutex.Wait();
        try
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
        finally
        {
            _mutex.Release();
        }

        return false;
    }
    
    public void RemoveTask(ITimedTask task)
    {
        _mutex.Wait();
        try
        {
            _timedTasks.Remove(task);
        }
        finally
        {
            _mutex.Release();
        }
    }
}