using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain.IRepositories
{
    public interface IItemRepository : IRepository
    {
        Task<IList<Item>> GetByCategory(int id);
    }
}
