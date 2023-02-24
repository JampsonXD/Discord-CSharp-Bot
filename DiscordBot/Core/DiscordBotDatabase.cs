using DiscordBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Core;

/// <summary>
/// Represents a session with the database containing entities needed for a Discord Hosted Service.
/// </summary>

/// <seealso cref="DbContext"/>

public class DiscordBotDatabase: DbContext
{
    /// <summary>
    /// Represents a set of <see cref="YoutubeTimedTaskInformation"/> entities that are being stored in a database.
    /// </summary>
    public required DbSet<YoutubeTimedTaskInformation> YoutubeTimedTaskInformation { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordBotDatabase"/> class and ensures that our database for this context has been created.
    /// </summary>
    /// <inheritdoc/>
    public DiscordBotDatabase(DbContextOptions options) : base(options)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Database.EnsureCreated();
    }
    
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<YoutubeTimedTaskInformation>().HasKey(e => new {e.DiscordUserId, e.ChannelId});
    }
}