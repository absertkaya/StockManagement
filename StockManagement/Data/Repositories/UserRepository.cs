﻿using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System.Linq;

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
