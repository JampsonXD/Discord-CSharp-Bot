using Newtonsoft.Json;

namespace YoutubeClient.Models;

public class YoutubeVideo
{
    internal YoutubeVideo()
    {
        
    }
    
    // Type of content/response we contain
    [JsonProperty("kind")] 
    public string Kind { get; set; }
    
    // The e tag of this resource
    [JsonProperty("etag")]
    public string ETag { get; set; }
    
    // The id of the channel resource
    [JsonProperty("id")] 
    public string Id { get; set; }

    [JsonProperty("snippet")] 
    public YoutubeVideoSnippet? Snippet { get; set; }
    
    [JsonProperty("contentDetails")]
    public YoutubeVideoContentDetails ContentDetails { get; set; }
    
}

public class YoutubeVideoSnippet
{
    internal YoutubeVideoSnippet()
    {
        
    }

    [JsonProperty("publishedAt")]
    public DateTime PublishedAt { get; set; }
    
    [JsonProperty("channelId")]
    public string ChannelId { get; set; }
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
    [JsonProperty("thumbnails")]
    public Dictionary<string, YoutubeThumbnailData> Thumbnails { get; set; }
    
    [JsonProperty("channelTitle")]
    public string ChannelTitle { get; set; }
    
    [JsonProperty("tags")]
    public List<string> Tags { get; set; }
    
    [JsonProperty("categoryId")]
    public string CategoryId { get; set; }
    
    [JsonProperty("liveBroadcastContent")]
    public string LiveBroadcastContent { get; set; }
    
    [JsonProperty("defaultLanguage")]
    public string DefaultLanguage { get; set; }
    
    [JsonProperty("localized")]
    public YoutubeLocalizedData? Localized { get; set; }
    
    [JsonProperty("defaultAudioLanguage")]
    public string DefaultAudioLanguage { get; set; }
}

public class YoutubeVideoContentDetails
{
    internal YoutubeVideoContentDetails()
    {
        
    }
    
    /* The length of the video in ISO 8601 formatting */
    [JsonProperty("duration")]
    public string Duration { get; set; }
    
    /* Whether the video is in 2D or 3D */
    [JsonProperty("dimension")]
    public string Dimension { get; set; }
    
    /* Whether the video is available in high or standard definition */
    [JsonProperty("definition")]
    public string Definition { get; set; }
    
    /* Indicates whether captions are available for the video */
    [JsonProperty("caption")]
    public string Caption { get; set; }
    
    /* Indicates whether the video represents licensed content,
     uploaded to a channel linked to a content partner and claimed by that partner */
    [JsonProperty("licensedContent")]
    public bool LicensedContent { get; set; }
    
    /* Contains information about which countries block/allow a video from being viewed */
    [JsonProperty("regionRestriction")]
    public YoutubeRegionRestrictionData RegionRestriction { get; set; }
    
    /* Contains information regarding the videos rating under different rating schemes */
    [JsonProperty("contentRating")]
    public YoutubeVideoContentRating ContentRating { get; set; }
    
    /* Specifies the projection format of the video (360 or rectangular) */
    [JsonProperty("projection")]
    public string Projection { get; set; }
    
    /* Whether the video uploader provided a custom thumbnail for the video (Only visible to video uploader) */
    [JsonProperty("hasCustomThumbnail")]
    public bool? HasCustomThumbnail { get; set; }
}

public class YoutubeVideoContentRating
{
    internal YoutubeVideoContentRating()
    {
        
    }
    
    [JsonProperty("acbRating")]
    public string AcbRating;

    [JsonProperty("agcomRating")]
    public string AgcomRating;

    [JsonProperty("anatelRating")]
    public string AnatelRating;

    [JsonProperty("bbfcRating")]
    public string BbfcRating;

    [JsonProperty("bfvcRating")]
    public string BfvcRating;

    [JsonProperty("bmukkRating")]
    public string BmukkRating;

    [JsonProperty("catvRating")]
    public string CatvRating;

    [JsonProperty("catvfrRating")]
    public string CatvfrRating;

    [JsonProperty("cbfcRating")]
    public string CbfcRating;

    [JsonProperty("cccRating")]
    public string CccRating;

    [JsonProperty("cceRating")]
    public string CceRating;

