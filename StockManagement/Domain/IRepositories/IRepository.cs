using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Domain.IRepositories
{
    public interface IRepository
    {
        void Save(object obj);
        void Delete(object obj);
        Task<object> GetByIdAsync(Type objType, object objId);

        object GetById(Type objType, object objId);
        Task<IList<TEntity>> GetAllAsync<TEntity>() where TEntity : class;
        IList<TEntity> GetAll<TEntity>() where TEntity : class;
        void Dispose();
    }
}
