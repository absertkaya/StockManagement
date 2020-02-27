using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        protected bool _notInStock = false;
        protected bool _invalidSerialNr = false;

        protected Scanner _scanner;

        [Parameter]
        public string Method { get; set; }

        protected void Submit()
        {
            _notInStock = false;
            _invalidSerialNr = false;
            if (Method == "out")
            {
                try
                {
                    bool inStock = Repository.GetItemInStock(_scanner.GetResult());
                    if (!inStock)
                    {
                        _notInStock = true;
                    }
                    else
                    {
                        NavigationManager.NavigateTo("itemform/out/" + _scanner.GetResult());
                    }
                }
                catch (ArgumentException ex)
                {
                    _invalidSerialNr = true;
                }

            }
            else if (Method == "in")
            {
                NavigationManager.NavigateTo("itemform/in/" + _scanner.GetResult());
            }

        }

        protected void Skip()
        {
            NavigationManager.NavigateTo("itemform/" + Method);
        }
    }
}
