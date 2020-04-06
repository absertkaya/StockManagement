using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Domain.IRepositories
{
    public interface IItemRepository : IRepository
    {
        Task<IList<Product>> GetByCategoryAsync(int id);
        IList<Product> GetByCategory(int id);
        Task<Item> GetItemWithUser(int id);
        IList<Item> GetByProduct(int id);
        Task<IList<Item>> GetByProductAsync(int id);
        Item GetBySerialNr(string serialnr);
        Product GetByProductNr(string productnr);
        Product GetByProductName(string name);
        bool GetItemInStock(int id);
        bool ItemExists(int id);
        Task<int> GetAmountInStockValueAsync(int id);
        int GetAmountInStockValue(int id);
        Task<IList<Item>> GetItemsByUser(string id);
        Task<ItemUser> GetLastUse(string userid, int itemid);
        Task<IList<ItemUser>> GetItemUsersByUser(string id);
        Task<IList<ItemUser>> GetItemUsersByItem(int id);
        bool ItemDuplicateExists(int id, string sn, int productId);
        bool ProductDuplicateExists(int id, string pn);
        Task<IList<Item>> GetBySupplierAsync(int id);
    }
}
