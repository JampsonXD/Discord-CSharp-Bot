using System.Diagnostics;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules.DiscordEmbeds;
using DiscordBot.Modules.TimedEvents;
using DiscordBot.Modules.TimedEvents.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using YoutubeClient.Exceptions;
using YoutubeClient.Models;
using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace DiscordBot.Modules.Commands;

public class YoutubeModule : ModuleBase<SocketCommandContext>
{
    private readonly IServiceProvider _services;
    private readonly YoutubeClientService _youtubeClientService;
    private readonly YoutubeChannelListServiceRequest _ludwigServiceRequest;
    
    public YoutubeModule(IServiceProvider serviceProvider)
    {
        _services = serviceProvider;
        _youtubeClientService = _services.GetRequiredService<YoutubeClientService>();
        
        _ludwigServiceRequest = _youtubeClientService.ChannelResource.List();
        _ludwigServiceRequest.Id = "UCjK0F1DopxQ5U0sCwOlXwOg";
        _ludwigServiceRequest.Parts.Add("contentDetails");
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