using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain;
using NHibernate.Criterion;
namespace StockManagement.Data.Repositories
{
    public class ItemRepository : RepositoryBase, IItemRepository
    {
        public virtual async Task<IList<Item>> GetByCategory(int id)
        {
            var query = _session.CreateCriteria<Item>().Add(Restrictions.Eq("Category.Id", id));
            return await query.ListAsync<Item>();
        }
    }
}
