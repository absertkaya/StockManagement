using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.Linq;
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
        public ItemRepository(Database database) : base(database)
        {
        }

        public virtual async Task<IList<Product>> GetByCategoryAsync(int id)
        {
            return await _session.QueryOver<Product>()
                .Where(p => p.Category.Id == id) 
                .OrderBy(p => p.Description).Asc
                .ListAsync();
        }

        public virtual async Task<IList<Item>> GetBySerialNrAsync(string serialnr)
        {
            return await _session.QueryOver<Item>()
                .Where(i => i.SerialNumber.IsInsensitiveLike(serialnr, MatchMode.Anywhere) || i.Hostname.IsInsensitiveLike(serialnr, MatchMode.Anywhere))
                .ListAsync();
        }

        public virtual async Task<Item> GetItemByProductAndSerialNumberAsync(string pn, string sn)
        {
            return await _session.QueryOver<Item>()
                .Where(i => i.Product.ProductNumber == pn && i.SerialNumber == sn)
                .SingleOrDefaultAsync();
        }

        public virtual async Task<Item> GetItemDetails(int id)
        {
            return await _session.QueryOver<Item>()
                
                .Fetch(SelectMode.Fetch, x => x.Product)
                .Fetch(SelectMode.Fetch, x => x.Supplier)
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();
        }

        public virtual bool GetItemInStock(int id)
        {
            Item item = _session.QueryOver<Item>()
                .Where(i => i.Id == id)
                .SingleOrDefault();
            return item?.ItemStatus == ItemStatus.INSTOCK;
        }

        public virtual async Task<int> GetAmountInStockValueAsync(int productId)
        {
            return await _session.QueryOver<Item>()
                .Where(i => i.Product.Id == productId && i.ItemStatus == ItemStatus.INSTOCK)
                .RowCountAsync();
        }

        public virtual async Task<IList<Item>> GetByProductAsync(int productid)
        {
            return await _session.QueryOver<Item>()
                
                .Where(i => i.Product.Id == productid)
                .ListAsync();
        }

        public virtual async Task<ItemUser> GetLastUse(string userid, int itemid)
        {
            return await _session.Query<ItemUser>()
                .FirstOrDefaultAsync(i => i.User.Id == userid && i.Item.Id == itemid && i.ToDate == null);
        }

        public virtual async Task<IList<Item>> GetItemsByUserAsync(string id)
        {
            return await _session.QueryOver<Item>()
                .Fetch(SelectMode.Fetch, i => i.Product)
                
                .Where(i => i.ADUser.Id == id)
                .ListAsync();
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByUser(string id)
        {
            return await _session.QueryOver<ItemUser>()
                
                .Fetch(SelectMode.Fetch, i => i.Item)
                .Where(i => i.User.Id == id)
                .OrderBy(i => i.ToDate).Desc
                .ListAsync();
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByItem(int id)
        {
            return await _session.QueryOver<ItemUser>()
                
                .Fetch(SelectMode.Fetch, i => i.Item)
                .Where(i => i.Item.Id == id)
                .OrderBy(i => i.ToDate).Desc
                .ListAsync();
        }

        public virtual IList<Product> GetByCategory(int id)
        {
            return _session.QueryOver<Product>()
                .Where(p => p.Category.Id == id)
                .OrderBy(p => p.Description).Desc
                .List();
        }

        public virtual bool ItemDuplicateExists(int id, string sn, int productId)
        {
            return _session.Query<Item>()
                .Any(i => i.Id != id && i.SerialNumber == sn && i.Product.Id == productId);
        }

        public IList<Item> GetByProduct(int id)
        {
            return _session.Query<Item>()
                .Where(i => i.Product.Id == id)
                .ToList();
        }

        public int GetAmountInStockValue(int id)
        {
            return _session.QueryOver<Item>()
                .Where(i => i.Product.Id == id && i.ItemStatus == ItemStatus.INSTOCK)
                .RowCount();
        }

        public bool ProductDuplicateExists(int id, string pn)
        {
            return _session.Query<Product>()
                .Any(p => p.Id != id && p.ProductNumber == pn);
        }

        public Product GetByProductNr(string productnr)
        {
            return _session.Query<Product>()
                .FirstOrDefault(p => p.ProductNumber == productnr);
        }

        public Product GetByProductName(string name)
        {
            return _session.Query<Product>()
                .FirstOrDefault(p => p.Description == name);
        }

        public virtual async Task<IList<Item>> GetBySupplierAsync(int id)
        {
            return await _session.QueryOver<Item>()
                .Fetch(SelectMode.Fetch, i => i.Supplier.Id == id)
                .Where(i => i.Supplier.Id == id)

                .ListAsync();
        }

        public bool ItemExists(int id)
        {
            return _session.Query<Item>().FirstOrDefault(i => i.Id == id) != null;     
        }
    }
}
