using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class ItemHistoryBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IItemRepository Repository { get; set; }

        protected IList<ItemUser> _itemusers;

        protected override async Task OnInitializedAsync()
        {
            _itemusers = (await Repository.GetItemUsersByItem(Id)).OrderByDescending(i => i.ToDate).ToList();
        }
    }
}
