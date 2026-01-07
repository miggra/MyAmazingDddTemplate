namespace MyAmazingDddTemplate.Domain.Examples.Orders.Models;

/// <summary>
/// Расчет времени доставки
/// </summary>
public record DeliveryEstimation(
    DateTime CookingStartTime,
    DateTime DeliveryTime,
    TimeSpan CookingDuration,
    TimeSpan DeliveryDuration)
{
    /// <summary>
    /// Общее время от размещения заказа до доставки
    /// </summary>
    public TimeSpan TotalDuration => CookingDuration + DeliveryDuration;

    /// <summary>
    /// Общее время в минутах
    /// </summary>
    public int TotalMinutes => (int)TotalDuration.TotalMinutes;

    /// <summary>
    /// Описание для клиента
    /// </summary>
    public string GetDescription()
    {
        return $"Приготовление: {(int)CookingDuration.TotalMinutes} мин, " +
               $"Доставка: {(int)DeliveryDuration.TotalMinutes} мин, " +
               $"Всего: ~{TotalMinutes} мин";
    }
}

