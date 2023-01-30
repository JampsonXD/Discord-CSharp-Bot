using System.Diagnostics;
using System.Xml;
using Discord_CSharp_Bot.modules.discord_embeds;
using Discord_CSharp_Bot.modules.timed_events;
using Discord_CSharp_Bot.modules.timed_events.interfaces;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using YoutubeClient.youtube_api.exceptions;
using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.models.youtube_data_models;
using YoutubeClient.youtube_api.request.youtube_requests;
using YoutubeClient.youtube_api.services;

namespace Discord_CSharp_Bot.modules.command_modules;

public class YoutubeModule : ModuleBase<SocketCommandContext>
{
    private readonly IServiceProvider _services;
    private readonly YoutubeClientService _youtubeClientService;
    private readonly YoutubeChannelListServiceRequest _ludwigServiceRequest;
    
    public YoutubeModule(IServiceProvider serviceProvider)
    {
        _services = serviceProvider;
        _youtubeClientService = _services.GetRequiredService<YoutubeClientService>();
        
        _ludwigServiceRequest = _youtubeClientService.Channels.List();
        _ludwigServiceRequest.Id = "UCjK0F1DopxQ5U0sCwOlXwOg";
        _ludwigServiceRequest.Parts.Add("contentDetails");
    }
    
    [Command("FindUploadPlaylistName")]
    [RequireOwner]
    [Summary("Returns the upload playlist's id for the passed in channel id")]
    public async Task FindUploadPlaylistNameAsync(string channelId)
    {
        try
        {
            var request = _youtubeClientService.Channels.List();
            request.Id = channelId;
            request.Parts.Add("contentDetails");

            var response = await request.ExecuteRequestAsync();
            await Context.Channel.SendMessageAsync(
                $"Upload playlist ID: {response.Items.First().ContentDetails?.RelatedPlaylists.Uploads}");
        }
        catch (YoutubeInvalidRequestException e)
        {
            await Context.Channel.SendMessageAsync($"Request failed! Error Message: {e.Message}");
        }
    }

    [Command("Ludwig")]
    [Summary("Sends channel information about Mogul Mail")]
    public async Task LudwigAsync()
    {
        // Request Mogul Mail's channel content details with its channel id
        var response = await _ludwigServiceRequest.ExecuteRequestAsync();

        // Print out the channels uploads playlist id if its content details is not null
        await Context.Channel.SendMessageAsync(response.Items.First().ContentDetails?.RelatedPlaylists.Uploads);
    }
    
    [Command("Mogul")]
    [Summary("Gets the latest mogul mail video")]
    public async Task MogulMailAsync()
    {
        await GetNewestUploadAsync("UUjK0F1DopxQ5U0sCwOlXwOg");
    }

    [Command("NotifyMe")]
    [RequireOwner]
    [Summary("Creates a new task to notify a user for youtube video uploads")]
    public async Task NotifyNewYoutubeVideoAsync(int interval, string channelPlaylistId)
    {
        var timedTaskHandler = _services.GetService<TimedTaskHandler>();
        if (timedTaskHandler != null)
        {
            timedTaskHandler.AddTask(new NewYoutubeVideoTimedTask(TimeSpan.FromMinutes(interval), channelPlaylistId, Context.User.Id, _youtubeClientService, _services.GetRequiredService<DiscordSocketClient>()));
            await Context.Channel.SendMessageAsync("Successfully added new notification alert!");
        }
    }

    [Command("UnNotifyMe")]
    [RequireOwner]
    [Summary("Removes a youtube task from the tasks being executed, if found")]
    public async Task UnNotifyYoutubeVideoAsync(string channelPlaylistId)
    {
        var timedTaskHandler = _services.GetService<TimedTaskHandler>();
        if (timedTaskHandler == null)
        {
            return;
        }

        // Find a task that is a YoutubeVideoTimedTask and has the same discord user id and playlist id
        if (timedTaskHandler.FindTask(currentTask =>
            {
                var youtubeTask = currentTask as NewYoutubeVideoTimedTask;
                if (youtubeTask == null) return false;

                return youtubeTask.DiscordUserId == Context.User.Id &&
                       youtubeTask.YoutubeChannelPlaylistId == channelPlaylistId;

            }, out ITimedTask? task) && task != null)
        {
            timedTaskHandler.RemoveTask(task);
            await Context.Message.Channel.SendMessageAsync("Successfully removed notification!");
        }
    }

