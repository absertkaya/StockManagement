using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Domain
{
    public class Product
    {
        public virtual int Id { get; set; }
        [Required(ErrorMessage = "Productnaam is verplicht")]
        public virtual string Description { get; set; }
        [Required(ErrorMessage = "Categorie is verplicht")]
        public virtual Category Category { get; set; }
        [Required(ErrorMessage = "Productnummer is verplicht")]
        public virtual string ProductNumber { get; set; }
        public virtual int AmountInStock { get; set; }
        public virtual IList<Item> Items { get; set; }
    }
}
