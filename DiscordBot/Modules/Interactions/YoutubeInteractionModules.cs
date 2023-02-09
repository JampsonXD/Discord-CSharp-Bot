using System.Diagnostics;
using Discord.Interactions;
using YoutubeClient.Services;

namespace DiscordBot.Modules.Interactions;

public class YoutubeInteractionModules: InteractionModuleBase<SocketInteractionContext>
{
    private readonly YoutubeClientService _youtubeClient;

    public YoutubeInteractionModules(YoutubeClientService youtubeClient)
    {
        _youtubeClient = youtubeClient;
    }
    
    [SlashCommand("find-playlist-duration-total", "Find the total amount of time it would take to watch all videos in a youtube playlist.")]
    [RequireOwner]
    public async Task FindPlaylistDurationTotal(string youtubePlaylistId)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // This command may take more than our timeout window so defer our response
        await DeferAsync();
        
        var duration = await YoutubeHelpers.YoutubeHelpers.FindPlaylistDurationTotal(youtubePlaylistId, _youtubeClient);
        Console.WriteLine($"Executed request for the requested data in {stopwatch.Elapsed.ToString()} time.");
        await FollowupAsync($"Total time length is {duration.Days} days, {duration.Hours} hours," +
                                               $" {duration.Minutes} minutes, and {duration.Seconds} seconds.");
    }
    
    [SlashCommand("find-playlist-leftover-duration", "Find the leftover amount of time it would take to finish a youtube playlist.")]
    [RequireOwner]
    public async Task FindPlaylistDurationLeftFromVideoAsync(string youtubePlaylistId, string youtubeVideoId)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // This command may take more than our timeout window so defer our response
        await DeferAsync();
        
        var duration =
            await YoutubeHelpers.YoutubeHelpers.FindPlaylistDurationLeftFromVideoAsync(youtubePlaylistId, youtubeVideoId,
                _youtubeClient);
        Console.WriteLine($"Executed request for the requested data in {stopwatch.Elapsed.ToString()} time.");
        await FollowupAsync($"Total time length is {duration.Days} days, {duration.Hours} hours," +
                                               $" {duration.Minutes} minutes, and {duration.Seconds} seconds.");
    }
}