using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class Product
    {
        public virtual int Id { get; set; }
        public virtual string Description { get; set; }
        public virtual Category Category { get; set; }
        public virtual string ProductNumber { get; set; }
        public virtual int AmountInStock { get; set; }
        public virtual IList<Item> Items { get; set; }
    }
}
