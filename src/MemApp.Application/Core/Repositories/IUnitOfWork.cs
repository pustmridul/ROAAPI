using MemApp.Domain.Core.Models;

namespace MemApp.Application.Core.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task RollBackChangesAsync();
        IBaseRepositoryAsync<T> Repository<T>() where T : BaseEntity;
    }
}