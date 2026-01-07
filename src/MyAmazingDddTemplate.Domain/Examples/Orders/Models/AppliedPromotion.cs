namespace MyAmazingDddTemplate.Domain.Examples.Orders.Models;

/// <summary>
/// Информация о примененной акции/промокоде
/// </summary>
public record AppliedPromotion(
    string Code,
    string Description,
    decimal DiscountAmount,
    DiscountType DiscountType);

/// <summary>
/// Тип скидки
/// </summary>
public enum DiscountType
{
    /// <summary>
    /// Процентная скидка (например, 15%)
    /// </summary>
    Percentage,

    /// <summary>
    /// Фиксированная скидка (например, 500₽)
    /// </summary>
    Fixed,

    /// <summary>
    /// Бесплатная доставка
    /// </summary>
    FreeDelivery,

    /// <summary>
    /// Подарок (например, бесплатный напиток)
    /// </summary>
    FreeItem
}

