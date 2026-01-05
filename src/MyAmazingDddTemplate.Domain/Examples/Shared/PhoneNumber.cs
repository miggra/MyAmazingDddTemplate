namespace MyAmazingDddTemplate.Domain.Examples.Customers;

public sealed record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Phone number cannot be empty", nameof(value));
        }

        // Remove common formatting characters
        var cleanedValue = new string(value.Where(c => char.IsDigit(c) || c == '+').ToArray());

        if (cleanedValue.Length < 11) // 89999999999
        {
            throw new ArgumentException("Phone number must contain at least 11 digits", nameof(value));
        }

        if (cleanedValue.Length > 12) // +79999999999 
        {
            throw new ArgumentException("Phone number cannot exceed 12 digits", nameof(value));
        }

        if (cleanedValue.StartsWith("+7"))
        {
            cleanedValue = cleanedValue.Replace("+7", "8");
        }

        return new PhoneNumber(cleanedValue);
    }
}
