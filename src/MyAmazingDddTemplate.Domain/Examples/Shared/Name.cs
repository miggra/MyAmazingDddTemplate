namespace MyAmazingDddTemplate.Domain.Examples.Customers;

public sealed record Name
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Name Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Name cannot be empty", nameof(value));
        }

        if (value.Length < 2)
        {
            throw new ArgumentException("Name must be at least 2 characters long", nameof(value));
        }

        if (value.Length > 100)
        {
            throw new ArgumentException("Name cannot exceed 100 characters", nameof(value));
        }

        return new Name(value.Trim());
    }
}
