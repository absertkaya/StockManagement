using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
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

        protected bool _dBError;

        protected IList<Category> _categories;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _categories = await Repository.GetAll<Category>();
            } catch (Exception ex)
            {
                _dBError = true;
            }
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
