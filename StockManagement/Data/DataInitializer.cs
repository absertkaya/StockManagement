using NHibernate;
using StockManagement.Data.Repositories;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Data
{
    public class DataInitializer
    {
        public void Initialize()
        {

            using (RepositoryBase repo = new RepositoryBase())
            {

                List<Category> cats = new List<Category> { new Category { CategoryName = "Categorie A" }, new Category { CategoryName = "Categorie B" }, new Category { CategoryName = "Categorie C" } };
                Supplier s1 = new Supplier { SupplierName = "Leverancier A" };
                Supplier s2 = new Supplier { SupplierName = "Leverancier B" };
                Supplier s3 = new Supplier { SupplierName = "Leverancier C" };
                cats.ForEach(c => repo.Save(c));

                for (int i = 0; i < 10; i++)
                {
                    Item item = new Item { Description = "item " + i };
                    ADUser user = new ADUser { FirstName = "FirstName " + i, LastName = "LastName" };
                    item.Category = cats[i % 3];
                    repo.Save(user);
                    repo.Save(item);
                }
                repo.Save(s1);
                repo.Save(s2);
                repo.Save(s3);
            }
        }
    }
}
