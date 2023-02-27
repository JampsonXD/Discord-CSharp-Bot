namespace DiscordBot.Modules.TimedEvents.Interfaces;

public interface ITimedTaskRetriever
{
    IEnumerable<ITimedTask> GetTimedTasks();
}