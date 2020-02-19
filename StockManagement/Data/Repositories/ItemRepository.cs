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

        public virtual async Task<IList<Product>> GetByCategory(int id)
        {
            var query = _session.CreateCriteria<Product>().Add(Restrictions.Eq("Category.Id", id));
            return await query.ListAsync<Product>();
        }

        public virtual async Task<object[]> GetAmountInStock(int productId)
        {
            var crit = _session.CreateCriteria<Item>()
                .Add(Restrictions.Eq("Product.Id", productId))
                .Add(Restrictions.Eq("InStock",true));
            IList<Item> items = await crit.ListAsync<Item>();
            return new object[] { items[0].Product.Description, items.Count };
        }

        public virtual Item GetBySerialNr(string serialnr)
        {
            return _session.Query<Item>()
                .FirstOrDefault(i => i.SerialNumber == serialnr);
        }

        public virtual bool GetItemInStock(string serialnr)
        {
            Item item = _session.Query<Item>()
                .FirstOrDefault(i => i.SerialNumber == serialnr);
            if (item == null)
            {
                throw new ArgumentException("Item doesn't exist");
            }
            return item.InStock;
        }

        public virtual async Task<int> GetAmountInStockValue(int productId)
        {
            var crit = _session.CreateCriteria<Item>()
                 .Add(Restrictions.Eq("Product.Id", productId))
                 .Add(Restrictions.Eq("InStock", true));
            return (await crit.ListAsync<Item>()).Count;
        }
    }
}
