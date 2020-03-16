using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class Stock
    {
        public virtual int Id { get; set; }
        public virtual string Location { get; set; }
        public virtual IList<Item> Items { get; set; }

        public Stock()
        {
            Items = new List<Item>();
        }

        public virtual void AddItem(Item item)
        {
            if (! Items.Contains(item))
                Items.Add(item);
        }

        public virtual void ReturnItem(Item item, ADUser returner)
        {
            if (! Items.Contains(item))
            {
                item.ReturnToStock(returner);
                Items.Add(item);
            }
        }

        public virtual void RemoveItem(Item item, ADUser user, ADUser assigner)
        {
            if (Items.Contains(item))
            {
                item.RemoveFromStock(user, assigner);
                Items.Remove(item);
            }
        }
    }
}
