using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Models;
using DiscordBot.Modules.TimedEvents.Interfaces;
using YoutubeClient.Services;

namespace DiscordBot.Modules.TimedEvents;

/// <summary>
/// Represents a message containing information for a <see cref="YoutubeTimedTaskInformation"/> entity being added or removed from the database.
/// </summary>

public record YoutubeTimedTaskDatabaseUpdateMessage : IMessage
{
    /// <summary>
    /// The database entity that was created for the timed task.
    /// </summary>
    public required YoutubeTimedTaskInformation TimedTaskInformation;
    
    /// <summary>
    /// Whether the <see cref="YoutubeTimedTaskInformation"/> was added or removed from the database.
    /// </summary>
    public required bool Added;
}

/// <summary>
/// Extension methods for <see cref="YoutubeTimedTaskDatabaseUpdateMessage"/>.
/// </summary>
public static class YoutubeTimedTaskDatabaseUpdateMessageExtensions
{
    /// <summary>
    /// Creates an <see cref="ITimedTask"/> instance using the messages information and the passed in Youtube and Discord clients.
    /// </summary>
    /// <param name="message">The <see cref="YoutubeTimedTaskDatabaseUpdateMessage"/> message.</param>
    /// <param name="youtubeClient">The Youtube client used in the created timed task.</param>
    /// <param name="discordClient">The Discord client used in the created timed task.</param>
    /// <returns>A new <see cref="ITimedTask"/> instance.</returns>
    public static ITimedTask ToTimedTask(this YoutubeTimedTaskDatabaseUpdateMessage message,
        YoutubeClientService youtubeClient, DiscordSocketClient discordClient)
    {
        var info = message.TimedTaskInformation;
        return new NewYoutubeVideoTimedTask(info.Interval, info.ChannelId, info.DiscordUserId, youtubeClient,
            discordClient);
    }

    /// <summary>
    /// Returns whether the message matches information with the passed in <see cref="ITimedTask"/>.
    /// </summary>
    /// <param name="message">The <see cref="YoutubeTimedTaskDatabaseUpdateMessage"/> message.</param>
    /// <param name="task">The task we are comparing against.</param>
    /// <returns>Whether the TimedTask matches information with the message.</returns>
    public static bool MatchesTimedTask(this YoutubeTimedTaskDatabaseUpdateMessage message,
        ITimedTask task)
    {
        if (task is not NewYoutubeVideoTimedTask youtubeTask)
        {
            return false;
        }

        return youtubeTask.DiscordUserId == message.TimedTaskInformation.DiscordUserId &&
               youtubeTask.YoutubeChannelPlaylistId == message.TimedTaskInformation.ChannelId;
    }
}