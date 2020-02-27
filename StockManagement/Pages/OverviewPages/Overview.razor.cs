using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;

namespace StockManagement.Pages.OverviewPages
{
    public class OverviewBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected IList<Category> _categories;

        protected override async Task OnInitializedAsync()
        {
            _categories = await Repository.GetAll<Category>();
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
