namespace MyAmazingDddTemplate.Domain.Examples.Orders.Models;

/// <summary>
/// Способ оплаты заказа
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Наличными при получении
    /// </summary>
    Cash = 0,

    /// <summary>
    /// Картой при получении
    /// </summary>
    CardOnDelivery = 1,

    /// <summary>
    /// Картой онлайн
    /// </summary>
    CardOnline = 2,

    /// <summary>
    /// Через Apple Pay
    /// </summary>
    ApplePay = 3,

    /// <summary>
    /// Через Google Pay
    /// </summary>
    GooglePay = 4,

    /// <summary>
    /// Бонусными баллами
    /// </summary>
    BonusPoints = 5
}

