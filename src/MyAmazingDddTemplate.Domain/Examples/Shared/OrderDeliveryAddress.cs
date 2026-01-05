using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Shared;

public sealed record DeliveryAddress ( 
    string Address,
    Coordinates Coordinates
);