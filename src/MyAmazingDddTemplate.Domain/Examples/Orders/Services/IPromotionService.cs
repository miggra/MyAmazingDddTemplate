using MyAmazingDddTemplate.Domain.Examples.Orders.Models;

namespace MyAmazingDddTemplate.Domain.Examples.Orders.Services;

/// <summary>
/// Доменный сервис для работы с промокодами и акциями
/// </summary>
public interface IPromotionService
{
    /// <summary>
    /// Применяет промокод к заказу и возвращает информацию о скидке
    /// Проверяет:
    /// - Существование и валидность промокода
    /// - Применимость к данному клиенту
    /// - Применимость к позициям заказа
    /// - Минимальную сумму заказа для применения
    /// - Срок действия промокода
    /// </summary>
    Task<PromotionResult> ApplyPromotionAsync(
        string promoCode,
        decimal orderAmount,
        Guid customerId,
        IEnumerable<OrderItem> orderItems);
}

