using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Customers;

public class CustomerDeliveryAddress : Entity<Guid>
{ 
    public required Name Name { get; set; }
    public required DeliveryAddress Address { get; set; }
}