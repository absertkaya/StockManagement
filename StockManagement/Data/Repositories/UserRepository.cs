using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;

namespace StockManagement.Data.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public ADUser GetByEmail(string email)
        {
            return _session.Query<ADUser>().FirstOrDefault(u => u.Mail == email);
        }
    }
}
