namespace MyAmazingDddTemplate.Domain.Examples.Orders.Services;

/// <summary>
/// Доменный сервис для работы с бонусной программой лояльности
/// </summary>
public interface IBonusService
{
    /// <summary>
    /// Рассчитывает количество бонусных баллов, которые клиент получит за заказ
    /// Учитывает:
    /// - Сумму заказа
    /// - Уровень клиента в программе лояльности (Bronze/Silver/Gold/Platinum)
    /// - Текущие акции по удвоению бонусов
    /// </summary>
    int CalculateEarnedPoints(decimal orderAmount, Guid customerId);
}

