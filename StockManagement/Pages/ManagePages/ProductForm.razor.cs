﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class ProductFormBase : ComponentBase
    {
        [Inject] public IItemRepository Repository { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string ProductNr { get; set; }

        [Parameter]
        public int? Id { get; set; }

        protected IList<Category> _categories;
        protected Product _product = new Product();
        protected EditContext _editContext;

        protected int? _selectedCategory;
        protected string _description;
        protected string _productnr;

        protected bool _submitFail;

        protected override void OnInitialized()
        {
            _productnr = ProductNr;
            _editContext = new EditContext(_product);
        }

        protected override async Task OnInitializedAsync()
        {
            _categories = await Repository.GetAll<Category>();
            if (Id != null)
            {
                _product = (Product)await Repository.GetById(typeof(Product), Id);
                _description = _product.Description;
                _productnr = _product.ProductNumber;
                _selectedCategory = _product.Category.Id;
            }
        }

        protected void SelectCategory(ChangeEventArgs e)
        {
            int id = int.Parse(e.Value.ToString());
            _product.Category = _categories.FirstOrDefault(c => c.Id == id);
        }

        protected void Submit()
        {
            if (_editContext.Validate())
            {
                try
                {
                    Repository.Save(_product);
                    NavigationManager.NavigateTo("/beheer", true);
                }
                catch (Exception ex)
                {
                    _submitFail = true;
                }
            }

        }

        protected void Back()
        {
            NavigationManager.NavigateTo("/beheer");
        }
    }
}