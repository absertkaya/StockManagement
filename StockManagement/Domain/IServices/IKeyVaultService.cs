using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain.IServices
{
    public interface IKeyVaultService
    {
        string GetSecret(string key);
        Task<string> GetSecretAsync(string key);
    }
}
