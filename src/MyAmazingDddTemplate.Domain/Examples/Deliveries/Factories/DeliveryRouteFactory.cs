using MyAmazingDddTemplate.Domain.Examples.Orders;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Deliveries.Factories;

/// <summary>
/// Фабрика для создания оптимального маршрута доставки
/// 
/// Паттерн: Factory
/// 
/// Почему фабрика, а не конструктор?
/// 1. Требуется зависимость от внешнего сервиса (данные о пробках)
/// 2. Нужны async операции для получения внешних данных
/// 3. Сложные валидации с выбросом исключений
/// 4. Возвращаем сложный результат (не просто объект)
/// </summary>
public class DeliveryRouteFactory
{
    private readonly ITrafficService _trafficService;

    public DeliveryRouteFactory(ITrafficService trafficService)
    {
        _trafficService = trafficService ?? throw new ArgumentNullException(nameof(trafficService));
    }

    /// <summary>
    /// Создать оптимальный маршрут доставки для курьера
    /// Демонстрирует ключевые особенности Factory:
    /// - Async операции с внешним сервисом
    /// - Валидации с выбросом исключений
    /// - Возврат богатого результата
    /// </summary>
    public async Task<DeliveryRoute> CreateRouteAsync(
        Courier courier,
        List<Order> pendingOrders,
        DateTime plannedStartTime)
    {
        if (courier == null)
            throw new ArgumentNullException(nameof(courier));

        if (pendingOrders == null || !pendingOrders.Any())
            throw new ArgumentException("Must have at least one order", nameof(pendingOrders));

        // Шаг 1: Валидация бизнес-правил
        ValidateCourierCapacity(courier, pendingOrders);

        // Шаг 2: Получение данных о пробках (async + внешний сервис)
        var trafficData = await _trafficService.GetCurrentTrafficAsync(
            pendingOrders.Select(o => o.OrderDeliveryAddress).ToList());

        // Шаг 3: Построение маршрута с остановками
        var stops = BuildRouteStops(pendingOrders, plannedStartTime, trafficData);
        var totalDistance = stops.Sum(s => s.DistanceFromPreviousKm);
        var totalDuration = stops.Last().EstimatedArrivalTime - plannedStartTime;

        // Шаг 4: Финальная валидация
        if (totalDuration > TimeSpan.FromHours(4))
        {
            throw new InvalidOperationException(
                $"Route duration ({totalDuration.TotalMinutes:F0} min) exceeds maximum 4 hours");
        }

        // Шаг 5: Создание маршрута через internal конструктор
        var route = new DeliveryRoute(
            courier.Id,
            plannedStartTime,
            stops,
            totalDistance,
            totalDuration);

        // Шаг 6: Формирование богатого результата (не просто объект!)
        return route;
    }

    private void ValidateCourierCapacity(Courier courier, List<Order> orders)
    {
        const int MaxOrdersPerRoute = 5;

        if (orders.Count > MaxOrdersPerRoute)
        {
            throw new InvalidOperationException(
                $"Cannot create route with {orders.Count} orders. Maximum {MaxOrdersPerRoute}");
        }

        var totalVolume = orders.Sum(o => o.OrderItems.Count);
        if (totalVolume > courier.BagCapacity)
        {
            throw new InvalidOperationException(
                $"Orders volume ({totalVolume}) exceeds bag capacity ({courier.BagCapacity})");
        }
    }

    private List<DeliveryStop> BuildRouteStops(
        List<Order> orders,
        DateTime startTime,
        TrafficData trafficData)
    {
        var stops = new List<DeliveryStop>();
        var currentTime = startTime;

        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            var distance = 3m + i; // Упрощенный расчет расстояния
            var travelTime = CalculateTravelTime(distance, trafficData);
            
            currentTime = currentTime.Add(travelTime).Add(TimeSpan.FromMinutes(5)); // +5 мин на доставку

            var stop = new DeliveryStop(
                order.Id,
                order.OrderDeliveryAddress,
                new Coordinates(55.75m + i * 0.01m, 37.62m + i * 0.01m), // Упрощенно
                sequenceNumber: i + 1,
                estimatedArrivalTime: currentTime,
                distanceFromPreviousKm: distance);

            stops.Add(stop);
        }

        return stops;
    }

    private TimeSpan CalculateTravelTime(
        decimal distanceKm,
        TrafficData trafficData)
    {
        var baseSpeedKmh = 30m; // Базовая скорость в городе
        
        // Корректировка на пробки (чем больше задержка, тем медленнее)
        var trafficFactor = trafficData.AverageDelayMinutes > 15 ? 0.6m : 1.0m;
        
        var actualSpeed = baseSpeedKmh * trafficFactor;
        var hours = distanceKm / actualSpeed;
        
        return TimeSpan.FromHours((double)hours);
    }

    private List<string> GenerateWarnings(
        TrafficData trafficData,
        TimeSpan totalDuration)
    {
        var warnings = new List<string>();

        if (trafficData.AverageDelayMinutes > 15)
        {
            warnings.Add($"Heavy traffic: +{trafficData.AverageDelayMinutes} min delay");
        }

        if (totalDuration.TotalHours > 3)
        {
            warnings.Add($"Long route: {totalDuration.TotalMinutes:F0} min");
        }

        return warnings;
    }
}

