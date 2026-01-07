namespace MyAmazingDddTemplate.Domain.Examples.Orders.Models;

/// <summary>
/// Результат размещения заказа через OrderPlacementService
/// Содержит всю информацию о размещенном заказе и примененных условиях
/// </summary>
public record OrderPlacementResult(
    Order Order,
    Guid RestaurantId,
    string RestaurantName,
    DateTime EstimatedCookingStartTime,
    DateTime EstimatedDeliveryTime,
    decimal AppliedDiscount,
    AppliedPromotion? AppliedPromotion,
    int EarnedBonusPoints,
    decimal TotalAmount,
    PaymentMethod PaymentMethod)
{
    /// <summary>
    /// Сумма без скидки
    /// </summary>
    public decimal OriginalAmount => Order.TotalAmount;

    /// <summary>
    /// Процент скидки
    /// </summary>
    public decimal DiscountPercentage => OriginalAmount > 0
        ? Math.Round((AppliedDiscount / OriginalAmount) * 100, 2)
        : 0;

    /// <summary>
    /// Примерное время доставки в минутах от текущего момента
    /// </summary>
    public int EstimatedDeliveryMinutes =>
        (int)(EstimatedDeliveryTime - DateTime.UtcNow).TotalMinutes;

    /// <summary>
    /// Описание для клиента
    /// </summary>
    public string GetCustomerDescription()
    {
        var description = $"Заказ №{Order.Id} успешно размещен!\n";
        description += $"Ресторан: {RestaurantName}\n";
        description += $"Начало приготовления: {EstimatedCookingStartTime:HH:mm}\n";
        description += $"Ожидаемое время доставки: {EstimatedDeliveryTime:HH:mm} (~{EstimatedDeliveryMinutes} мин)\n";
        description += $"Сумма заказа: {OriginalAmount:C}\n";

        if (AppliedDiscount > 0)
        {
            description += $"Скидка: -{AppliedDiscount:C} ({DiscountPercentage}%)\n";
            description += $"К оплате: {TotalAmount:C}\n";
        }

        if (EarnedBonusPoints > 0)
        {
            description += $"Вы получите {EarnedBonusPoints} бонусных баллов\n";
        }

        return description;
    }
}

