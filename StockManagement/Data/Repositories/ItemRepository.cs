﻿using NHibernate;
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
            return await _session.Query<Product>().Where(p => p.Category.Id == id).OrderBy(p => p.Description).ToListAsync();
        }

        public virtual async Task<IList<Item>> GetBySerialNrAsync(string serialnr)
        {
            return await _session.QueryOver<Item>()
                .Where(i => i.SerialNumber.IsInsensitiveLike(serialnr, MatchMode.Anywhere) || i.Hostname.IsInsensitiveLike(serialnr, MatchMode.Anywhere))
                .ListAsync();
        }

        public virtual async Task<Item> GetItemByProductAndSerialNumberAsync(string pn, string sn)
        {
            return await _session.Query<Item>().FirstOrDefaultAsync(i => i.Product.ProductNumber == pn && i.SerialNumber == sn);
        }

        public virtual async Task<Item> GetItemDetails(int id)
        {
            return await _session.QueryOver<Item>()
                .Fetch(SelectMode.Fetch, x => x.ADUser)
                .Fetch(SelectMode.Fetch, x => x.Product)
                .Fetch(SelectMode.Fetch, x => x.Supplier)
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();
        }

        public virtual bool GetItemInStock(int id)
        {
            Item item = _session.Query<Item>()
                .FirstOrDefault(i => i.Id == id);
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
            return await _session.QueryOver<Item>().Where(i => i.Product.Id == productid)
                .Left.JoinQueryOver(i => i.ADUser)
                .ListAsync();
        }

        public virtual async Task<ItemUser> GetLastUse(string userid, int itemid)
        {
            return await _session.Query<ItemUser>()
                .FirstOrDefaultAsync(i => i.User.Id == userid && i.Item.Id == itemid && i.ToDate == null);
        }

        public virtual async Task<IList<Item>> GetItemsByUserAsync(string id)
        {
            return await _session.Query<Item>()
                .Where(i => i.ADUser.Id == id).Fetch(i => i.ADUser)
                .Fetch(i => i.Product)
                .ToListAsync();
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByUser(string id)
        {
            return await _session.Query<ItemUser>()
                .Where(i => i.User.Id == id)
                .Fetch(i => i.User).Fetch(i => i.Item)
                .ThenFetch(i => i.Product)
                .OrderByDescending(i => i.ToDate)
                .ToListAsync();
        }

        public virtual async Task<IList<ItemUser>> GetItemUsersByItem(int id)
        {
            return await _session.Query<ItemUser>()
                .Where(i => i.Item.Id == id)
                .Fetch(i => i.User)
                .Fetch(i => i.Item)
                .ThenFetch(i => i.Product)
                .OrderByDescending(i => i.ToDate)
                .ToListAsync();
        }

        public virtual IList<Product> GetByCategory(int id)
        {
            return _session.Query<Product>()
                .Where(p => p.Category.Id == id)
                .OrderBy(p => p.Description)
                .ToList();
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
                .Where(i => i.Supplier.Id == id)
                .Left.JoinQueryOver(i => i.ADUser)
                .ListAsync();
        }

        public bool ItemExists(int id)
        {
            return _session.Query<Item>().FirstOrDefault(i => i.Id == id) != null;     
        }
    }
}
