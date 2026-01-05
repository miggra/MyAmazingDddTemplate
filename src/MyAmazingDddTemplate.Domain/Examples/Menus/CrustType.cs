namespace MyAmazingDddTemplate.Domain.Examples.Menus;

public sealed record CrustType
{
    public string Value { get; }

     private CrustType(string value)
    {
        Value = value;
    }

    // Предопределенные значения
    public static readonly CrustType Thin = new("Тонкое");
    public static readonly CrustType Thick = new("Обычное");
    public static readonly CrustType GlutenFree = new("Безглютеновое");

    public static IReadOnlyList<CrustType> All => new[]
    {
        Thin, Thick, GlutenFree
    };
    public static CrustType FromString(string value)
    {
        var crustType = All.FirstOrDefault(c => c.Value.Equals(value, StringComparison.OrdinalIgnoreCase));
        if (crustType is null)
        {
            throw new ArgumentException($"Invalid crust type: {value}. Valid types: {string.Join(", ", All.Select(c => c.Value))}");
        }
        return crustType;
    }

    public override string ToString() => Value;
}
