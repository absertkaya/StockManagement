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

                List<Category> cats = new List<Category> { 
                    new Category { CategoryName = "Categorie A" },
                    new Category { CategoryName = "Categorie B" },
                    new Category { CategoryName = "Categorie C" }
                };
                cats.ForEach(c => repo.Save(c));
                List<Supplier> sups = new List<Supplier> { 
                    new Supplier { SupplierName = "Leverancier A" },
                    new Supplier { SupplierName = "Leverancier B" },
                    new Supplier { SupplierName = "Leverancier C" } 
                };
                sups.ForEach(s => repo.Save(s));

                List<Product> descs = new List<Product>();
                for (int i = 0; i < 10; i++)
                {
                    
                    if (i < 5)
                    {
                        Product desc = new Product() { 
                            ProductNumber = "00"+i,
                            Category = cats[i % cats.Count], 
                            Supplier =  sups[i%sups.Count], 
                            Description = "Product " + i
                        };
                        descs.Add(desc);
                        repo.Save(desc);
                    }

                    Item item = new Item {
                        SerialNumber = "00" + i, Comment = "Comment " + i,
                        Product = descs[i % descs.Count],
                        InStock = true
                    };
                    ADUser user = new ADUser { FirstName = "FirstName " + i, LastName = "LastName" };
                    repo.Save(user);
                    repo.Save(item);
                }
            }
        }
    }
}
