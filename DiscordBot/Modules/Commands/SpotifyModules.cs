using Discord.Commands;
using DiscordBot.Modules.DiscordEmbeds;
using Microsoft.Extensions.DependencyInjection;
using SpotifyClient.Services;

namespace DiscordBot.Modules.Commands;

public class SpotifyModules: ModuleBase<SocketCommandContext>
{
    private readonly IServiceProvider _services;
    private readonly SpotifyClientService _spotifyClientService;

    public SpotifyModules(IServiceProvider serviceProvider)
    {
        _services = serviceProvider;
        _spotifyClientService = _services.GetRequiredService<SpotifyClientService>();
    }
    
    [Command("FindSpotifyAlbum")]
    [RequireOwner]
    [Summary("Returns album information for the passed in album id")]
    public async Task FindSpotifyAlbumAsync(string albumId)
    {
        var request = _spotifyClientService.Album.AlbumRequest();
        request.AlbumId = albumId;

        var response = await request.ExecuteRequestAsync();
        await Context.Channel.SendMessageAsync($"Here is information on {response.Name}", embed: SpotifyEmbeds.CreateNewSpotifyAlbumEmbed(response));
}
}