using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules.TimedEvents.Interfaces;
using YoutubeClient.Services;

namespace DiscordBot.Modules.TimedEvents;

public class YoutubeTimedTaskDatabaseRetriever: ITimedTaskRetriever
{
    private readonly DiscordBotDatabase _database;
    private readonly YoutubeClientService _youtubeClient;
    private readonly DiscordSocketClient _discordClient;

    public YoutubeTimedTaskDatabaseRetriever(DiscordBotDatabase database, YoutubeClientService youtubeClient, DiscordSocketClient discordClient)
    {
        _database = database;
        _youtubeClient = youtubeClient;
        _discordClient = discordClient;
    }


    public IEnumerable<ITimedTask> GetTimedTasks()
    {
        return _database.YoutubeTimedTaskInformation.Select(item =>
            new NewYoutubeVideoTimedTask(item.Interval, item.ChannelId, item.DiscordUserId, _youtubeClient,
                _discordClient));
    }
}