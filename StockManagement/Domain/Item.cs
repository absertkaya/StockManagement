using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Domain
{
    public class Item
    {
        public virtual int Id { get; set; }
        [Required(ErrorMessage = "Product is verplicht")]
        public virtual Product Product { get; set; }
        public virtual ADUser ADUser { get; set; }
        public virtual string Comment { get; set; }
        public virtual bool IsDefective { get; set; }
        [Required(ErrorMessage = "Serienummer is verplicht")]
        public virtual string SerialNumber { get; set; }
        public virtual bool InStock { get; set; }
        [Required(ErrorMessage = "Leverancier is verplicht")]
        public virtual Supplier Supplier { get; set; }
        public virtual IList<ItemUser> ItemUsers { get; set; }
    }
}
