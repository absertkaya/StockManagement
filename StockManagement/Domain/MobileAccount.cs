using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class MobileAccount
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string AccountNumber { get; set; }
        [Required]
        public virtual string AccountName { get; set; }

        public virtual IList<MobileSubscription> MobileSubscriptions { get; set; }
    }
}
