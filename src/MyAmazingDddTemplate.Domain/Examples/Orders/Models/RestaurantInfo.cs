using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Orders.Models;

/// <summary>
/// Информация о ресторане для доменной логики заказов
/// Упрощенное представление ресторана для OrderPlacementService
/// </summary>
public record RestaurantInfo(
    Guid Id,
    string Name,
    Coordinates Coordinates,
    DeliveryZone DeliveryZone,
    int CurrentKitchenLoad,
    int MaxKitchenCapacity)
{
    /// <summary>
    /// Процент загруженности кухни (0-100)
    /// </summary>
    public int LoadPercentage => MaxKitchenCapacity > 0
        ? (int)((CurrentKitchenLoad / (decimal)MaxKitchenCapacity) * 100)
        : 0;

    /// <summary>
    /// Кухня перегружена?
    /// </summary>
    public bool IsOverloaded => LoadPercentage > 90;

    /// <summary>
    /// Может ли ресторан принять новый заказ?
    /// </summary>
    public bool CanAcceptOrder => CurrentKitchenLoad < MaxKitchenCapacity;
}

/// <summary>
/// Зона доставки ресторана
/// </summary>
public record DeliveryZone(
    decimal RadiusInKm,
    decimal MinOrderAmount,
    decimal DeliveryFee)
{
    /// <summary>
    /// Проверяет, находится ли адрес в зоне доставки
    /// </summary>
    public bool IsWithinZone(Coordinates restaurantLocation, Coordinates deliveryLocation)
    {
        var distance = CalculateDistance(restaurantLocation, deliveryLocation);
        return distance <= RadiusInKm;
    }

    private static decimal CalculateDistance(Coordinates point1, Coordinates point2)
    {
        // Упрощенный расчет расстояния (в реальности используется формула Haversine)
        var dx = point2.Latitude - point1.Latitude;
        var dy = point2.Longitude - point1.Longitude;
        return (decimal)Math.Sqrt((double)(dx * dx + dy * dy)) * 111; // приблизительно км
    }
}

