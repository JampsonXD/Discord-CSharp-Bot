namespace DiscordBot.Configuration;

public class DiscordBotOptions
{
    public const string Position = "Discord";
    
    public required string Prefix { get; set; }
    public required string Token { get; set; }
    public required string GameStatus { get; set; }
    public required GrammarBotOptions GrammarBotSettings { get; set; }
}

public class GrammarBotOptions
{
    public required string DefaultGrammarResponse { get; set; }
    public List<ulong>? GrammarCheckIds { get; set; } = null;
    public List<string>? GrammarCorrectionResponses { get; set; } = null;
}