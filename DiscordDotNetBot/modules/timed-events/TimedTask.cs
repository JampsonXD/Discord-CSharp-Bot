using Discord_CSharp_Bot.modules.timed_events.interfaces;

namespace Discord_CSharp_Bot.modules.timed_events;

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