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

        [Required(ErrorMessage = "Abonnement heeft verplicht een GSM nummer.")]
        public virtual string MobileNumber { get; set; }
        [Required(ErrorMessage = "Abonnement heeft verplicht een type.")]
        public virtual string SubscriptionType { get; set; }
        [Required(ErrorMessage = "Abonnement heeft verplicht een gebruiker.")]
        public virtual ADUser User { get; set; }
        [Required(ErrorMessage = "Abonnement heeft verplicht een mobile account.")]
        public virtual MobileAccount MobileAccount { get; set; }

        public MobileSubscription()
        {

        }

    }
}
