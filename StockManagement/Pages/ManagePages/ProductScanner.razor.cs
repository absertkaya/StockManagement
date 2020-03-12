using Microsoft.AspNetCore.Components;
using StockManagement.Domain.IComponents;
using StockManagement.Pages.ReuseableComponents;
using System.Text.RegularExpressions;

namespace StockManagement.Pages.ManagePages
{
    public class ProductScannerBase : ComponentBase
    {
        [Parameter]
        public int? Category { get; set; }

        [Inject] 
        public NavigationManager NavigationManager { get; set; }

        protected IScannerComponent _scanner;
        protected void Submit()
        {
            if (Category == null)
                NavigationManager.NavigateTo("/productform/" + Regex.Replace(_scanner.GetResult(), @"\s+", ""));
            else
                NavigationManager.NavigateTo("/productform/categorie/" + Category + "/" + Regex.Replace(_scanner.GetResult(), @"\s+", ""));
        }

    }
}
