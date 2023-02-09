using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using DiscordBot.Modules.DiscordEmbeds;
using DiscordBot.Modules.TimedEvents.Interfaces;
using YoutubeClient.Models;
using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace DiscordBot.Modules.TimedEvents;

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

        _request = clientService.PlaylistItemResource.List();
        _request.Parts.Add("snippet");
        _request.PlaylistId = youtubeChannelPlaylistId;

        _discordSocketClient = socketClient;
        _discordUserId = discordUserId;
    }

    public Task<bool> CanExecuteTask()
    {
        return Task.FromResult(DateTime.UtcNow > _canExecuteTime);
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

    public Task ResetTask()
    {
        _canExecuteTime = DateTime.UtcNow + _interval;
        return Task.CompletedTask;
    }

    private bool IsWithinPublishingWindow(YoutubePlaylistItem playlistItem)
    {
        return playlistItem.Snippet != null && playlistItem.Snippet.PublishedAt.ToUniversalTime() >
            DateTime.UtcNow.Subtract(_interval + _offset);
    }

    public Task InitializeTask(IServiceProvider serviceProvider)
    {
        _canExecuteTime = DateTime.UtcNow + _interval;
        return Task.CompletedTask;
    }
}