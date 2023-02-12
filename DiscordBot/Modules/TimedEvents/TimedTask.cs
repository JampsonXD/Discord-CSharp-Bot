using DiscordBot.Modules.TimedEvents.Interfaces;

namespace DiscordBot.Modules.TimedEvents;

public class TimedTask : ITimedTask
{

    private readonly TimeSpan _interval;
    private DateTime _taskFinishTime;
    public Action? Action { get; set; }

    public TimedTask(TimeSpan interval, Action? action = null)
    {
        _interval = interval;
        Action = action;
    }
    
    public Task<bool> CanExecuteTask()
    {
        return Task.FromResult(DateTime.UtcNow > _taskFinishTime);
    }

    public Task ExecuteTask()
    {
        Action?.Invoke();
        return Task.CompletedTask;
    }

    public Task ResetTask()
    {
        _taskFinishTime = DateTime.UtcNow + _interval;
        return Task.CompletedTask;
    }

    public Task InitializeTask(IServiceProvider serviceProvider)
    {
        return Task.CompletedTask;
    }
}