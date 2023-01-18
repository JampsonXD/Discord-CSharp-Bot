using Discord;
using Discord_CSharp_Bot.modules.discord_embeds;
using Discord_CSharp_Bot.modules.timed_events.interfaces;
using Discord_CSharp_Bot.modules.youtube_api.models;
using Discord_CSharp_Bot.modules.youtube_api.request.interfaces;
using Discord_CSharp_Bot.modules.youtube_api.request.request_parameters;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Discord_CSharp_Bot.modules.timed_events;

public class NewYoutubeVideoTimedTask: ITimedTask
{
    private DiscordSocketClient? _discordSocketClient;
    private IYoutubeClient? _youtubeClient;
    private readonly TimeSpan _interval;
    private readonly string _youtubeChannelPlaylistId;
    private readonly ulong _discordUserId;
    private DateTime _canExecuteTime;
    private string? _eTag = null;
    
    public ulong DiscordUserId => _discordUserId;
    public string YoutubeChannelPlaylistId => _youtubeChannelPlaylistId;

    /* An offset to add to our time when checking execution to see if the video is new
        A video is considered to be new if its publishing date is newer than the current time - (our interval amount + the time offset)
     */
    private TimeSpan _offset = TimeSpan.FromMinutes(5);

    public NewYoutubeVideoTimedTask(TimeSpan interval, string youtubeChannelPlaylistId, ulong discordUserId)
    {
        _interval = interval;
        _canExecuteTime = DateTime.UtcNow;
        _youtubeChannelPlaylistId = youtubeChannelPlaylistId;
        _discordUserId = discordUserId;
    }

    public async Task<bool> CanExecuteTask()
    {
        return DateTime.UtcNow > _canExecuteTime;
    }

    public async Task ExecuteTask()
    {
        if (_youtubeClient == null || _discordSocketClient == null)
        {
            return;
        }

        var builder = new YoutubePlaylistItemParameterBuilder().WithId(_youtubeChannelPlaylistId).WithSnippet()
            .WithMaxResults(1);
        
        var response = await _youtubeClient.YoutubePlaylistItemsEndpoint.GetYoutubePlaylistItemsWithParametersAsync(
            builder.Build(), _eTag);

        var playlistItems = response.Items;
        if (playlistItems.Count > 0)
        {
            var item = playlistItems.First();
            
            // If we have a valid etag value and the etag value equals the items etag value, do not send a message, the message has already been sent
            // If the newest item in the playlist is published within the interval + our offset time, send a message to the user
            if(!AreETagsMatching(_eTag, response.ETag) && IsWithinPublishingWindow(item))
            {
                var user = await _discordSocketClient.GetUserAsync(_discordUserId);
                await user.SendMessageAsync($"New video has been uploaded by {item.Snippet?.VideoOwnerChannelTitle}!",embed: YoutubeVideoEmbed.CreateNewYoutubeVideoEmbed(item));
            }

            // Cache our ETag for follow up queries
            _eTag = response.ETag;
        }
    }

    private bool AreETagsMatching(string? eTag, string responseETag)
    {
        return eTag != null && eTag == responseETag;
    }

    private bool IsWithinPublishingWindow(YoutubePlaylistItem playlistItem)
    {
        return playlistItem.Snippet != null && playlistItem.Snippet.PublishedAt.ToUniversalTime() >
            DateTime.UtcNow.Subtract(_interval + _offset);
    }

    public async Task InitializeTask(IServiceProvider serviceProvider)
    {
        _youtubeClient = serviceProvider.GetService<IYoutubeClient>();
        _discordSocketClient = serviceProvider.GetService<DiscordSocketClient>();
        _canExecuteTime = DateTime.UtcNow + _interval;
    }
}