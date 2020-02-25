using System.Collections.Generic;

namespace StockManagement.Domain
{
    public class Category
    {
        public virtual int Id { get; set; }
        public virtual string CategoryName { get; set; }
        public virtual IList<Product> Products { get; set; }
        public virtual string ImgURL { get; set; }
    }
}
