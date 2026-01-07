using System.Collections.ObjectModel;
using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Customers;

public class Customer : Entity<Guid>
{
    private readonly List<CustomerDeliveryAddress> _customerDeliveryAddresses = [];
    
    public Name Name { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public ReadOnlyCollection<CustomerDeliveryAddress> CustomerDeliveryAddresses => _customerDeliveryAddresses.AsReadOnly();
}
