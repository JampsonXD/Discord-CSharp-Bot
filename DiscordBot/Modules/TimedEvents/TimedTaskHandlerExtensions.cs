using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Models;
using YoutubeClient.Services;

namespace DiscordBot.Modules.TimedEvents;

public class TimedTaskMessage : IMessage
{
    public required YoutubeTimedTaskInformation TimedTaskInformation { get; set; }
    public bool Added { get; set; }
}

public static class TimedTaskHandlerExtensions
{
    public static void NotifyTimedTaskHandler(this TimedTaskHandler handler, TimedTaskMessage message, YoutubeClientService youtubeClient, DiscordSocketClient discordClient)
    {
        var info = message.TimedTaskInformation;
        if (message.Added)
        {
            handler.AddTask(new NewYoutubeVideoTimedTask(info.Interval, info.ChannelId, info.DiscordUserId, youtubeClient, discordClient));
        }
        else
        {
            if (handler.FindTask(task => task is NewYoutubeVideoTimedTask youtubeTask &&
                                         youtubeTask.DiscordUserId == info.DiscordUserId &&
                                         youtubeTask.YoutubeChannelPlaylistId == info.ChannelId, out var foundTask) && foundTask != null)
            {
                handler.RemoveTask(foundTask);
            }
        }
    }
}