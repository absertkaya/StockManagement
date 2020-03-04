using Microsoft.AspNetCore.Components;
using StockManagement.Domain.IComponents;
using StockManagement.Pages.ReuseableComponents;

namespace StockManagement.Pages.ManagePages
{
    public class ProductScannerBase : ComponentBase
    {
        [Inject] 
        public NavigationManager NavigationManager { get; set; }

        protected IScannerComponent _scanner;
        protected bool _quagga = true;
        protected void Submit()
        {
            NavigationManager.NavigateTo("/productform/" + _scanner.GetResult());
        }

        protected void SwitchScanner()
        {
            _quagga = !_quagga;
        }
    }
}
