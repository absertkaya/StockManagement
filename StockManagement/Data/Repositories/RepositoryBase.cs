using NHibernate;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _session.Flush();
        }
        public virtual async Task<object> GetByIdAsync(Type objType, object objId)
        {
            return await _session.GetAsync(objType, objId);
        }
        public virtual async Task<IList<TEntity>> GetAllAsync<TEntity>() where TEntity : class
        {
            var criteria = _session.CreateCriteria<TEntity>();
            return await criteria.ListAsync<TEntity>();
        }

        public virtual IList<TEntity> GetAll<TEntity>() where TEntity : class
        {
            var query = _session.Query<TEntity>();
            return query.ToList();
        }
        #endregion
        public void Dispose()
        {
            if (_session != null)
            {
                CloseSession();
            }
        }

        public object GetById(Type objType, object objId)
        {
            return _session.Get(objType, objId);
        }
    }
    
}
