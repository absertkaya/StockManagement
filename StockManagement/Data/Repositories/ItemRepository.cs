using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
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

        public virtual async Task<IList<Product>> GetByCategoryAsync(int id)
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
            return item.ItemStatus == ItemStatus.INSTOCK;
        }

        public virtual async Task<int> GetAmountInStockValueAsync(int productId)
        {
            return await _session.QueryOver<Item>().Where(i => i.Product.Id == productId && i.ItemStatus == ItemStatus.INSTOCK).RowCountAsync();
        }

        public virtual async Task<IList<Item>> GetByProductAsync(int productid)
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
                .Add(Restrictions.Eq("User.Id", id))
                .AddOrder(Order.Desc("ToDate"));
            
            return await crit.ListAsync<ItemUser>();
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByItem(int id)
        {
            var sqlFunction = new SQLFunctionTemplate(NHibernateUtil.String
                                         , "COALESCE(ToDate, '03/19/2020')");
            var projection = Projections.SqlFunction(sqlFunction, NHibernateUtil.String);
            var crit = _session.CreateCriteria<ItemUser>()
                .Add(Restrictions.Eq("Item.Id", id))
                .AddOrder(Order.Desc(projection)); 
            return await crit.ListAsync<ItemUser>();
        }

        public virtual IList<Product> GetByCategory(int id)
        {
            return _session.Query<Product>().Where(p => p.Category.Id == id).OrderBy(p => p.Description).ToList();
        }

        public virtual bool ItemDuplicateExists(int id, string sn, int productId)
        {
            return _session.Query<Item>().Any(i => i.Id != id && i.SerialNumber == sn && i.Product.Id == productId);
        }

        public IList<Item> GetByProduct(int id)
        {
            var crit = _session.CreateCriteria<Item>().Add(Restrictions.Eq("Product.Id", id));
            return crit.List<Item>();
        }

        public int GetAmountInStockValue(int id)
        {
            return _session.QueryOver<Item>().Where(i => i.Product.Id == id && i.ItemStatus == ItemStatus.INSTOCK).RowCount();
        }

        public bool ProductDuplicateExists(int id, string pn)
        {
            return _session.Query<Product>().Any(p => p.Id != id && p.ProductNumber == pn);
        }

        public Product GetByProductNr(string productnr)
        {
            return _session.Query<Product>().FirstOrDefault(p => p.ProductNumber == productnr);
        }

        public Product GetByProductName(string name)
        {
            return _session.Query<Product>().FirstOrDefault(p => p.Description == name);
        }
    }
}
