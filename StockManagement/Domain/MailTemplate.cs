using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class MailTemplate
    {
        public virtual int Id { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
    }
} 
