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

            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                List<string> _existing = new List<string>();
                foreach (DataTable table in result.Tables)
                {
                    Category cat = new Category() { CategoryName = table.ToString() };
                    Repo.Save(cat);
                    rowNr = 0;
                    foreach (DataRow row in table.Rows)
                    {
                        rowNr++;
                        if (rowNr == 1)
                        {
                            continue;
                        }
                        string pn = row.ItemArray[2].ToString();
                        string desc = row.ItemArray[0].ToString() + " | " + row.ItemArray[1].ToString();
                        string ex = Regex.Replace(desc.ToLower(), @"\s+", "");
                        if (!_existing.Contains(ex) && !_existing.Contains(pn))
                        {
                            Product product = new Product()
                            {
                                Category = cat,
                                Description = desc,
                                ProductNumber = pn
                            };
                            Repo.Save(product);
                            _existing.Add(ex);
                            _existing.Add(pn);
                        }

                    }
                }
            }
        }


    }
}
