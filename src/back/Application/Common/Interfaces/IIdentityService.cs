namespace ShopApi.Application.Common.Interfaces;

public interface IIdentityService
{
    string? GetUserIdentity();

    string GetUsername();

    string? GetTraceId();

    List<string> GetRoles();
}
