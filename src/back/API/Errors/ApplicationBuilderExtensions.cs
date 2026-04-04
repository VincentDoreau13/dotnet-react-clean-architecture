namespace ShopApi.API.Errors;

public static class ApplicationBuilderExtensions
{
    public static void UseErrors(this IApplicationBuilder application, IHostEnvironment environment)
    {
        if (environment.IsDevelopment() || environment.IsEnvironment("Docker"))
        {
            application.Use(CustomErrorHandlerHelper.WriteDevelopmentResponse);
        }
        else
        {
            application.Use(CustomErrorHandlerHelper.WriteProductionResponse);
        }
    }
}