    [Command("GetNewestUpload")]
    [RequireOwner]
    [Summary("Gets the newest upload in a youtube channel playlist")]
    public async Task GetNewestUploadAsync(string channelPlaylistId)
    {
        var request = _youtubeClientService.PlaylistItems.List();
        request.PlaylistId = channelPlaylistId;
        request.MaxResults = 1;
        request.Parts.Add("snippet");
        
        var list = (await request.ExecuteRequestAsync()).Items;
        if (list.Count > 0)
        {
            var item = list.First();
            var snippet = item.Snippet;
            if (snippet == null)
            {
                Console.WriteLine("Could not perform GetNewestUploadCommand. Playlist Item Snippet was not requested or available");
                return;
            }

            await Context.Channel.SendMessageAsync($"Here is the newest video for {snippet.VideoOwnerChannelTitle}",
                embed: YoutubeVideoEmbed.CreateNewYoutubeVideoEmbed(item));
        }
    }

    [Command("FindPlaylistDurationTotal")]
    [RequireOwner]
    [Summary("Find the total duration of all videos added up for a youtube playlist")]
    public async Task FindPlaylistDurationTotal(string playlistId)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = _youtubeClientService.PlaylistItems.List();
        request.Parts.Add("snippet");
        request.PlaylistId = playlistId;

        List<string> videoIds = new List<string>();

        var playlistItems = await GetAllPlaylistItemsInternalAsync(request);
        playlistItems.ForEach(item => 
            item.Items.ForEach(playlistItem =>
            {
                Debug.Assert(playlistItem.Snippet != null, "playlistItem.Snippet != null");
                videoIds.Add(playlistItem.Snippet.ResourceId.VideoId);
            }));

        TimeSpan duration = await GetVideoListTotalDurationAsync(videoIds);

        Console.WriteLine($"Executed request for the requested data in {stopwatch.Elapsed.ToString()} time.");
        await Context.Channel.SendMessageAsync($"Total time length is {duration.Days} days, {duration.Hours} hours," +
                                               $" {duration.Minutes} minutes, and {duration.Seconds} seconds.");
    }
    
    [Command("FindPlaylistDurationLeftFromVideo")]
    [RequireOwner]
    [Summary("Find the duration leftover if watching a playlist in order from a specific video")]
    public async Task FindPlaylistDurationLeftFromVideoAsync(string playlistId, string videoId)
    {
        var stopwatch = Stopwatch.StartNew();
        List<string> videoIds = new List<string>();
        
        var request = _youtubeClientService.PlaylistItems.List();
        request.Parts.Add("snippet");
        request.PlaylistId = playlistId;
        request.MaxResults = 50;
        
        do
        {
            var playlistItems = await request.ExecuteRequestAsync();
            foreach (var item in playlistItems.Items)
            {
                if (item.Snippet.ResourceId.VideoId == videoId)
                {
                    request.PageToken = null;
                    break;
                }

                videoIds.Add(item.Snippet.ResourceId.VideoId);
                request.PageToken = playlistItems.NextPageToken;
            }
        } while (request.PageToken != null);

        var duration = await GetVideoListTotalDurationAsync(videoIds);
        
        Console.WriteLine($"Executed request for the requested data in {stopwatch.Elapsed.ToString()} time.");
        await Context.Channel.SendMessageAsync($"Total time length is {duration.Days} days, {duration.Hours} hours," +
                                               $" {duration.Minutes} minutes, and {duration.Seconds} seconds.");
    }

    private async Task<TimeSpan> GetVideoListTotalDurationAsync(List<string> videoIds)
    {
        var request = _youtubeClientService.Videos.List();
        request.Parts.Add("contentDetails");
        request.MaxResults = 50;

        TimeSpan duration = TimeSpan.Zero;
        while (videoIds.Count > 0)
        {
            // Find the amount of total videos we are going to request (up to 50), retrieve them from our list starting from the first index, remove the items from the list
            int removeAmount = Math.Min(50, videoIds.Count);
            var ids = videoIds.GetRange(0, removeAmount);
            videoIds.RemoveRange(0, removeAmount);
            request.Ids = ids;
            var videoResponse = await request.ExecuteRequestAsync();

            // Convert Youtube's ISO 8601 time standard into a TimeSpan and add that to our total duration
            videoResponse.Items.ForEach(
                item => duration = duration.Add(XmlConvert.ToTimeSpan(item.ContentDetails.Duration)));
        }

        return duration;
    }

    private async Task<List<YoutubeResponseDataWrapper<YoutubePlaylistItem>>> GetAllPlaylistItemsInternalAsync(YoutubePlaylistItemListServiceRequest request)
    {
        List<YoutubeResponseDataWrapper<YoutubePlaylistItem>> items =
            new List<YoutubeResponseDataWrapper<YoutubePlaylistItem>>();
        
        do
        {
            var response = await request.ExecuteRequestAsync();
            items.Add(response);
            request.PageToken = response.NextPageToken;

        } while (request.PageToken != null);

        return items;
    }
}