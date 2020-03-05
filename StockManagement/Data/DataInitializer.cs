using StockManagement.Data.Repositories;
using StockManagement.Domain;
using System;
using System.Collections.Generic;

namespace StockManagement.Data
{
    public class DataInitializer
    {
        public void Initialize()
        {

            using (RepositoryBase repo = new RepositoryBase())
            {
                List<Category> cats = new List<Category> {
                    new Category { CategoryName = "Monitors" },
                    new Category { CategoryName = "Laptops" },
                    new Category { CategoryName = "IPads" },
                    new Category { CategoryName = "Switches" }
                };
                cats.ForEach(c => repo.Save(c));
                List<Supplier> sups = new List<Supplier> {
                    new Supplier { SupplierName = "bol.com" },
                    new Supplier { SupplierName = "coolblue" },
                    new Supplier { SupplierName = "Megekko" }
                };
                sups.ForEach(s => repo.Save(s));

                Random rand = new Random();
                List<Product> descs = new List<Product>();
                for (int i = 0; i < 30; i++)
                {

                    if (i < 5)
                    {
                        Product desc = new Product()
                        {
                            ProductNumber = "00" + i,
                            Category = cats[i % cats.Count],
                            Description = "Product " + i
                        };
                        descs.Add(desc);
                        repo.Save(desc);
                    }

                    Item item = new Item
                    {
                        SerialNumber = "00" + i,
                        Comment = "Comment " + i,
                        Product = descs[rand.Next(i) % descs.Count],
                        Supplier = sups[i % sups.Count],
                        InStock = true
                    };
                    repo.Save(item);
                }
            }
        }
    }
}
