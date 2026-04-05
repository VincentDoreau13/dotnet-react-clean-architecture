namespace ShopApi.Domain.Common;

/// <summary>
/// Audit fields — written exclusively by MustHaveAuditableBeforeSavingStrategy via the EF property API.
/// Application and domain code must treat these as read-only.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
    string? CreatedBy { get; }
    string? UpdatedBy { get; }
}
