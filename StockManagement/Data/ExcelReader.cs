using Blazor.FileReader;
using ExcelDataReader;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Win32.SafeHandles;
using StockManagement.Data.Repositories;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Data
{
    public class ExcelReader
    {
        public IItemRepository Repo { get; set; } = new ItemRepository();

        public void ReadAndPopulateDatabase(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                ReaderHelper(stream);
            }
        }

        public void ReadAndPopulateDatabase(Stream stream)
        {
            ReaderHelper(stream);
        }

        private void ReaderHelper(Stream stream)
        {
            int rowNr;
            Supplier unknown = new Supplier() { SupplierName = "UNKNOWN" };
            Supplier supplier = unknown;
            List<string> products = new List<string>();
            List<Supplier> suppliers = new List<Supplier>() { unknown};
            Repo.Save(supplier);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                List<Product> _existingProduct = new List<Product>();
                foreach (DataTable table in result.Tables)
                {
                    Category cat = new Category() { CategoryName = table.ToString() };
                    Repo.Save(cat);
                    rowNr = 0;
                    foreach (DataRow row in table.Rows)
                    {
                        Product product = null;
                        rowNr++;
                        if (rowNr == 1)
                        {
                            continue;
                        }
                        string pn = row.ItemArray[2].ToString();
                        if (string.IsNullOrWhiteSpace(pn))
                        {
                            pn = table.ToString() + " ROWNUMBER: " + rowNr;
                        }
                        string sn = row.ItemArray[3].ToString();
                        if (string.IsNullOrWhiteSpace(sn))
                        {
                            sn = table.ToString() + " ROWNUMBER: " + rowNr;
                        }
                        DateTime? delivery;
                        try
                        {
                            delivery = DateTime.Parse(row.ItemArray[4].ToString());
                        } catch (Exception exc)
                        {
                            delivery = null;
                        }
                        
                        if (delivery == null)
                        {
                            delivery = DateTime.Today;
                        }
                        string supplierName = row.ItemArray[5].ToString().Trim().ToUpper();
                        if (string.IsNullOrWhiteSpace(supplierName))
                        {
                            supplier = unknown;
                        } else if (suppliers.Any(s => s.SupplierName == supplierName))
                        {
                            supplier = suppliers.First(s => s.SupplierName == supplierName);
                        } else
                        {
                            supplier = new Supplier() { SupplierName = supplierName };
                            Repo.Save(supplier);
                            suppliers.Add(supplier);
                        }
                        DateTime? invoice;
                        try
                        {
                            invoice = DateTime.Parse(row.ItemArray[6].ToString());
                        } catch (Exception exc)
                        {
                            invoice = null;
                        }
                        if (invoice == null)
                        {
                            invoice = DateTime.Today;
                        }
                        string comment = row.ItemArray[10].ToString();

                        string desc = row.ItemArray[0].ToString() + " | " + row.ItemArray[1].ToString();
                        string ex = Regex.Replace(desc.Trim().ToLower(), @"\s+", "");
                        if (!Repo.ProductDuplicateExists(0, pn) && !products.Contains(ex))
                        {
                            product = new Product()
                            {
                                Category = cat,
                                Description = desc,
                                ProductNumber = pn
                            };
                            Repo.Save(product);
                            products.Add(Regex.Replace(desc.Trim().ToLower(), @"\s+", ""));
                        }

                        if (product == null)
                        {
                            product = Repo.GetByProductNr(pn);
                            if (product == null)
                            {
                                product = Repo.GetByProductName(desc);
                            }
                        }
                        
                        DateTime? outstockdate;
                        try
                        {
                            outstockdate = DateTime.Parse(row.ItemArray[9].ToString());
                        }
                        catch (Exception exc)
                        {
                            outstockdate = null;
                        }
                        comment += "User: " + row.ItemArray[8].ToString();
                        bool instock = outstockdate == null;
                        Item item = new Item()
                        {
                            Product = product,
                            SerialNumber = sn,
                            Supplier = supplier,
                            DeliveryDate = delivery == null ? DateTime.Now : (DateTime)delivery,
                            InvoiceDate = invoice == null ? DateTime.Now : (DateTime)invoice,
                            Comment = comment,
                            InStock = instock
                        };
                        if (!Repo.ItemDuplicateExists(item.Id, sn, product.Id))
                        {
                            try
                            {
                                Repo.Save(item);
                            }
                            catch (Exception exc)
                            {

                            }
                        }
                    }
                }
            }
        }


    }
}
