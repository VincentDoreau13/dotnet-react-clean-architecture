using System.Diagnostics;
using ShopApi.Application.Common.Interfaces;

namespace ShopApi.Infrastructure.Services;

public class IdentityService(IHttpContextAccessor httpContextAccessor) : IIdentityService
{
    public string? GetUserIdentity()
        => httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value
           ?? httpContextAccessor.HttpContext?.User.FindFirst("client_id")?.Value;

    public string GetUsername()
        => httpContextAccessor.HttpContext?.User.Identity?.Name
           ?? httpContextAccessor.HttpContext?.User.FindFirst(claim => claim.Type == "name")?.Value
           ?? "System";

    public string? GetTraceId()
        => httpContextAccessor.HttpContext?.TraceIdentifier
           ?? Activity.Current?.Context.TraceId.ToString();

    public List<string> GetRoles()
        => httpContextAccessor.HttpContext?.User.Claims
               .Where(claim => claim.Type == "role")
               .Select(claim => claim.Value)
               .ToList()
           ?? [];
}
