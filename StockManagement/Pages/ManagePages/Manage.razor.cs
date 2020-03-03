﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class ManageBase : ComponentBase
    {
        protected IList<Product> _products;
        protected IList<Category> _categories;

        [Inject] 
        public IItemRepository Repository { get; set; }
        [Inject] 
        public NavigationManager NavigationManager { get; set; }

        protected bool _loadFail;
        protected bool _deleteFailProduct;
        protected bool _deleteFailCategory;

        protected bool _showDialog;

        [Inject]
        public IBlobService BlobService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                _categories = await Repository.GetAll<Category>();
                _products = await Repository.GetAll<Product>();
                foreach (Product prod in _products)
                {
                    prod.AmountInStock = await Repository.GetAmountInStockValue(prod.Id);
                }
            } catch (Exception e)
            {
                _loadFail = true;
            }

        }

        protected int GetAmountOfProducts(Category cat)
        {
            return _products.Where(p => p.Category == cat).Count();
        }

        protected void AddProduct()
        {
            NavigationManager.NavigateTo("/productscanner");
        }

        protected void AddCategory()
        {
            NavigationManager.NavigateTo("/categorie");
        }

        protected void AddItem(int id)
        {
            NavigationManager.NavigateTo("/scanner/in");
        }

        protected void RemoveItem(int id)
        {
            NavigationManager.NavigateTo("/scanner/out");
        }

        protected void EditProduct(int id)
        {
            NavigationManager.NavigateTo("/productform/id/" + id);
        }

        protected void EditCategory(int id)
        {
            NavigationManager.NavigateTo("/categorie/" + id);
        }

        protected async Task DeleteProduct(int id)
        {
            Product product = _products.FirstOrDefault(p => p.Id == id);
            try
            {
                await DeleteItemBlobs(product.Items);
            }
            catch (Exception ex)
            {
                //Log
            }

            try
            {
                Repository.Delete(product);
                _products.Remove(product);
            }
            catch (Exception ex)
            {
                _deleteFailProduct = true;
            }

        }


        protected async Task DeleteCategory(int id)
        {
            try
            {
                _products.Where(p => p.Category.Id == id).ToList().ForEach(async p =>
                {
                    await DeleteItemBlobs(p.Items);
                });
            } catch (Exception ex)
            {
                //Log
            }

            try
            {
                await BlobService.SetContainer("categories");
                await BlobService.DeleteBlob("category" + id);
                Repository.Delete(_categories.FirstOrDefault(p => p.Id == id));
                _categories.Remove(_categories.FirstOrDefault(p => p.Id == id));
                _products = _products.Where(p => p.Category.Id != id).ToList();
            }
            catch (Exception ex)
            {
                _deleteFailCategory = true;
            }

        }

        private async Task DeleteItemBlobs(IList<Item> items)
        {

            foreach (Item item in items)
            {
                await BlobService.SetContainer("item"+item.Id);
                await BlobService.DeleteContainer();
                
            }
        }

        protected void GetItems(int id)
        {
            NavigationManager.NavigateTo("/itemlijst/" + id);
        }
    }
}
