namespace MyAmazingDddTemplate.Domain.Examples.Kitchen;

public enum CookingTaskStatus
{
    Pending = 0,        // Ожидает начала
    InProgress = 1,     // В процессе приготовления
    Completed = 2,      // Завершено
    Cancelled = 3       // Отменено
}

