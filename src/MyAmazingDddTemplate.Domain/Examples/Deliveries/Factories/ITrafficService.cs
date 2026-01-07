using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Deliveries.Factories;

/// <summary>
/// Сервис для получения данных о дорожной обстановке (пробках)
/// Интерфейс определен в Domain, реализация будет в Infrastructure
/// </summary>
public interface ITrafficService
{
    /// <summary>
    /// Получить текущую информацию о пробках для списка адресов
    /// </summary>
    Task<TrafficData> GetCurrentTrafficAsync(List<DeliveryAddress> addresses);
}

/// <summary>
/// Данные о дорожной обстановке
/// </summary>
public class TrafficData
{
    public DateTime Timestamp { get; init; }
    public int AverageDelayMinutes { get; init; }
    private readonly Dictionary<string, TrafficLevel> _trafficLevels = new();

    public TrafficLevel GetTrafficLevel(Coordinates from, Coordinates to)
    {
        // Упрощенная логика для примера
        // В реальности здесь был бы поиск по маршруту между точками
        var key = $"{from.Latitude},{from.Longitude}-{to.Latitude},{to.Longitude}";
        return _trafficLevels.TryGetValue(key, out var level) 
            ? level 
            : TrafficLevel.Light;
    }

    public void AddTrafficLevel(Coordinates from, Coordinates to, TrafficLevel level)
    {
        var key = $"{from.Latitude},{from.Longitude}-{to.Latitude},{to.Longitude}";
        _trafficLevels[key] = level;
    }
}

/// <summary>
/// Уровень загруженности дорог
/// </summary>
public enum TrafficLevel
{
    Free = 0,       // Свободно
    Light = 1,      // Легкие пробки
    Moderate = 2,   // Средние пробки
    Heavy = 3,      // Сильные пробки
    Jam = 4         // Затор
}

