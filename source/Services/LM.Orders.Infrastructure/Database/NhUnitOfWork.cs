using NHibernate;
using LM.Orders.Domain.Interfaces;

namespace LM.Orders.Infrastructure.Database
{
    public class NhUnitOfWork(ISession session) : IUnitOfWork
    {
        private readonly ISession _session = session;
        private ITransaction? _transaction; 

        public void BeginTransaction()
        {
            if (!_session.IsConnected || !_session.IsOpen)
            {
                _session.FlushMode = FlushMode.Commit;
            }
            if (_transaction == null || !_transaction.IsActive)
            {
                _transaction = _session.BeginTransaction();
            }
        }

        public async Task CommitAsync()
        {
            if (_transaction != null && _transaction.IsActive)
            {
                await _transaction.CommitAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null && _transaction.IsActive)
            {
                await _transaction.RollbackAsync();
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}