namespace MyAmazingDddTemplate.Domain.Examples.Deliveries.Factories;

/// <summary>
/// Результат создания маршрута доставки
/// Содержит не только сам маршрут, но и дополнительную информацию
/// о процессе оптимизации и внешних факторах
/// </summary>
public record DeliveryRouteCreationResult(
    DeliveryRoute Route,
    OptimizationSavings OptimizationSavings,
    int TrafficImpact,
    List<string> Warnings)
{
    /// <summary>
    /// Описание результата для курьера
    /// </summary>
    public string GetDescription()
    {
        var description = $"Маршрут #{Route.Id} создан!\n";
        description += $"Остановок: {Route.Stops.Count}\n";
        description += $"Расстояние: {Route.TotalDistanceKm:F1} км\n";
        description += $"Время: ~{Route.EstimatedDuration.TotalMinutes:F0} мин\n";
        description += $"Пробки: +{TrafficImpact} мин задержки\n";

        if (OptimizationSavings.SavedDistanceKm > 0)
        {
            description += $"\n💡 Оптимизация сэкономила {OptimizationSavings.SavedDistanceKm:F1} км";
        }

        if (Warnings.Any())
        {
            description += "\n\n⚠️ Предупреждения:\n";
            foreach (var warning in Warnings)
            {
                description += $"  - {warning}\n";
            }
        }

        return description;
    }

    /// <summary>
    /// Детали по остановкам
    /// </summary>
    public string GetStopsDetails()
    {
        var details = "Последовательность доставки:\n\n";
        
        foreach (var stop in Route.Stops.OrderBy(s => s.SequenceNumber))
        {
            details += $"{stop.SequenceNumber}. {stop.Address}\n";
            details += $"   Прибытие: {stop.EstimatedArrivalTime:HH:mm}\n";
            details += $"   Расстояние от предыдущей точки: {stop.DistanceFromPreviousKm:F1} км\n\n";
        }

        return details;
    }
}

/// <summary>
/// Экономия от оптимизации маршрута
/// </summary>
public record OptimizationSavings(
    decimal SavedDistanceKm,
    TimeSpan SavedTime)
{
    /// <summary>
    /// Экономия топлива (литры)
    /// Средний расход: 10л/100км
    /// </summary>
    public decimal SavedFuelLiters => SavedDistanceKm * 0.1m;

    /// <summary>
    /// Экономия денег на топливе (рубли)
    /// Цена топлива: ~55₽/литр
    /// </summary>
    public decimal SavedMoney => SavedFuelLiters * 55m;
}

