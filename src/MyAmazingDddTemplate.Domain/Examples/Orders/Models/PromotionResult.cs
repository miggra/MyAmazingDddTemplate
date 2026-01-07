namespace MyAmazingDddTemplate.Domain.Examples.Orders.Models;

/// <summary>
/// Результат применения промокода
/// </summary>
public record PromotionResult(
    bool IsValid,
    string Code,
    string Description,
    decimal DiscountAmount,
    DiscountType DiscountType,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Создать успешный результат
    /// </summary>
    public static PromotionResult Success(
        string code,
        string description,
        decimal discountAmount,
        DiscountType discountType)
    {
        return new PromotionResult(
            IsValid: true,
            Code: code,
            Description: description,
            DiscountAmount: discountAmount,
            DiscountType: discountType);
    }

    /// <summary>
    /// Создать неуспешный результат
    /// </summary>
    public static PromotionResult Failure(string errorMessage)
    {
        return new PromotionResult(
            IsValid: false,
            Code: string.Empty,
            Description: string.Empty,
            DiscountAmount: 0,
            DiscountType: DiscountType.Fixed,
            ErrorMessage: errorMessage);
    }
}

