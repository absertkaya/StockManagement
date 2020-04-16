using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StockManagement.Domain
{
    public class Product
    {
        private string _productNumber;

        public virtual int Id { get; set; }
        [Required(ErrorMessage = "Productnaam is verplicht")]
        public virtual string Description { get; set; }
        [Required(ErrorMessage = "Categorie is verplicht")]
        public virtual Category Category { get; set; }
        [Required(ErrorMessage = "Productnummer is verplicht")]
        public virtual string ProductNumber { 
            get {

                    return _productNumber;

            } 
            set { _productNumber = value; } }
        public virtual IList<Item> Items { get; set; }
        public virtual int AmountInStock { get; set; }

        public Product()
        {
            Items = new List<Item>();
        }

        public virtual bool HasProductNumber()
        {
            return !ProductNumber.StartsWith("NOPRODUCTNR");
        }

        public virtual bool HasItem(string serialNumber)
        {
            return Items.Any(i => i.SerialNumber == serialNumber);
        }

        public virtual void AddItem(Item item)
        {
            if (!HasItem(item.SerialNumber))
            {
                Items.Add(item);
            }
        }
    }
}
