# Factory Pattern - DeliveryRouteFactory

## 🎯 Что это?

**Factory** - паттерн для создания сложных объектов, когда обычного конструктора недостаточно.

## 🤔 Почему нужна Factory?

### ❌ Конструктор НЕ МОЖЕТ:

```csharp
public DeliveryRoute(...)
{
    // ❌ Конструкторы не могут быть async
    var traffic = await trafficService.GetTrafficAsync();
    
    // ❌ Конструкторы не должны иметь зависимости от сервисов
    public DeliveryRoute(ITrafficService trafficService, ...) 
    
    // ❌ Возвращает только объект (не метаданные)
}
```

### ✅ Factory МОЖЕТ:

```csharp
public class DeliveryRouteFactory
{
    private readonly ITrafficService _trafficService;
    
    // ✅ Async операции
    public async Task<DeliveryRouteCreationResult> CreateOptimalRouteAsync(...)
    {
        // ✅ Получаем данные о пробках (внешний API)
        var traffic = await _trafficService.GetCurrentTrafficAsync(...);
        
        // ✅ Валидация с детальными ошибками
        if (totalDuration > TimeSpan.FromHours(4))
            throw new InvalidOperationException("Route too long!");
        
        // ✅ Создаем маршрут через internal конструктор
        var route = new DeliveryRoute(...);
        
        // ✅ Возвращаем богатый результат (не просто объект!)
        return new DeliveryRouteCreationResult(
            Route: route,
            OptimizationSavings: ...,
            TrafficImpact: ...,
            Warnings: ...
        );
    }
}
```

## 💻 Пример использования

```csharp
// Создаем маршрут через фабрику
var result = await deliveryRouteFactory.CreateOptimalRouteAsync(
    courier,
    pendingOrders,
    plannedStartTime: DateTime.Now.AddMinutes(10)
);

// Получаем не просто маршрут, а полную информацию
Console.WriteLine(result.GetDescription());
```

**Вывод:**
```
Маршрут #550e8400... создан!
Остановок: 4
Расстояние: 12.3 км
Время: ~45 мин
Пробки: +18 мин задержки

💡 Оптимизация сэкономила 2.5 км

⚠️ Предупреждения:
  - Heavy traffic: +18 min delay
  - Long route: 45 min
```

## 🔑 Ключевые особенности

### 1. Внешняя зависимость
```csharp
public DeliveryRouteFactory(
    ITrafficService trafficService)   // Данные о пробках из внешнего API
```

### 2. Async операция
```csharp
// Получаем данные о пробках (внешний API вызов)
var traffic = await _trafficService.GetCurrentTrafficAsync(...);
```

### 3. Валидации
```csharp
if (orders.Count > MaxOrdersPerRoute)
    throw new InvalidOperationException("Too many orders!");

if (totalDuration > TimeSpan.FromHours(4))
    throw new InvalidOperationException("Route too long!");
```

### 4. Богатый результат
```csharp
return new DeliveryRouteCreationResult(
    Route: route,                    // Сам объект
    OptimizationSavings: ...,        // Экономия от оптимизации
    TrafficImpact: ...,              // Влияние пробок
    Warnings: ...                    // Предупреждения
);
```

## 📊 Factory vs Конструктор

| Что нужно | Конструктор | Factory |
|-----------|-------------|---------|
| Простое создание | ✅ | ⚠️ Избыточно |
| Async операции | ❌ | ✅ |
| Внешние сервисы | ❌ | ✅ |
| Сложные валидации | ⚠️ Ограничено | ✅ |
| Богатый результат | ❌ | ✅ |

## 🎯 Когда использовать Factory?

✅ Используйте Factory когда:
- Нужны async операции при создании
- Требуются внешние сервисы (API, БД)
- Сложные валидации с детальными ошибками
- Нужен результат с метаданными

❌ НЕ нужна Factory когда:
- Простое создание без логики
- Все данные есть сразу
- Не нужны внешние вызовы

---

📚 **См. также:**
- Сравнение с Domain Service: `Examples/FACTORY_VS_SERVICE.md`
- Пример кода: `DeliveryRouteFactory.cs`
