using HelpDesk.Core.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Storage
{
    public class EfTransaction : ITransaction
    {
        private readonly IDbContextTransaction _transaction;
        private bool _disposed;
        private bool _committed;

        public Guid TransactionId => throw new NotImplementedException();

        public EfTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
            _committed = false;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EfTransaction));

            if (_committed)
                throw new InvalidOperationException("Transaction already committed");

            await _transaction.CommitAsync(cancellationToken);
            _committed = true;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EfTransaction));

            if (_committed)
                throw new InvalidOperationException("Cannot rollback committed transaction");

            await _transaction.RollbackAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (!_committed)
                {
                    await _transaction.RollbackAsync();
                }
                await _transaction.DisposeAsync();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (!_committed)
                {
                    _transaction.Rollback();
                }
                _transaction.Dispose();
                _disposed = true;
            }
        }
    }
}
