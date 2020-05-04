using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain.IServices
{
    public interface MailService
    {
        void SendMail(string subj, string msg, string to);

    }
}
