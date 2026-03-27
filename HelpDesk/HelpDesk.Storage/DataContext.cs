using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Storage
{
    public class DataContext : DbContext, IUnitOfWork
    {
        private ITransaction? _currentTransaction;

        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("A transaction is already in progress");

            var transaction = await Database.BeginTransactionAsync(cancellationToken);
            _currentTransaction = new EfTransaction(transaction);

            return _currentTransaction;
        }
    }
}
