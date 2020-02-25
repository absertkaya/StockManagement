using NHibernate;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Data.Repositories
{
    public class RepositoryBase : IRepository, IDisposable
    {
        protected ISession _session = null;
        protected ITransaction _transaction = null;
        public RepositoryBase()
        {
            _session = Database.OpenSession();
        }
        public RepositoryBase(ISession session)
        {
            _session = session;
        }
        #region Transaction and Session Management Methods
        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }
        public void CommitTransaction()
        {
            _transaction.Commit();
            CloseTransaction();
        }
        public void RollbackTransaction()
        {
            _transaction.Rollback();
            CloseTransaction();
            CloseSession();
        }
        private void CloseTransaction()
        {
            _transaction.Dispose();
            _transaction = null;
        }
        private void CloseSession()
        {
            _session.Close();
            _session.Dispose();
            _session = null;
        }
        #endregion
        #region IRepository Members
        public virtual void Save(object obj)
        {
            _session.SaveOrUpdate(obj);
        }
        public virtual void Delete(object obj)
        {
            _session.Delete(obj);
        }
        public virtual async Task<object> GetById(Type objType, object objId)
        {
            return await _session.GetAsync(objType, objId);
        }
        public virtual async Task<IList<TEntity>> GetAll<TEntity>() where TEntity : class
        {
            var criteria = _session.CreateCriteria<TEntity>();
            return await criteria.ListAsync<TEntity>();
        }
        #endregion
        public void Dispose()
        {
            if (_transaction != null)
            {

                CommitTransaction();
            }
            if (_session != null)
            {
                _session.Flush();
                CloseSession();
            }
        }

    }
}
