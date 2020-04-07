using Blazor.FileReader;
using ExcelDataReader;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.JSInterop;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;
using StockManagement.Data.Repositories;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Data
{
    public class ExcelReader
    {
        public IItemRepository Repo { get; set; }
        protected List<GraphUser> _colGraphUsers = new List<GraphUser>();
        public IConfiguration Configuration { get; set; }

        private List<ADUser> newUsers = new List<ADUser>();

        private Dictionary<string, GraphUser> userMap = new Dictionary<string, GraphUser>();

        public ExcelReader(IItemRepository repository, IConfiguration config)
        {
            Repo = repository;
            Configuration = config;
        }

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

        public async Task ApiCall(string url)
        {
            try
            {
                IConfidentialClientApplication confidentialClientApplication =
                ConfidentialClientApplicationBuilder
                    .Create(Configuration["AzureAd:ClientId"])
                    .WithTenantId(Configuration["AzureAd:TenantId"])
                    .WithClientSecret(Configuration["AzureAd:ClientSecret"])
                    .Build();
                string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
                AuthenticationResult result = null;
                result = await confidentialClientApplication.AcquireTokenForClient(scopes)
                    .ExecuteAsync();
                var httpClient = new HttpClient();
                var apiCaller = new ProtectedApiCallHelper(httpClient);
                var res = await apiCaller
                    .CallWebApiAndProcessResultASync(
                        url,
                        result.AccessToken
                        );
                DisplayUsers(res);
                if (res.Properties().FirstOrDefault(p => p.Name == "@odata.nextLink") != null)
                {
                    await ApiCall(res.Properties().First(p => p.Name == "@odata.nextLink").Value.ToString());
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void DisplayUsers(JObject result)
        {
            foreach (JProperty child in result.Properties().Where(p => !p.Name.StartsWith("@")))
            {
                _colGraphUsers.AddRange(
                    child.Value.ToObject<List<GraphUser>>()
                    );
            }

            _colGraphUsers = _colGraphUsers
              .Where(u => u.Mail != null && u.GivenName != null && u.Surname != null && u.OfficeLocation != null && u.JobTitle != null)
              .ToList();

        }

        public void ReadUsers(string path)
        {
            int rowNr;
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    rowNr = 0;
                    foreach (DataTable table in result.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            rowNr++;
                            if (rowNr < 4)
                            {
                                continue;
                            }
                            string code = row.ItemArray[0].ToString().ToLower();
                            string email = row.ItemArray[2].ToString().ToLower();
                            GraphUser user = _colGraphUsers.FirstOrDefault(u => u.Mail == email);
                            userMap.Add(code, user);
                        }
                    }
                }
            }
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
                            pn = "NOPRODUCTNR " + cat.CategoryName + " " + rowNr;
                        }
                        string sn = row.ItemArray[3].ToString();
                        if (string.IsNullOrWhiteSpace(sn))
                        {
                            sn = "NOSERIALNR " + cat.CategoryName + " " + rowNr;
                        }
                        DateTime? delivery;
                        try
                        {
                            delivery = DateTime.Parse(row.ItemArray[4].ToString());
                        } catch (Exception exc)
                        {
                            delivery = null;
                        }
                        
                        string supplierName = row.ItemArray[5].ToString().Trim().ToLower();
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

                        string comment = row.ItemArray[10].ToString();
                        bool stolen = comment.ToLower().Trim() == "gestolen";

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

                        string loc = row.ItemArray[7].ToString().ToLower().Trim();
                        bool locStock = true;
                        if (!string.IsNullOrEmpty(loc) && loc != "stock")
                        {
                            locStock = false;
                        }

                        string u = row.ItemArray[8].ToString().ToLower();
                        ADUser aduser = null;
                        if (!string.IsNullOrEmpty(u))
                        {
                            GraphUser user = null;
                            if (userMap.ContainsKey(u))
                            {
                                user = userMap[u];
                            }
                             
                            if (user != null)
                            {
                                if (!newUsers.Any(u => u.Id == user.Id))
                                {
                                    aduser = new ADUser(user);
                                    newUsers.Add(aduser);
                                    Repo.Save(aduser);
                                } else
                                {
                                    aduser = newUsers.First(u => u.Id == user.Id);
                                }
                            } else
                            {
                                comment += " (User: " + u + ")";
                            }
                        }

                        bool instock = outstockdate == null && locStock;
                        ItemStatus status = instock ? ItemStatus.INSTOCK : ItemStatus.OUTSTOCK;
                        if(stolen)
                        {
                            status = ItemStatus.STOLEN;
                        }
                        
                        Item item = new Item()
                        {
                            Product = product,
                            SerialNumber = sn,
                            Supplier = supplier,
                            DeliveryDate = delivery == null ? DateTime.Now : (DateTime)delivery,
                            InvoiceDate = invoice == null ? DateTime.Now : (DateTime)invoice,
                            Comment = comment,
                            ItemStatus = status,
                            ADUser = aduser
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
