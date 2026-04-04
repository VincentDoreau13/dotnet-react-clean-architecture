using MediatR;
using ShopApi.Application.Common.Interfaces;

namespace ShopApi.Application.Common.Behaviours;

public class TransactionBehaviour<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (unitOfWork.HasActiveTransaction)
            return await next();

        return await unitOfWork.ExecuteWithTransactionAsync(() => next(), cancellationToken);
    }
}
