using Microsoft.WindowsAzure.Storage.Table;
using NHibernate;
using NHibernate.Linq;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Repositories
{
    public class RepositoryBase : IRepository, IDisposable
    {
        public void Delete(object obj)
        {
            
        }

        public void Dispose()
        {
            
        }

        public IList<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return new List<TEntity>();
        }

        public async Task<IList<TEntity>> GetAllAsync<TEntity>() where TEntity : class
        {
            return new List<TEntity>(); ;
        }

        public object GetById(Type objType, object objId)
        {
            return new List<object>();
        }

        public async Task<object> GetByIdAsync(Type objType, object objId)
        {
            return new List<object>();
        }

        public void Save(object obj)
        {
            
        }
    }

}
