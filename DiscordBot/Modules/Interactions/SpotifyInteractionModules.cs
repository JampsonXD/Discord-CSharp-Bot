using Discord.Interactions;
using DiscordBot.Modules.DiscordEmbeds;
using SpotifyClient.Services;

namespace DiscordBot.Modules.Interactions;

public class SpotifyInteractionModules: InteractionModuleBase<SocketInteractionContext>
{
    private readonly SpotifyClientService _spotifyClient;


    public SpotifyInteractionModules(SpotifyClientService spotifyClient)
    {
        _spotifyClient = spotifyClient;
    }
    
    [SlashCommand("find-album", "Find a spotify album using the album id")]
    public async Task FindAlbumAsync(string id)
    {
        var request = _spotifyClient.Album.AlbumRequest();
        request.AlbumId = id;
        
        var response = await request.ExecuteRequestAsync();
        await RespondAsync(embed: SpotifyEmbeds.CreateNewSpotifyAlbumEmbed(response));
    }

    [SlashCommand("find-track", "Finds information on a specific spotify track using its track id")]
    public async Task FindTrackAsync(string id)
    {
        var request = _spotifyClient.Track.TrackRequest(id);

        var response = await request.ExecuteRequestAsync();
        await RespondAsync(embed: SpotifyEmbeds.CreateNewSpotifyTrackEmbed(response));
    }
}