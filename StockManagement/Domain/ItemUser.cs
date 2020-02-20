using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class ItemUser
    {
        public virtual int Id { get; set; }
        public virtual Item Item { get; set; }
        public virtual ADUser User { get; set; }
        public virtual ADUser AssignedBy { get; set; }
        public virtual DateTime FromDate { get; set; }
        public virtual DateTime ToDate { get; set; }
    }
}
