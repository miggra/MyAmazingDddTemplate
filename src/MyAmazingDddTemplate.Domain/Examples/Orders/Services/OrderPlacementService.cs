using MyAmazingDddTemplate.Domain.Examples.Customers;
using MyAmazingDddTemplate.Domain.Examples.Orders.Models;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Orders.Services;

/// <summary>
/// Доменный сервис для размещения заказа
/// 
/// Паттерн: Domain Service
/// 
/// Почему Domain Service, а не метод Order?
/// 1. Операция координирует несколько агрегатов (Order, Customer, Restaurant, Promotion)
/// 2. Требуются внешние проверки (доступность в ресторане, применение промокода)
/// 3. Логика не принадлежит естественно ни Order, ни Customer
/// 4. Работает с СУЩЕСТВУЮЩИМИ объектами (не создает новые)
/// </summary>
public class OrderPlacementService
{
    private readonly IRestaurantSelector _restaurantSelector;
    private readonly IPromotionService _promotionService;
    private readonly IBonusService _bonusService;

    public OrderPlacementService(
        IRestaurantSelector restaurantSelector,
        IPromotionService promotionService,
        IBonusService bonusService)
    {
        _restaurantSelector = restaurantSelector ?? throw new ArgumentNullException(nameof(restaurantSelector));
        _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        _bonusService = bonusService ?? throw new ArgumentNullException(nameof(bonusService));
    }

    /// <summary>
    /// Размещение заказа с координацией бизнес-процесса
    /// Демонстрирует ключевые особенности Domain Service:
    /// - Координация нескольких агрегатов
    /// - Работа с существующими объектами (не создание!)
    /// - Применение бизнес-правил
    /// - Изменение состояния (Draft → Confirmed)
    /// </summary>
    public async Task<OrderPlacementResult> PlaceOrderAsync(
        Order draftOrder,
        Customer customer,
        PaymentMethod paymentMethod,
        string? promoCode = null)
    {
        if (draftOrder == null)
            throw new ArgumentNullException(nameof(draftOrder));

        if (customer == null)
            throw new ArgumentNullException(nameof(customer));

        // Шаг 2: Выбор ресторана для доставки (координация с Restaurant)
        var restaurant = await _restaurantSelector.SelectRestaurantAsync(
            draftOrder.OrderDeliveryAddress,
            draftOrder.OrderItems);

        if (restaurant == null)
            throw new InvalidOperationException("No restaurant available for delivery");

        // Шаг 3: Применение промокода (координация с Promotion)
        decimal discount = 0;
        AppliedPromotion? appliedPromotion = null;

        if (!string.IsNullOrEmpty(promoCode))
        {
            var promotionResult = await _promotionService.ApplyPromotionAsync(
                promoCode,
                draftOrder.TotalAmount,
                customer.Id,
                draftOrder.OrderItems);

            if (promotionResult.IsValid)
            {
                discount = promotionResult.DiscountAmount;
                appliedPromotion = new AppliedPromotion(
                    promotionResult.Code,
                    promotionResult.Description,
                    promotionResult.DiscountAmount,
                    promotionResult.DiscountType);
            }
        }

        // Шаг 4: Начисление бонусных баллов (координация с Customer/Bonus)
        var finalAmount = draftOrder.TotalAmount - discount;
        var earnedBonusPoints = _bonusService.CalculateEarnedPoints(
            finalAmount,
            customer.Id);

        // Шаг 5: Подтверждение заказа (вызов доменного метода агрегата)
        // Domain Service НЕ меняет Order напрямую, а вызывает его метод!
        draftOrder.Confirm();

        // Шаг 6: Формирование результата операции
        return new OrderPlacementResult(
            Order: draftOrder,
            RestaurantId: restaurant.Id,
            RestaurantName: restaurant.Name,
            EstimatedCookingStartTime: DateTime.UtcNow.AddMinutes(10),
            EstimatedDeliveryTime: DateTime.UtcNow.AddMinutes(40),
            AppliedDiscount: discount,
            AppliedPromotion: appliedPromotion,
            EarnedBonusPoints: earnedBonusPoints,
            TotalAmount: finalAmount,
            PaymentMethod: paymentMethod
        );
    }
}

