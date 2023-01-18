using Discord_CSharp_Bot.modules.timed_events.interfaces;

namespace Discord_CSharp_Bot.modules.timed_events;

public class TimedTask : ITimedTask
{

    private readonly int _interval;
    private DateTime _taskFinishTime;
    public Action? Action { get; set; }

    public TimedTask(int interval)
    {
        _interval = interval;
        Action = null;
    }
    
    public async Task<bool> CanExecuteTask()
    {
        return DateTime.Now > _taskFinishTime;
    }

    public async Task ExecuteTask()
    {
        Action?.Invoke();
    }

    public async Task InitializeTask(IServiceProvider serviceProvider)
    {
        _taskFinishTime = DateTime.Now.AddSeconds(_interval);
    }
}