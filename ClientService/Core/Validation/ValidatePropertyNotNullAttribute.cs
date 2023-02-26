namespace ClientService.Core.Validation;

/// <summary>
/// Represents a reflection attribute for validating if the property value is not null.
/// </summary>

/// <seealso cref="PropertyValidatorAttribute"/>

[AttributeUsage(AttributeTargets.Property)]
public class ValidatePropertyNotNullAttribute : PropertyValidatorAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatePropertyNotNullAttribute"/> class.
    /// </summary>
    public ValidatePropertyNotNullAttribute(){}
    
    /// <summary>
    /// Returns if the passed in property is not null.
    /// </summary>
    /// <param name="propertyValue">The property value.</param>
    public override bool IsValid(object? propertyValue)
    {
        return propertyValue != null;
    }
}