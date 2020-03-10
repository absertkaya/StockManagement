using NHibernate.Criterion;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace StockManagement.Data.Repositories
{
    public class ItemRepository : RepositoryBase, IItemRepository
    {

        public virtual async Task<IList<Product>> GetByCategory(int id)
        {
            var query = _session.CreateCriteria<Product>()
                .Add(Restrictions.Eq("Category.Id", id))
                .AddOrder(Order.Asc("Description"));
            return await query.ListAsync<Product>();
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
            return await _session.QueryOver<Item>().Where(i => i.Product.Id == productId && i.InStock).RowCountAsync();
        }

        public virtual async Task<IList<Item>> GetByProduct(int productid)
        {
            var crit = _session.CreateCriteria<Item>().Add(Restrictions.Eq("Product.Id", productid));
            return await crit.ListAsync<Item>();
        }

        public virtual async Task<ItemUser> GetLastUse(string userid, int itemid)
        {
            var crit = _session.CreateCriteria<ItemUser>()
                .Add(Restrictions.Eq("User.Id", userid))
                .Add(Restrictions.Eq("Item.Id", itemid))
                .Add(Restrictions.Eq("ToDate", null));
            return await crit.UniqueResultAsync<ItemUser>();
        }

        public virtual async Task<IList<Item>> GetItemsByUser(string id)
        {
            var crit = _session.CreateCriteria<Item>()
                .Add(Restrictions.Eq("ADUser.Id", id));
            return await crit.ListAsync<Item>();
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByUser(string id)
        {
            var crit = _session.CreateCriteria<ItemUser>()
                .Add(Restrictions.Eq("User.Id", id));
            return await crit.ListAsync<ItemUser>();
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByItem(int id)
        {
            var crit = _session.CreateCriteria<ItemUser>()
                .Add(Restrictions.Eq("Item.Id", id));
            return await crit.ListAsync<ItemUser>();
        }

        public virtual bool ItemDuplicateExists(int id, string sn, int productId)
        {
            return _session.Query<Item>().Any(i => i.Id != id && i.SerialNumber == sn && i.Product.Id == productId);
        }
    }
}
