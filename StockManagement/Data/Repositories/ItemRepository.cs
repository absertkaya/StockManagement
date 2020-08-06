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

        public ItemRepository() : base()
        {

        }

        public virtual async Task<IList<Product>> GetByCategoryAsync(int id)
        {
            Category cat = new Category();
            IList<Product> products = new List<Product>();
            for (int i = 0; i < 10000; i++)
            {
                products.Add(new Product()
                {
                    Category = cat,
                    Description = "Product " + i
                });
            }
            return products;
        }

        public virtual async Task<IList<Item>> GetBySerialNrAsync(string serialnr)
        {
            return null;
        }

        public virtual async Task<Item> GetItemByProductAndSerialNumberAsync(string pn, string sn)
        {
            return null;
        }

        public virtual async Task<Item> GetItemDetails(int id)
        {
            return null;
        }

        public virtual bool GetItemInStock(int id)
        {
            return false;
        }

        public virtual async Task<int> GetAmountInStockValueAsync(int productId)
        {
            return 0;
        }

        public virtual async Task<IList<Item>> GetByProductAsync(int productid)
        {
            return null;
        }

        public virtual async Task<ItemUser> GetLastUse(string userid, int itemid)
        {
            return null;
        }

        public virtual async Task<IList<Item>> GetItemsByUserAsync(string id)
        {
            return null;
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByUser(string id)
        {
            return null;
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByItem(int id)
        {
            return null;
        }

        public virtual IList<Product> GetByCategory(int id)
        {
            return null;
        }

        public virtual bool ItemDuplicateExists(int id, string sn, int productId)
        {
            return false;
        }

        public IList<Item> GetByProduct(int id)
        {
            return null;
        }

        public int GetAmountInStockValue(int id)
        {
            return 0;
        }

        public bool ProductDuplicateExists(int id, string pn)
        {
            return false;
        }

        public Product GetByProductNr(string productnr)
        {
            return null;
        }

        public Product GetByProductName(string name)
        {
            return null;
        }

        public virtual async Task<IList<Item>> GetBySupplierAsync(int id)
        {
            return null;
        }

        public bool ItemExists(int id)
        {
            return false;
        }
    }
}
