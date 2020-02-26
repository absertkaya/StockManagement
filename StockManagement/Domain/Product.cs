using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Domain
{
    public class Product
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string Description { get; set; }
        [Required]
        public virtual Category Category { get; set; }
        [Required]
        public virtual string ProductNumber { get; set; }
        public virtual int AmountInStock { get; set; }
        public virtual IList<Item> Items { get; set; }
    }
}
