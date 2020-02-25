using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Domain.IRepositories
{
    public interface IItemRepository : IRepository
    {
        Task<IList<Product>> GetByCategory(int id);
        Task<IList<Item>> GetByProduct(int id);
        Item GetBySerialNr(string serialnr);
        Task<object[]> GetAmountInStock(int id);
        bool GetItemInStock(string serialnr);
        Task<int> GetAmountInStockValue(int id);

        Task<IList<Item>> GetItemsByUser(string id);
        Task<ItemUser> GetLastUse(string userid, int itemid);
        Task<IList<ItemUser>> GetItemUsersByUser(string id);
        Task<IList<ItemUser>> GetItemUsersByItem(int id);
    }
}
