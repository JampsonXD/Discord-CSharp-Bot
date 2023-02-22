using System.Diagnostics;
using Discord.Commands;
using DiscordBot.Core;
using DiscordBot.Models;
using DiscordBot.Modules.DiscordEmbeds;
using DiscordBot.Modules.TimedEvents;
using Microsoft.EntityFrameworkCore;
using YoutubeClient.Exceptions;
using YoutubeClient.Models;
using YoutubeClient.Services;

namespace DiscordBot.Modules.Commands;

public class YoutubeModule : ModuleBase<SocketCommandContext>
{
    private readonly YoutubeClientService _youtubeClientService;
    private readonly DiscordBotDatabase _database;
    private readonly IMessageSink _messageSink;

    public YoutubeModule(YoutubeClientService youtubeClientService, DiscordBotDatabase database, IMessageSink messageSink)
    {
        _youtubeClientService = youtubeClientService;
        _database = database;
        _messageSink = messageSink;
    }

    [Command("SearchYoutubeChannel")]
    [RequireOwner]
    [Summary("Searches for youtube channels that match the inputted query")]
    public async Task SearchYoutubeChannelAsync(string query)
    {
        try
        {
            var request = _youtubeClientService.SearchResource.List();
            request.SetSearchResultOrder(YoutubeSearchResultsOrder.Relevance);
            request.MaxResults = 10;
            request.Query = query;
            request.SetSearchType(YoutubeSearchType.Channel);

            var response = await request.ExecuteRequestAsync();
            var foundChannels = response.Items.Select(item => item.Snippet?.ChannelTitle)
                .Aggregate(string.Empty, (total, next) => total + "," + next);
            foundChannels = foundChannels.Remove(0, 1);
            await Context.Channel.SendMessageAsync(foundChannels);
        }
        catch (YoutubeInvalidRequestException e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    [Command("FindUploadPlaylistName")]
    [RequireOwner]
    [Summary("Returns the upload playlist's id for the passed in channel id")]
    public async Task FindUploadPlaylistNameAsync(string channelId)
    {
        try
        {
            var request = _youtubeClientService.ChannelResource.List();
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

    [Command("NotifyMe")]
    [RequireOwner]
    [Summary("Creates a new task to notify a user for youtube video uploads")]
    public async Task NotifyNewYoutubeVideoAsync(int interval, string channelPlaylistId)
    {
        if (_database.YoutubeTimedTaskInformation.Any(item =>
                item.DiscordUserId == Context.User.Id && item.ChannelId == channelPlaylistId))
        {
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} Already subscribed for notifications to this playlist!");
            return;
        }
        
        var info = new YoutubeTimedTaskInformation()
        {
            ChannelId = channelPlaylistId,
            DiscordUserId = Context.User.Id,
            Interval = TimeSpan.FromMinutes(Math.Clamp(5, interval, 1440))
        };

        _database.YoutubeTimedTaskInformation.Add(info);
        await _database.SaveChangesAsync();

        await _messageSink.Process(new TimedTaskMessage()
        {
            TimedTaskInformation = info,
            Added = true
        });

        await Context.Channel.SendMessageAsync($"{Context.User.Mention} Successfully subscribed to channel playlist!");
    }

    [Command("UnNotifyMe")]
    [RequireOwner]
    [Summary("Removes a youtube task from the tasks being executed, if found")]
    public async Task UnNotifyYoutubeVideoAsync(string channelPlaylistId)
    {
        var dbItem = await _database.YoutubeTimedTaskInformation.FirstOrDefaultAsync(item =>
            item.DiscordUserId == Context.User.Id && item.ChannelId == channelPlaylistId);

        if (dbItem != null)
        {
            _database.YoutubeTimedTaskInformation.Remove(dbItem);
            await _database.SaveChangesAsync();
            
            await _messageSink.Process(new TimedTaskMessage()
            {
                Added = false,
                TimedTaskInformation = dbItem
            });
            await Context.Message.Channel.SendMessageAsync($"{Context.User.Mention} Successfully removed notification for this playlist!");
        }
    }

    [Command("GetNewestUpload")]
    [RequireOwner]
    [Summary("Gets the newest upload in a youtube channel playlist")]
    public async Task GetNewestUploadAsync(string channelPlaylistId)
    {
        var request = _youtubeClientService.PlaylistItemResource.List();
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
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var duration = await YoutubeHelpers.YoutubeHelpers.FindPlaylistDurationTotal(playlistId, _youtubeClientService);
            Console.WriteLine($"Executed request for the requested data in {stopwatch.Elapsed.ToString()} time.");
            await Context.Channel.SendMessageAsync($"Total time length is {duration.Days} days, {duration.Hours} hours," +
                                                   $" {duration.Minutes} minutes, and {duration.Seconds} seconds.");
        }
        catch(YoutubeInvalidRequestException e)
        {
            Console.WriteLine($"Exception occured while trying to retrieve request data. \n {e.Message}");
            await Context.Channel.SendMessageAsync("Unable to handle request!");
        }
    }
    
    [Command("FindPlaylistDurationLeftFromVideo")]
    [RequireOwner]
    [Summary("Find the duration leftover if watching a playlist in order from a specific video")]
    public async Task FindPlaylistDurationLeftFromVideoAsync(string playlistId, string videoId)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var duration =
                await YoutubeHelpers.YoutubeHelpers.FindPlaylistDurationLeftFromVideoAsync(playlistId, videoId,
                    _youtubeClientService);
            Console.WriteLine($"Executed request for the requested data in {stopwatch.Elapsed.ToString()} time.");
            await Context.Channel.SendMessageAsync(
                $"Total time length is {duration.Days} days, {duration.Hours} hours," +
                $" {duration.Minutes} minutes, and {duration.Seconds} seconds.");
        }
        catch (YoutubeInvalidRequestException e)
        {
            Console.WriteLine($"Exception occured while trying to retrieve request data. \n {e.Message}");
            await Context.Channel.SendMessageAsync("Unable to handle request!");
        }
    }
}