using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Domain.IRepositories
{
    interface IUserRepository : IRepository
    {
        ADUser GetByEmail(string email);
        Task<ADUser> GetUserDetailsAsync(string id);
        Task<IList<MobileAccount>> GetAllMobileAccounts();
    }
}
