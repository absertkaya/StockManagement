using Microsoft.AspNetCore.Components;
using StockManagement.Domain.IRepositories;
using System.Threading.Tasks;

namespace StockManagement.Pages.StockPages
{
    public class UpdateSuccesBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public string Method { get; set; }

        protected int _amountInStock;

        protected override async Task OnInitializedAsync()
        {
            _amountInStock = await Repository.GetAmountInStockValue(Id);
        }

        protected void Add()
        {
            NavigationManager.NavigateTo("scanner/in");
        }

        protected void Remove()
        {
            NavigationManager.NavigateTo("scanner/out");
        }
    }
}
