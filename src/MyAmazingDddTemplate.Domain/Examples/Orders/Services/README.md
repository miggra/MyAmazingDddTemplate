# Domain Service - OrderPlacementService

## 🎯 Что это?

**Domain Service** - паттерн для координации операций над несколькими агрегатами.

## 🤔 Почему нужен Domain Service?

### ❌ Метод Order НЕ ДОЛЖЕН:

```csharp
public class Order
{
    // ❌ Order не должен знать о ресторанах
    public void PlaceOrder(IRestaurantRepository restaurantRepo)
    {
        var restaurant = restaurantRepo.FindNearest(...);
    }
    
    // ❌ Order не должен знать о промокодах
    public void ApplyPromoCode(IPromotionRepository promoRepo)
    {
        var promo = promoRepo.GetByCode(...);
    }
    
    // ❌ Order не должен знать о бонусах
    public void CalculateBonus(IBonusCalculator bonusCalc)
    {
        var bonus = bonusCalc.Calculate(...);
    }
}
```

**Проблемы:**
- Order раздут и делает слишком много
- Нарушение SRP (Single Responsibility Principle)
- Зависимость от инфраструктуры

### ✅ Domain Service МОЖЕТ:

```csharp
public class OrderPlacementService
{
    private readonly IRestaurantSelector _restaurantSelector;
    private readonly IPromotionService _promotionService;
    private readonly IBonusService _bonusService;
    
    // ✅ Координирует бизнес-процесс
    public async Task<OrderPlacementResult> PlaceOrderAsync(
        Order draftOrder,    // Существующий Order!
        Customer customer,   // Существующий Customer!
        PaymentMethod payment,
        string? promoCode)
    {
        // ✅ Выбор ресторана
        var restaurant = await _restaurantSelector.SelectRestaurantAsync(...);
        
        // ✅ Применение промокода
        var promo = await _promotionService.ApplyPromotionAsync(...);
        
        // ✅ Начисление бонусов
        var bonus = _bonusService.CalculateEarnedPoints(...);
        
        // ✅ Изменение состояния через метод агрегата
        draftOrder.Confirm();  // Draft → Confirmed
        
        // ✅ Возврат результата операции
        return new OrderPlacementResult(...);
    }
}
```

## 💻 Пример использования

```csharp
// 1. Создаем корзину (Draft Order)
var cart = Order.Create(address);
cart.AddOrderItem(pizza1);
cart.AddOrderItem(pizza2);

// 2. Размещаем через Domain Service
var result = await orderPlacementService.PlaceOrderAsync(
    cart,
    customer,
    PaymentMethod.CardOnline,
    promoCode: "PIZZA20"
);

// 3. Заказ размещен, статус изменен
Console.WriteLine($"Order #{result.Order.Id} placed!");
Console.WriteLine($"Status: {result.Order.Status}"); // Confirmed
Console.WriteLine($"Restaurant: {result.RestaurantName}");
Console.WriteLine($"Total: {result.TotalAmount:C}");
Console.WriteLine($"Discount: {result.AppliedDiscount:C}");
Console.WriteLine($"Bonus: +{result.EarnedBonusPoints} points");
```

**Вывод:**
```
Order #550e8400... placed!
Status: Confirmed
Restaurant: Mama Roma
Total: 1200₽
Discount: -300₽
Bonus: +60 points
```

## 🔑 Ключевые особенности

### 1. Координация агрегатов
```csharp
// Работает с Order, Customer, Restaurant, Promotion
var restaurant = await _restaurantSelector.SelectRestaurantAsync(...);
var promo = await _promotionService.ApplyPromotionAsync(...);
var bonus = _bonusService.CalculateEarnedPoints(...);
```

### 2. Работа с существующими объектами
```csharp
// Принимает УЖЕ созданные Order и Customer
public async Task<OrderPlacementResult> PlaceOrderAsync(
    Order draftOrder,     // ← Существует!
    Customer customer)    // ← Существует!
```

### 3. Изменение состояния
```csharp
// НЕ меняет Order напрямую, а вызывает его метод
draftOrder.Confirm();  // Draft → Confirmed
```

### 4. Результат операции
```csharp
return new OrderPlacementResult(
    Order: draftOrder,           // Измененный Order
    RestaurantName: ...,         // Информация
    AppliedDiscount: ...,        // Результаты
    EarnedBonusPoints: ...       // Бонусы
);
```

## 📊 Domain Service vs Метод агрегата

| Операция | Метод Order | Domain Service |
|----------|-------------|----------------|
| `order.AddItem()` | ✅ | ❌ |
| `order.Cancel()` | ✅ | ❌ |
| `order.Confirm()` | ✅ | ❌ |
| Выбор ресторана | ❌ | ✅ |
| Применение промокода | ❌ | ✅ |
| Начисление бонусов | ❌ | ✅ |
| Размещение заказа | ❌ | ✅ |

**Правило:** Если операция естественно принадлежит агрегату - это метод. Если координирует несколько агрегатов - Domain Service.

## 🎯 Когда использовать Domain Service?

✅ Используйте Domain Service когда:
- Операция координирует несколько агрегатов
- Логика не принадлежит ни одному агрегату
- Нужны внешние проверки/расчеты
- Работаете с существующими объектами

❌ НЕ нужен Domain Service когда:
- Операция относится к одному агрегату → метод агрегата
- Создание нового объекта → Factory
- Работа с БД → Repository (Application Service)

## 🆚 Domain Service vs Factory

```
Factory СОЗДАЕТ:
[Параметры] → Factory → [Новый объект]

Domain Service КООРДИНИРУЕТ:
[Существующие объекты] → Service → [Результат операции]
```

---

📚 **См. также:**
- Сравнение с Factory: `Examples/FACTORY_VS_SERVICE.md`
- Пример кода: `OrderPlacementService.cs`
