namespace ShopApi.Domain.Common;

/// <summary>
/// Audit timestamps — written exclusively by MustHaveAuditableBeforeSavingStrategy via the EF property API.
/// Application and domain code must treat these as read-only.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
}
