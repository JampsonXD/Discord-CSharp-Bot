namespace ClientService.Core.Validation;

/// <summary>
/// Represents a reflection attribute validator for the property value belonging to this attribute.
/// </summary>

[AttributeUsage(AttributeTargets.Property)]
public abstract class PropertyValidatorAttribute: Attribute
{
    /// <summary>
    /// Describes whether the property value passed in meets the validation requirements.
    /// </summary>
    /// <param name="propertyValue">The tested properties value.</param>
    /// <returns>Whether validation was successful.</returns>
    public abstract bool IsValid(object? propertyValue);
}