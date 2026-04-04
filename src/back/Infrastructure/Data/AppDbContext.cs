using System.Data;
using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using ShopApi.Application.Common.Interfaces;
using ShopApi.Domain.Catalog;
using ShopApi.Domain.Common;
using ShopApi.Infrastructure.Strategies;

namespace ShopApi.Infrastructure.Data;

public class AppDbContext : DbContext, IUnitOfWork
{
    private static readonly EntityState[] ModifiedStates =
    [
        EntityState.Added,
        EntityState.Modified,
        EntityState.Deleted
    ];

    private readonly IEnumerable<IDbContextStrategy> _dbContextStrategies;
    private readonly IMediator _mediator;
    private IDbContextTransaction? _currentTransaction;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        IEnumerable<IDbContextStrategy> dbContextStrategies,
        IMediator mediator) : base(options)
    {
        _dbContextStrategies = dbContextStrategies;
        _mediator = mediator;

        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public bool HasActiveTransaction => _currentTransaction != null;

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            IEnumerable<IModelCreatingDbContextStrategy> modelCreatingDbContextStrategies = _dbContextStrategies
                .OfType<IModelCreatingDbContextStrategy>()
                .Where(strategy => strategy.CanExecute(modelBuilder, entityType));

            foreach (IModelCreatingDbContextStrategy modelCreatingDbContextStrategy in modelCreatingDbContextStrategies)
                modelCreatingDbContextStrategy.Execute(modelBuilder, entityType);
        }

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void OnBeforeSaving()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries().Where(IsModifiedEntity))
        {
            IEnumerable<IBeforeSavingDbContextStrategy> beforeSavingDbContextStrategies = _dbContextStrategies
                .OfType<IBeforeSavingDbContextStrategy>()
                .Where(strategy => strategy.CanExecute(entry));

            foreach (IBeforeSavingDbContextStrategy beforeSavingDbContextStrategy in beforeSavingDbContextStrategies)
                beforeSavingDbContextStrategy.Execute(entry);
        }
    }

    private static bool IsModifiedEntity(EntityEntry entityEntry)
    {
        return ModifiedStates.Contains(entityEntry.State) && entityEntry.Entity is BaseEntity;
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(this);
        await SaveChangesAsync(cancellationToken);
        return true;
    }

    // IUnitOfWork
    public async Task<TResult> ExecuteWithTransactionAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken)
    {
        IExecutionStrategy strategy = Database.CreateExecutionStrategy();
        TResult result = default!;

        await strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await BeginTransactionAsync()
                ?? throw new InvalidOperationException("Failed to begin transaction.");

            result = await action();

            await CommitTransactionAsync(transaction);
        });

        return result;
    }

    private async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        return _currentTransaction;
    }

    private async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        if (transaction != _currentTransaction)
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    private void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
