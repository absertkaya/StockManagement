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
        public RepositoryBase()
        {
            _session = Database.OpenSession();
        }
        public RepositoryBase(ISession session)
        {
            _session = session;
        }
        #region Transaction and Session Management Methods
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
            _session.Flush();
        }
        public virtual void Delete(object obj)
        {
            _session.Delete(obj);
           // _session.Flush();
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

            if (_session != null)
            {
                _session.Flush();
                CloseSession();
            }
        }

    }
}
