using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Menus;

public class PizzaSize : Entity<Guid>
{
    private PizzaSize(
        string name,
        int diameterInCm,
        decimal priceMultiplier) : base(Guid.NewGuid())
    {
        Name = name;
        DiameterInCm = diameterInCm;
        PriceMultiplier = priceMultiplier;
    }

    public string Name { get; private set; }
    public int DiameterInCm { get; private set; }
    public decimal PriceMultiplier { get; private set; }

    // Фабричный метод
    public static PizzaSize Create(string name, int diameterInCm, decimal priceMultiplier)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Pizza size name cannot be empty", nameof(name));
        
        if (diameterInCm <= 0)
            throw new ArgumentException("Diameter must be positive", nameof(diameterInCm));
        
        if (priceMultiplier <= 0)
            throw new ArgumentException("Price multiplier must be positive", nameof(priceMultiplier));

        return new PizzaSize(name, diameterInCm, priceMultiplier);
    }
    public void UpdatePriceMultiplier(decimal newMultiplier)
    {
        if (newMultiplier <= 0)
            throw new ArgumentException("Price multiplier must be positive", nameof(newMultiplier));

        PriceMultiplier = newMultiplier;
    }

    public void UpdateDetails(string name, int diameterInCm)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Pizza size name cannot be empty", nameof(name));
        
        if (diameterInCm <= 0)
            throw new ArgumentException("Diameter must be positive", nameof(diameterInCm));

        Name = name;
        DiameterInCm = diameterInCm;
    }
    public decimal CalculatePrice(decimal basePrice) => basePrice * PriceMultiplier;
    public string ToCustomerDesctiption() => $"{Name} ({DiameterInCm}см)";
    public string ToInternalDesctiption() => $"{Name} ({DiameterInCm}см, x{PriceMultiplier})";
}