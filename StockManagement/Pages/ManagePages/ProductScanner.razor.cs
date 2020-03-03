using Microsoft.AspNetCore.Components;
using StockManagement.Pages.ReuseableComponents;

namespace StockManagement.Pages.ManagePages
{
    public class ProductScannerBase : ComponentBase
    {
        [Inject] 
        public NavigationManager NavigationManager { get; set; }

        protected Scanner _scanner;
        protected ElementReference resetButton;
        protected void Submit()
        {
            NavigationManager.NavigateTo("/productform/" + _scanner.GetResult());
        }
    }
}
