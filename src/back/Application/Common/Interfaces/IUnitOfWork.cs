namespace ShopApi.Application.Common.Interfaces;

public interface IUnitOfWork
{
    bool HasActiveTransaction { get; }

    Task<TResult> ExecuteWithTransactionAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken);
}
