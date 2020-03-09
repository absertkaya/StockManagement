using Microsoft.AspNetCore.Components;
using StockManagement.Domain.IComponents;
using StockManagement.Pages.ReuseableComponents;
using System.Text.RegularExpressions;

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
            NavigationManager.NavigateTo("/productform/" + Regex.Replace(_scanner.GetResult(), @"\s+", ""));
        }

        protected void SwitchScanner()
        {
            _quagga = !_quagga;
        }
    }
}
