using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Domain
{
    public class Category
    {
        public virtual int Id { get; set; }
        [Required(ErrorMessage = "Categorienaam is verplicht")]
        public virtual string CategoryName { get; set; }
        public virtual IList<Product> Products { get; set; }
        public virtual bool IsVisible { get; set; }

        public Category()
        {
            Products = new List<Product>();
            IsVisible = true;
        }

        public virtual void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }
    }
}
