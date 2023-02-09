namespace DiscordBot.Modules.TimedEvents.Interfaces;

public interface ITimedTask
{
    public Task<bool> CanExecuteTask();

    public Task ExecuteTask();

    public Task ResetTask();

    public Task InitializeTask(IServiceProvider serviceProvider);
    
}