using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Models;

public class YoutubeTimedTaskInformation
{
    [Required]
    public required ulong DiscordUserId { get; set; }
    [Required]
    public required string ChannelId { get; set; }
    [Required]
    public required TimeSpan Interval { get; set; }
}