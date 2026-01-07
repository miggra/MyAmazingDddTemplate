using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Customers;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Deliveries;

/// <summary>
/// Курьер доставки
/// </summary>
public class Courier : Entity<Guid>
{
    private Courier() : base() { }

    public Courier(
        Name name,
        PhoneNumber phoneNumber,
        Coordinates currentLocation,
        int bagCapacity = 10) : base(Guid.NewGuid())
    {
        Name = name;
        PhoneNumber = phoneNumber;
        CurrentLocation = currentLocation;
        BagCapacity = bagCapacity;
        IsAvailable = true;
    }

    public Name Name { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public Coordinates CurrentLocation { get; private set; } = null!;
    public int BagCapacity { get; private set; }
    public bool IsAvailable { get; private set; }

    public void UpdateLocation(Coordinates newLocation)
    {
        CurrentLocation = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
    }

    public void MarkAsUnavailable()
    {
        IsAvailable = false;
    }

    public void MarkAsAvailable()
    {
        IsAvailable = true;
    }
}

