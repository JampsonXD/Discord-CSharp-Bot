namespace Discord_CSharp_Bot.modules.timed_events.interfaces;

public interface ITimedTask
{
    public Task<bool> CanExecuteTask();

    public Task ExecuteTask();

    public Task InitializeTask(IServiceProvider serviceProvider);
    
}