using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class MobileSubscription
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual string MobileNumber { get; set; }
        [Required]
        public virtual string SubscriptionType { get; set; }
        [Required]
        public virtual ADUser User { get; set; }
        [Required]
        public virtual MobileAccount MobileAccount { get; set; }

        public MobileSubscription()
        {

        }

    }
}
