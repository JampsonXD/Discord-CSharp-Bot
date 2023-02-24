using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using DiscordBot.Modules.DiscordEmbeds;
using DiscordBot.Modules.TimedEvents.Interfaces;
using YoutubeClient.Models;
using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace DiscordBot.Modules.TimedEvents;

/// <summary>
/// Represents a <see cref="ITimedTask"/> that notifies a discord user when a new video on a youtube playlist has been uploaded within a set timeframe.
/// </summary>

/// <seealso cref="ITimedTask"/>

public class NewYoutubeVideoTimedTask: ITimedTask
{
    /// <summary>
    /// The discord client used to send discord notifications.
    /// </summary>
    private readonly DiscordSocketClient _discordSocketClient;
    /// <summary>
    /// The interval at which we will execute the task and check if a new video has been uploaded between this time.
    /// </summary>
    private readonly TimeSpan _interval;
    /// <summary>
    /// The <see cref="YoutubePlaylistItemListServiceRequest"/> that is used to request playlist items from a playlist.
    /// </summary>
    private readonly YoutubePlaylistItemListServiceRequest _request;
    /// <summary>
    /// The discord ID to notify when a new video has been added to the playlist.
    /// </summary>
    private readonly ulong _discordUserId;
    /// <summary>
    /// The time we check against to see if we can execute the request.
    /// </summary>
    /// <remarks>
    /// This time is set as the current UTC time plus our interval. On <see cref="CanExecuteTask"/> this time is then checked to see if it less than the current UTC time to see if the task can be triggered.
    /// </remarks>
    private DateTime _canExecuteTime;

    /// <summary>
    /// The discord ID to notify when a new video has been added to the playlist.
    /// </summary>
    public ulong DiscordUserId => _discordUserId;
    /// <summary>
    /// Gets the ID of the youtube channel playlist being queried against.
    /// </summary>
    public string YoutubeChannelPlaylistId
    {
        get
        {
            Debug.Assert(_request.PlaylistId != null, "_request.PlaylistId != null");
            return _request.PlaylistId;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewYoutubeVideoTimedTask"/> class
    /// </summary>
    /// <param name="interval">The interval at which our task will be executed.</param>
    /// <param name="youtubeChannelPlaylistId">The youtube channel playlist id to be queried for new uploads.</param>
    /// <param name="discordUserId">The discord user id of the discord user to notify.</param>
    /// <param name="clientService">The Youtube Client Service used to make Youtube playlist item requests.</param>
    /// <param name="socketClient">The discord client used to notify our discord user.</param>
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

   /// <inheritdoc/>
    public Task<bool> CanExecuteTask()
    {
        return Task.FromResult(DateTime.UtcNow > _canExecuteTime);
    }

    /// <inheritdoc/>
    public async Task ExecuteTask()
    {
        var response = await _request.ExecuteRequestAsync();
        var playlistItems = response.Items;
        if (playlistItems.Count > 0)
        {
            var item = playlistItems.First();
            if(CanAlertUser(item))
            {
                var user = await _discordSocketClient.GetUserAsync(_discordUserId);
                await user.SendMessageAsync($"New video has been uploaded by {item.Snippet?.VideoOwnerChannelTitle}!",embed: YoutubeVideoEmbed.CreateNewYoutubeVideoEmbed(item));
            }
        }
    }

    /// <inheritdoc/>
    public Task ResetTask()
    {
        _canExecuteTime = DateTime.UtcNow + _interval;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Check to see if the correct conditions were met to alert the user that the <see cref="YoutubePlaylistItem"/> is a new upload.
    /// </summary>
    /// <param name="playlistItem"><see cref="YoutubePlaylistItem"/> to check against.</param>
    /// <returns>Whether the conditions have been met.</returns>
    private bool CanAlertUser(YoutubePlaylistItem playlistItem)
    {
        return playlistItem.Snippet != null && playlistItem.Snippet.PublishedAt.ToUniversalTime() >
            DateTime.UtcNow.Subtract(_interval);
    }

    /// <inheritdoc/>
    public Task InitializeTask(IServiceProvider serviceProvider)
    {
        _canExecuteTime = DateTime.UtcNow + _interval;
        return Task.CompletedTask;
    }
}