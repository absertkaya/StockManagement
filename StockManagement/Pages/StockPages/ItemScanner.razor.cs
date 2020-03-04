using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain.IComponents;
using StockManagement.Domain.IRepositories;
using StockManagement.Pages.ReuseableComponents;
using System;
using System.Threading.Tasks;

namespace StockManagement.Pages.StockPages
{
    public class ItemScannerBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        protected ElementReference resetButton;
        protected bool _notInStock = false;
        protected bool _invalidSerialNr = false;

        protected IScannerComponent _scanner;
        protected bool _quagga = true;

        [Parameter]
        public string Method { get; set; }

        protected void Submit()
        {
            _notInStock = false;
            _invalidSerialNr = false;
            string res = _scanner.GetResult();
            if (Method == "out")
            {
                try
                {
                    bool inStock = Repository.GetItemInStock(res);
                    if (!inStock)
                    {
                        _notInStock = true;
                    }
                    else
                    {
                        NavigationManager.NavigateTo("itemform/out/" + res);
                    }
                }
                catch (ArgumentException ex)
                {
                    _invalidSerialNr = true;
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
