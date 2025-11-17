using MyAmazingDddTemplate.Domain.Shared;

namespace MyAmazingDddTemplate.Domain.Orders;

public sealed record OrderDeliveryAddress ( 
    string Address,
    Coordinates Coordinates
);