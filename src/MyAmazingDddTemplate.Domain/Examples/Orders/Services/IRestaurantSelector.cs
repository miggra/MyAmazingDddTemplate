using MyAmazingDddTemplate.Domain.Examples.Orders.Models;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Orders.Services;

/// <summary>
/// Доменный сервис для выбора ресторана для доставки заказа
/// </summary>
public interface IRestaurantSelector
{
    /// <summary>
    /// Выбирает оптимальный ресторан для доставки заказа по указанному адресу
    /// Учитывает: расстояние, зону доставки, загруженность кухни
    /// </summary>
    Task<RestaurantInfo?> SelectRestaurantAsync(
        DeliveryAddress deliveryAddress,
        IEnumerable<OrderItem> orderItems);
}

