using Newtonsoft.Json;

namespace SpotifyClient.DataModels;

public partial class SpotifyPlaylists
        {
            [JsonProperty("href")]
            public Uri Href { get; set; }

            [JsonProperty("items")]
            public SpotifyPlaylist[] Items { get; set; }

            [JsonProperty("limit")]
            public long Limit { get; set; }

            [JsonProperty("next")]
            public object Next { get; set; }

            [JsonProperty("offset")]
            public long Offset { get; set; }

            [JsonProperty("previous")]
            public Uri Previous { get; set; }

            [JsonProperty("total")]
            public long Total { get; set; }
        }

        public partial class SpotifyPlaylist
        {
            [JsonProperty("collaborative")]
            public bool Collaborative { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("external_urls")]
            public SpotifyExternalUrls SpotifyExternalUrls { get; set; }

            [JsonProperty("href")]
            public Uri Href { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("images")]
            public SpotifyImage[] Images { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("owner")]
            public SpotifyPlaylistOwner SpotifyPlaylistOwner { get; set; }

            [JsonProperty("primary_color")]
            public object PrimaryColor { get; set; }

            [JsonProperty("public")]
            public bool Public { get; set; }

            [JsonProperty("snapshot_id")]
            public string SnapshotId { get; set; }

            [JsonProperty("tracks")]
            public SpotifyTracks SpotifyTracks { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("uri")]
            public string Uri { get; set; }
        }

        public partial class SpotifyExternalUrls
        {
            [JsonProperty("spotify")]
            public string? Spotify { get; set; }
        }

        public partial class SpotifyPlaylistOwner
        {
            [JsonProperty("display_name")]
            public string DisplayName { get; set; }

            [JsonProperty("external_urls")]
            public SpotifyExternalUrls SpotifyExternalUrls { get; set; }

            [JsonProperty("href")]
            public Uri Href { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("uri")]
            public string Uri { get; set; }
        }

        public partial class SpotifyTracks
        {
            [JsonProperty("href")]
            public Uri Href { get; set; }

            [JsonProperty("total")]
            public long Total { get; set; }
        }

        public class SpotifyPlaylistData
        {
            [JsonProperty("href")]
            public Uri Href { get; set; }

            [JsonProperty("items")]
            public SpotifyPlaylistTrackData[] Items { get; set; }

            [JsonProperty("limit")]
            public long Limit { get; set; }

            [JsonProperty("next")]
            public object Next { get; set; }

            [JsonProperty("offset")]
            public long Offset { get; set; }

            [JsonProperty("previous")]
            public object Previous { get; set; }

            [JsonProperty("total")]
            public long Total { get; set; }
        }

    public class SpotifyPlaylistTrackData
        {
            [JsonProperty("added_at")]
            public DateTimeOffset AddedAt { get; set; }

            [JsonProperty("added_by")]
            public AddedBy AddedBy { get; set; }

            [JsonProperty("is_local")]
            public bool IsLocal { get; set; }

            [JsonProperty("primary_color")]
            public object PrimaryColor { get; set; }

            [JsonProperty("track")]
            public SpotifyTrack SpotifyTrack { get; set; }

            [JsonProperty("video_thumbnail")]
            public VideoThumbnail VideoThumbnail { get; set; }
        }