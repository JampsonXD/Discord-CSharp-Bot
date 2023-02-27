namespace DiscordBot.Modules.TimedEvents.Interfaces;

/// <summary>
/// Represents a task that can be executed repeatedly over time.
/// </summary>

public interface ITimedTask
{
    /// <summary>
    /// Whether the task meets the criteria for execution.
    /// </summary>
    /// <returns>If the task can be executed.</returns>
    public Task<bool> CanExecuteTask();

    /// <summary>
    /// Executes the task.
    /// </summary>
    public Task ExecuteTask();

    /// <summary>
    /// Resets the task to a base state.
    /// </summary>
    public Task ResetTask();

    /// <summary>
    /// Initializes the task using the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public Task InitializeTask(IServiceProvider serviceProvider);
    
}