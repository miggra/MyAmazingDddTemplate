using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Customers;

public class CustomerDeliveryAddress : Entity<Guid>
{ 
    public Name Name { get; private set; }
    public DeliveryAddress Address { get; private set; }
}