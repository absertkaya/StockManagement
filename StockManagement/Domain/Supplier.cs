using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Domain
{
    public class Supplier
    {
        public virtual int Id { get; set; }
        [Required(ErrorMessage = "Leveranciernaam is verplicht.")]
        public virtual string SupplierName { get; set; }
        public virtual IList<Item> Items { get; set; }
    }
}
