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
        [Required(ErrorMessage = "Account heeft verplicht een accountnummer.")]
        public virtual string AccountNumber { get; set; }
        [Required(ErrorMessage = "Account heeft verplicht een accountnaam.")]
        public virtual string AccountName { get; set; }

        public virtual IList<MobileSubscription> MobileSubscriptions { get; set; }

        public MobileAccount()
        {
            MobileSubscriptions = new List<MobileSubscription>();
        }

        public virtual bool HasSubscriptions()
        {
            return MobileSubscriptions != null && MobileSubscriptions.Count != 0;
        }
    }


}
