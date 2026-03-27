using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }

    public interface ITransaction : IAsyncDisposable, IDisposable
    {
        Guid TransactionId { get; }
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