    [JsonProperty("chfilmRating")]
    public string ChfilmRating;

    [JsonProperty("chvrsRating")]
    public string ChvrsRating;

    [JsonProperty("cicfRating")]
    public string CicfRating;

    [JsonProperty("cnaRating")]
    public string CnaRating;

    [JsonProperty("cncRating")]
    public string CncRating;

    [JsonProperty("csaRating")]
    public string CsaRating;

    [JsonProperty("cscfRating")]
    public string CscfRating;

    [JsonProperty("czfilmRating")]
    public string CzfilmRating;

    [JsonProperty("djctqRating")]
    public string DjctqRating;

    [JsonProperty("djctqRatingReasons")]
    public List<string> DjctqRatingReasons;

    [JsonProperty("ecbmctRating")]
    public string EcbmctRating;

    [JsonProperty("eefilmRating")]
    public string EefilmRating;

    [JsonProperty("egfilmRating")]
    public string EgfilmRating;

    [JsonProperty("eirinRating")]
    public string EirinRating;

    [JsonProperty("fcbmRating")]
    public string FcbmRating;

    [JsonProperty("fcoRating")]
    public string FcoRating;

    [JsonProperty("fmocRating")]
    public string FmocRating;

    [JsonProperty("fpbRating")]
    public string FpbRating;

    [JsonProperty("fpbRatingReasons")]
    public List<string> FpbRatingReasons;

    [JsonProperty("fskRating")]
    public string FskRating;

    [JsonProperty("grfilmRating")]
    public string GrfilmRating;

    [JsonProperty("icaaRating")]
    public string IcaaRating;

    [JsonProperty("ifcoRating")]
    public string IfcoRating;

    [JsonProperty("ilfilmRating")]
    public string IlfilmRating;

    [JsonProperty("incaaRating")]
    public string IncaaRating;

    [JsonProperty("kfcbRating")]
    public string KfcbRating;

    [JsonProperty("kijkwijzerRating")]
    public string KijkwijzerRating;

    [JsonProperty("kmrbRating")]
    public string KmrbRating;

    [JsonProperty("lsfRating")]
    public string LsfRating;

    [JsonProperty("mccaaRating")]
    public string MccaaRating;

    [JsonProperty("mccypRating")]
    public string MccypRating;

    [JsonProperty("mcstRating")]
    public string McstRating;

    [JsonProperty("mdaRating")]
    public string MdaRating;

    [JsonProperty("medietilsynetRating")]
    public string MedietilsynetRating;

    [JsonProperty("mekuRating")]
    public string MekuRating;

    [JsonProperty("mibacRating")]
    public string MibacRating;

    [JsonProperty("mocRating")]
    public string MocRating;

    [JsonProperty("moctwRating")]
    public string MoctwRating;

    [JsonProperty("mpaaRating")]
    public string MpaaRating;

    [JsonProperty("mpaatRating")]
    public string MpaatRating;

    [JsonProperty("mtrcbRating")]
    public string MtrcbRating;

    [JsonProperty("nbcRating")]
    public string NbcRating;

    [JsonProperty("nbcplRating")]
    public string NbcplRating;

    [JsonProperty("nfrcRating")]
    public string NfrcRating;

    [JsonProperty("nfvcbRating")]
    public string NfvcbRating;

    [JsonProperty("nkclvRating")]
    public string NkclvRating;

    [JsonProperty("oflcRating")]
    public string OflcRating;

    [JsonProperty("pefilmRating")]
    public string PefilmRating;

    [JsonProperty("rcnofRating")]
    public string RcnofRating;

    [JsonProperty("resorteviolenciaRating")]
    public string ResorteviolenciaRating;

    [JsonProperty("rtcRating")]
    public string RtcRating;

    [JsonProperty("rteRating")]
    public string RteRating;

    [JsonProperty("russiaRating")]
    public string RussiaRating;

    [JsonProperty("skfilmRating")]
    public string SkfilmRating;

    [JsonProperty("smaisRating")]
    public string SmaisRating;

    [JsonProperty("smsaRating")]
    public string SmsaRating;

    [JsonProperty("tvpgRating")]
    public string TvpgRating;

    [JsonProperty("ytRating")]
    public string YtRating;
}