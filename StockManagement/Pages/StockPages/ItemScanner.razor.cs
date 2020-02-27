using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain.IRepositories;

namespace StockManagement.Pages.StockPages
{
    public class ItemScannerBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject] 
        public IJSRuntime JSRuntime { get; set; }

        protected string _serialnr;

        protected bool _notInStock = false;
        protected bool _invalidSerialNr = false;

        protected ElementReference startButton;
        protected ElementReference resetButton;
        protected ElementReference video;
        protected ElementReference sourceSelectPanel;
        protected ElementReference sourceSelect;
        protected ElementReference result;

        [Parameter]
        public string Method { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstrender)
        {
            if (firstrender)
                await JSRuntime.InvokeVoidAsync("JsFunctions.scanner");
        }

        protected void Submit()
        {
            _notInStock = false;
            _invalidSerialNr = false;
            if (Method == "out")
            {
                try
                {
                    bool inStock = Repository.GetItemInStock(_serialnr);
                    if (!inStock)
                    {
                        _notInStock = true;
                    }
                    else
                    {
                        NavigationManager.NavigateTo("itemform/out/" + _serialnr);
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex);
                    _invalidSerialNr = true;
                }

            }
            else if (Method == "in")
            {
                NavigationManager.NavigateTo("itemform/in/" + _serialnr);
            }

        }

        protected void Skip()
        {
            NavigationManager.NavigateTo("itemform/" + Method);
        }
    }
}
