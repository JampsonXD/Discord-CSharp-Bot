using FluentValidation;

namespace DiscordBot.Configuration.Validation;

public class DiscordOptionsValidator: AbstractValidator<DiscordBotOptions>
{
    public DiscordOptionsValidator()
    {
        RuleFor(x => x.Prefix).NotEmpty().WithMessage("Discord Bot must contain a valid prefix!");
        RuleFor(x => x.Token).NotEmpty().WithMessage("Discord Bot must contain a valid token!");
        RuleFor(x => x.GameStatus).NotEmpty().WithMessage("Discord Bot must contain a valid game status!");
        RuleFor(x => x.GrammarBotSettings.DefaultGrammarResponse).NotEmpty()
            .WithMessage("Discord Bot must contain a valid default grammar response!");
    }
}