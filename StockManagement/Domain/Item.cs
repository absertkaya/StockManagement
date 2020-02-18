using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class Item
    {
        public virtual int Id { get; set; }

        public virtual string Description { get; set; }
        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ADUser ADUser { get; set; }
        public virtual string Comment { get; set; }
        public virtual bool IsDefective { get; set; }
        public virtual string SerialNumber { get; set; }
        public virtual string ProductNumber { get; set; }
        public virtual int AmountInStock { get; set; }
    }
}
