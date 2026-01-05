using System.Collections.ObjectModel;
using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Customers;

public class Customer : Entity<Guid>
{
    private readonly List<CustomerDeliveryAddress> _customerDeliveryAddresses = [];
    
    public required Name Name { get; set; }
    public required PhoneNumber PhoneNumber { get; set; }
    public ReadOnlyCollection<CustomerDeliveryAddress> CustomerDeliveryAddresses => _customerDeliveryAddresses.AsReadOnly();
}
