using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository() : base()
        {
        }

        public bool ADUserExists(string id)
        {
            return false;
        }

        public async Task<IList<MobileAccount>> GetAllMobileAccounts()
        {
            return null;
        }

        public ADUser GetByEmail(string email)
        {
            return null;
        }

        public async Task<ADUser> GetUserDetailsAsync(string id)
        {
            return null;
        }

        public async Task<IList<ADUser>> GetUsersWithItems()
        {
            return null;
        }
    }
}
