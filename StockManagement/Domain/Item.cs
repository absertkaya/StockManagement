using StockManagement.Graph;
using System.Collections.Generic;

namespace StockManagement.Domain
{
    public class Item
    {
        public virtual int Id { get; set; }
        public virtual Product Product { get; set; }
        public virtual ADUser ADUser { get; set; }
        public virtual string Comment { get; set; }
        public virtual bool IsDefective { get; set; }
        public virtual string SerialNumber { get; set; }
        public virtual bool InStock { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual IList<ItemUser> ItemUsers { get; set; }
    }
}
