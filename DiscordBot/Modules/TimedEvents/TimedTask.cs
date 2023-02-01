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
    
    public async Task<bool> CanExecuteTask()
    {
        return DateTime.UtcNow > _taskFinishTime;
    }

    public async Task ExecuteTask()
    {
        Action?.Invoke();
    }

    public async Task InitializeTask(IServiceProvider serviceProvider)
    {
        _taskFinishTime = DateTime.UtcNow + _interval;
    }
}