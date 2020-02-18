using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class Category
    {
        public virtual int Id { get; set; }
        public virtual string CategoryName { get; set; }
    }
}
