namespace Application.Abstraction
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void BeginTransaction();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
