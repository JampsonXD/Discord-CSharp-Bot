using Discord_CSharp_Bot.modules.discord_embeds;
using Discord_CSharp_Bot.modules.timed_events;
using Discord_CSharp_Bot.modules.timed_events.interfaces;
using Discord_CSharp_Bot.modules.youtube_api;
using Discord_CSharp_Bot.modules.youtube_api.request.interfaces;
using Discord_CSharp_Bot.modules.youtube_api.request.request_parameters;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Discord_CSharp_Bot.modules.command_modules
{
    public class UtilityModule : ModuleBase<SocketCommandContext>
    {
        private readonly IServiceProvider _services;

        public UtilityModule(IServiceProvider serviceProvider)
        {
            _services = serviceProvider;
        }
        
        [Command("ping")]
        [Discord.Commands.Summary("Tells the user what ping they have to the current server")]
        public async Task PingAsync()
        {
            await Context.Channel.SendMessageAsync($"Ping is currently {Context.Client.Latency}.");
        }


        [Command("Ludwig")]
        [Discord.Commands.Summary("Sends channel information about Mogul Mail")]
        public async Task LudwigAsync()
        {
            var youtubeClient = _services.GetService<IYoutubeClient>();
            if(youtubeClient != null)
            {
                // Request Mogul Mail's channel content details with its channel id
                var response = await youtubeClient.YoutubeChannelEndpoint.GetYoutubeChannelWithParametersAsync(
                    new YoutubeChannelParameterBuilder().WithContentDetails().WithId("UCjK0F1DopxQ5U0sCwOlXwOg").Build());
                
                //var response = await youtubeClient.YoutubeChannelEndpoint.GetYoutubeChannelByIdAsync("UCjK0F1DopxQ5U0sCwOlXwOg");
                
                // Print out the channels uploads playlist id if its content details is not null
                await Context.Channel.SendMessageAsync(response.ContentDetails?.RelatedPlaylists.Uploads);
            }
        }
        
        [Command("Mogul")]
        [Discord.Commands.Summary("Gets the latest mogul mail video")]
        public async Task MogulMailAsync()
        {
            GetNewestUploadAsync("UUjK0F1DopxQ5U0sCwOlXwOg");
        }

        [Command("NotifyMe")]
        [RequireOwner]
        [Summary("Creates a new task to notify a user for youtube video uploads")]
        public async Task NotifyNewYoutubeVideoAsync(int interval, string channelPlaylistId)
        {
            var timedTaskHandler = _services.GetService<TimedTaskHandler>();
            if (timedTaskHandler != null)
            {
                timedTaskHandler.AddTask(new NewYoutubeVideoTimedTask(TimeSpan.FromMinutes(interval), channelPlaylistId, Context.User.Id));
                Context.Channel.SendMessageAsync("Successfully added new notification alert!");
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
            if (timedTaskHandler.FindTask(task =>
                {
                    var youtubeTask = task as NewYoutubeVideoTimedTask;
                    if (youtubeTask == null) return false;

                    return youtubeTask.DiscordUserId == Context.User.Id &&
                           youtubeTask.YoutubeChannelPlaylistId == channelPlaylistId;

                }, out ITimedTask? task) && task != null)
            {
                timedTaskHandler.RemoveTask(task);
                Context.Message.Channel.SendMessageAsync("Successfully removed notification!");
            }
        }

        [Command("GetNewestUpload")]
        [RequireOwner]
        [Summary("Gets the newest upload in a youtube channel playlist")]
        public async Task GetNewestUploadAsync(string channelPlaylistId)
        {
            var ytClient = _services.GetService<IYoutubeClient>();

            if (ytClient == null)
            {
                Console.WriteLine("Could not perform GetNewestUploadCommand. Youtube client does not exist");
                return;
            }

            var list = (await ytClient.YoutubePlaylistItemsEndpoint.GetYoutubePlaylistItemsByPlaylistIdAsync(channelPlaylistId, 1)).Items;
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
    }
}