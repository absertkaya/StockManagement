using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain.IComponents;
using StockManagement.Domain.IRepositories;
using StockManagement.Pages.ReuseableComponents;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockManagement.Pages.StockPages
{
    public class ItemScannerBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        protected ElementReference resetButton;

        protected IScannerComponent _scanner;
        protected bool _quagga = true;

        [Parameter]
        public string Method { get; set; }

        protected void Submit()
        {
            string res = null;
            if (_scanner.GetResult() != null)
            {
                res = Regex.Replace(_scanner.GetResult(), @"\s+", "");
            }
            
            if (Method == "out")
            {
                try
                {
                    bool inStock = Repository.GetItemInStock(res);
                    if (!inStock)
                    {
                        ToastService.ShowError("Item is niet in stock.");
                    }
                    else
                    {
                        NavigationManager.NavigateTo("itemform/out/" + res);
                    }
                }
                catch (ArgumentException ex)
                {
                    ToastService.ShowError("Item bestaat niet.");
                }

            }
            else if (Method == "in")
            {
                NavigationManager.NavigateTo("itemform/in/" + res);
            }

        }

        protected void SwitchScanner()
        {
            _quagga = !_quagga;
        }

        protected void Skip()
        {
            NavigationManager.NavigateTo("itemform/" + Method);
        }
    }
}
