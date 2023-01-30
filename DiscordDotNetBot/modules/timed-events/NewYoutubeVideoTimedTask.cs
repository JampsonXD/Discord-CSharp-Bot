using System.Diagnostics;
using Discord;
using Discord_CSharp_Bot.modules.discord_embeds;
using Discord_CSharp_Bot.modules.timed_events.interfaces;
using Discord.WebSocket;
using YoutubeClient.youtube_api.models.youtube_data_models;
using YoutubeClient.youtube_api.request.youtube_requests;
using YoutubeClient.youtube_api.services;

namespace Discord_CSharp_Bot.modules.timed_events;

public class NewYoutubeVideoTimedTask: ITimedTask
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly TimeSpan _interval;
    private readonly YoutubePlaylistItemListServiceRequest _request;
    private readonly ulong _discordUserId;
    private DateTime _canExecuteTime;

    public ulong DiscordUserId => _discordUserId;
    public string YoutubeChannelPlaylistId
    {
        get
        {
            Debug.Assert(_request.PlaylistId != null, "_request.PlaylistId != null");
            return _request.PlaylistId;
        }
    }

    /* An offset to add to our time when checking execution to see if the video is new
        A video is considered to be new if its publishing date is newer than the current time - (our interval amount + the time offset)
     */
    private TimeSpan _offset = TimeSpan.FromMinutes(5);

    public NewYoutubeVideoTimedTask(TimeSpan interval, string youtubeChannelPlaylistId, ulong discordUserId, YoutubeClientService clientService, DiscordSocketClient socketClient)
    {
        _interval = interval;
        _canExecuteTime = DateTime.UtcNow;

        _request = clientService.PlaylistItems.List();
        _request.Parts.Add("snippet");
        _request.PlaylistId = youtubeChannelPlaylistId;

        _discordSocketClient = socketClient;
        _discordUserId = discordUserId;
    }

    public async Task<bool> CanExecuteTask()
    {
        return DateTime.UtcNow > _canExecuteTime;
    }

    public async Task ExecuteTask()
    {
        var response = await _request.ExecuteRequestAsync();
        var playlistItems = response.Items;
        if (playlistItems.Count > 0)
        {
            var item = playlistItems.First();
            
            // If the newest item in the playlist is published within the interval + our offset time, send a message to the user
            if(IsWithinPublishingWindow(item))
            {
                var user = await _discordSocketClient.GetUserAsync(_discordUserId);
                await user.SendMessageAsync($"New video has been uploaded by {item.Snippet?.VideoOwnerChannelTitle}!",embed: YoutubeVideoEmbed.CreateNewYoutubeVideoEmbed(item));
            }
        }
    }

    private bool IsWithinPublishingWindow(YoutubePlaylistItem playlistItem)
    {
        return playlistItem.Snippet != null && playlistItem.Snippet.PublishedAt.ToUniversalTime() >
            DateTime.UtcNow.Subtract(_interval + _offset);
    }

    public async Task InitializeTask(IServiceProvider serviceProvider)
    {
        _canExecuteTime = DateTime.UtcNow + _interval;
    }
}