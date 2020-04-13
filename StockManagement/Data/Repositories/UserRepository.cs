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
        public UserRepository(Database database) : base(database)
        {
        }

        public async Task<IList<MobileAccount>> GetAllMobileAccounts()
        {
            return await _session.Query<MobileAccount>().ToListAsync();
        }

        public ADUser GetByEmail(string email)
        {
            return _session.Query<ADUser>().FirstOrDefault(u => u.Mail == email);
        }

        public async Task<ADUser> GetUserDetailsAsync(string id)
        {
            return await _session.QueryOver<ADUser>()
                .Fetch(SelectMode.Fetch, x => x.MobileSubscriptions)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

    }
}
