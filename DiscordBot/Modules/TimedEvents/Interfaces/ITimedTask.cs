namespace DiscordBot.Modules.TimedEvents.Interfaces;

public interface ITimedTask
{
    public Task<bool> CanExecuteTask();

    public Task ExecuteTask();

    public Task InitializeTask(IServiceProvider serviceProvider);
    
}