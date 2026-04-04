using MediatR;
using Serilog;
using ShopApi.Domain.Exceptions;

namespace ShopApi.Application.Common.Behaviours;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Log.Information("Handling {CQRSName}", typeof(TRequest).Name);
        try
        {
            TResponse response = await next();
            Log.Information("Handled {CQRSName}", typeof(TRequest).Name);
            return response;
        }
        catch (NotFoundException notFoundException)
        {
            Log.Error("{Code} - {Message}", notFoundException.Code, notFoundException.Message);
            throw;
        }
        catch (FunctionalException functionalException)
        {
            Log.Error("{Code} - {Message}", functionalException.Code, functionalException.Message);
            throw;
        }
        catch (Exception exception)
        {
            Log.Fatal("Server Error : {@Exception}", exception);
            throw;
        }
    }
}
