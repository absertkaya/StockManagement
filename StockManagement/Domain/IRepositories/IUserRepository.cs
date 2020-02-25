namespace StockManagement.Domain.IRepositories
{
    interface IUserRepository : IRepository
    {
        ADUser GetByEmail(string email);
    }
}
