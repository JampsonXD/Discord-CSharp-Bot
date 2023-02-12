using System.Diagnostics;
using System.Xml;
using YoutubeClient.Models;
using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace DiscordBot.Modules.YoutubeHelpers;
public static class YoutubeHelpers
{
    public static async Task<TimeSpan> FindPlaylistDurationTotal(string playlistId, YoutubeClientService youtubeClient)
    {
        var request = youtubeClient.PlaylistItemResource.List();
        request.Parts.Add("snippet");
        request.PlaylistId = playlistId;
        request.MaxResults = 50;

        var playlistItems = await GetAllPlaylistItemsInternalAsync(request);
        
        // Get each video id for each list of items in each of our wrapper response objects
        var videoIds = (from wrapper in playlistItems
            from playlistValues in wrapper.Items
            select playlistValues.Snippet?.ResourceId.VideoId).ToList();

        TimeSpan duration = await GetVideoListTotalDurationAsync(videoIds, youtubeClient);
        return duration;
    }
    
    public static async Task<TimeSpan> FindPlaylistDurationLeftFromVideoAsync(string playlistId, string videoId, YoutubeClientService youtubeClient)
    {
        List<string> videoIds = new List<string>();
        
        var request = youtubeClient.PlaylistItemResource.List();
        request.Parts.Add("snippet");
        request.PlaylistId = playlistId;
        request.MaxResults = 50;
        
        do
        {
            var playlistItems = await request.ExecuteRequestAsync();
            foreach (var item in playlistItems.Items)
            {
                Debug.Assert(item.Snippet != null && item.Snippet.ResourceId.VideoId != null, "item.Snippet != null && item.Snippet.ResourceId.VideoId != null");
                if (item.Snippet.ResourceId.VideoId == videoId)
                {
                    request.PageToken = null;
                    break;
                }

                videoIds.Add(item.Snippet.ResourceId.VideoId);
                request.PageToken = playlistItems.NextPageToken;
            }
        } while (request.PageToken != null);

        return await GetVideoListTotalDurationAsync(videoIds, youtubeClient);
    }

    private static async Task<TimeSpan> GetVideoListTotalDurationAsync(List<string> videoIds, YoutubeClientService youtubeClient)
    {
        var request = youtubeClient.VideoResource.List();
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
            duration = videoResponse.Items.Select(item => XmlConvert.ToTimeSpan(item.ContentDetails.Duration))
                .Aggregate(duration, (total, next) => total.Add(next));
        }

        return duration;
    }

    private static async Task<List<YoutubeResponseDataWrapper<YoutubePlaylistItem>>> GetAllPlaylistItemsInternalAsync(YoutubePlaylistItemListServiceRequest request)
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