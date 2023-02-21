using DiscordBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Core;

public class DiscordBotDatabase: DbContext
{
    public required DbSet<YoutubeTimedTaskInformation> YoutubeTimedTaskInformation { get; set; }

    public DiscordBotDatabase(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<YoutubeTimedTaskInformation>().HasKey(e => new {e.DiscordUserId, e.ChannelId});
    }
}