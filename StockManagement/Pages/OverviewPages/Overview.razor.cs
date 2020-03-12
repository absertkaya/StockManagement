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

        protected IList<Category> _categories;

        protected override void OnInitialized()
        {
             _categories = Repository.GetAll<Category>();
        }
    }
}
