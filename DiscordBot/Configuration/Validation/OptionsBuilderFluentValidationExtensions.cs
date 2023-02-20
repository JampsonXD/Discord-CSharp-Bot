using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DiscordBot.Configuration.Validation;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(s => 
            new FluentValidateOptions<TOptions>(optionsBuilder.Name, s.GetRequiredService<IValidator<TOptions>>()));
        return optionsBuilder;
    }
}

 public class FluentValidateOptions<TOptions>
        : IValidateOptions<TOptions> where TOptions : class
 {
     private readonly IValidator<TOptions> _validator;
        public string? Name { get; }
        
        public FluentValidateOptions(string? name, IValidator<TOptions> validator)
        {
            Name = name;
            _validator = validator;
        }
        
        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            // Null name is used to configure all named options.
            if (Name != null && Name != name)
            {
                // Ignored if not validating this instance.
                return ValidateOptionsResult.Skip;
            }

            // Ensure options are provided to validate against
            ArgumentNullException.ThrowIfNull(options);

            var validationResults = _validator.Validate(options);
            if (validationResults.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            var errors = validationResults.Errors.Select(x =>
                $"Options validation failed for '{x.PropertyName}' with error: '{x.ErrorMessage}'");

            return ValidateOptionsResult.Fail(errors);
        }
    }