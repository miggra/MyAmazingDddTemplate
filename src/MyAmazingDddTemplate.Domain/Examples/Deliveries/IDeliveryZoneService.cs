using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

public interface IDeliveryZoneService
{
    Restaurant GetRestaurantByDeliveryZone(string address);
}

public interface IKitchenService
{

}

public class Restaurant : Entity<Guid>
{
    public Coordinates Coordinates { get; private set;}
}