using Blazor.Extensions.Storage.Interfaces;
using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
using StockManagement.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class OverviewBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected List<string> _uris;

        protected bool _dBError;

        protected IList<Category> _categories;

        protected override void OnInitialized()
        {
             _categories = Repository.GetAll<Category>();
        }

        protected void GetProductList(int id)
        {
            NavigationManager.NavigateTo("/productlijst/" + id);
        }

        protected void GetUsers()
        {
            NavigationManager.NavigateTo("/gebruikers");
        }
    }
}
