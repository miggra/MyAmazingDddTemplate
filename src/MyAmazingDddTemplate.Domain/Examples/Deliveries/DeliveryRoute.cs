using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Orders;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Deliveries;

/// <summary>
/// Маршрут доставки для курьера
/// Aggregate Root
/// </summary>
public class DeliveryRoute : Entity<Guid>, IAggregateRoot
{
    private readonly List<DeliveryStop> _stops = [];

    // Приватный конструктор без параметров для инфраструктуры
    private DeliveryRoute() : base() { }

    // Внутренний конструктор - используется только фабрикой
    internal DeliveryRoute(
        Guid courierId,
        DateTime plannedStartTime,
        List<DeliveryStop> stops,
        decimal totalDistanceKm,
        TimeSpan estimatedDuration) : base(Guid.NewGuid())
    {
        CourierId = courierId;
        PlannedStartTime = plannedStartTime;
        CreatedAt = DateTime.UtcNow;
        Status = DeliveryRouteStatus.Planned;
        TotalDistanceKm = totalDistanceKm;
        EstimatedDuration = estimatedDuration;
        
        foreach (var stop in stops)
        {
            _stops.Add(stop);
        }
    }

    public Guid CourierId { get; private set; }
    public DateTime PlannedStartTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DeliveryRouteStatus Status { get; private set; }
    public decimal TotalDistanceKm { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }

    public IReadOnlyCollection<DeliveryStop> Stops => _stops.AsReadOnly();

    // Доменные методы для управления маршрутом
    public void StartRoute()
    {
        if (Status != DeliveryRouteStatus.Planned)
            throw new InvalidOperationException("Route must be in Planned status");

        Status = DeliveryRouteStatus.InProgress;
        StartedAt = DateTime.UtcNow;
    }

    public void CompleteStop(Guid stopId)
    {
        if (Status != DeliveryRouteStatus.InProgress)
            throw new InvalidOperationException("Route must be in progress");

        var stop = _stops.FirstOrDefault(s => s.Id == stopId);
        if (stop == null)
            throw new InvalidOperationException($"Stop {stopId} not found");

        stop.MarkAsCompleted();

        // Если все остановки выполнены - маршрут завершен
        if (_stops.All(s => s.IsCompleted))
        {
            Status = DeliveryRouteStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }
    }

    public void Cancel(string reason)
    {
        if (Status == DeliveryRouteStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed route");

        Status = DeliveryRouteStatus.Cancelled;
    }

    public TimeSpan? GetActualDuration()
    {
        if (StartedAt.HasValue && CompletedAt.HasValue)
        {
            return CompletedAt.Value - StartedAt.Value;
        }
        return null;
    }
}

/// <summary>
/// Остановка в маршруте доставки
/// Entity, но не Aggregate Root (часть DeliveryRoute)
/// </summary>
public class DeliveryStop : Entity<Guid>
{
    // Приватный конструктор для инфраструктуры
    private DeliveryStop() : base() { }

    internal DeliveryStop(
        Guid orderId,
        DeliveryAddress address,
        Coordinates coordinates,
        int sequenceNumber,
        DateTime estimatedArrivalTime,
        decimal distanceFromPreviousKm) : base(Guid.NewGuid())
    {
        OrderId = orderId;
        Address = address;
        Coordinates = coordinates;
        SequenceNumber = sequenceNumber;
        EstimatedArrivalTime = estimatedArrivalTime;
        DistanceFromPreviousKm = distanceFromPreviousKm;
        IsCompleted = false;
    }

    public Guid OrderId { get; private set; }
    public DeliveryAddress Address { get; private set; } = null!;
    public Coordinates Coordinates { get; private set; } = null!;
    public int SequenceNumber { get; private set; }
    public DateTime EstimatedArrivalTime { get; private set; }
    public DateTime? ActualArrivalTime { get; private set; }
    public decimal DistanceFromPreviousKm { get; private set; }
    public bool IsCompleted { get; private set; }

    internal void MarkAsCompleted()
    {
        IsCompleted = true;
        ActualArrivalTime = DateTime.UtcNow;
    }
}

/// <summary>
/// Статус маршрута доставки
/// </summary>
public enum DeliveryRouteStatus
{
    Planned = 0,      // Запланирован
    InProgress = 1,   // В процессе выполнения
    Completed = 2,    // Завершен
    Cancelled = 3     // Отменен
}

