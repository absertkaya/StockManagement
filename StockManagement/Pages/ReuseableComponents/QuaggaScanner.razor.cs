using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain.IComponents;
using System;
using System.Threading.Tasks;

namespace StockManagement.Pages.ReuseableComponents
{
    public class QuaggaScannerBase : ComponentBase, IScannerComponent, IDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected string _code;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JSRuntime.InvokeVoidAsync("JsFunctions.quagga");
        }

        public string GetResult()
        {
            return _code;
        }

        public void Dispose()
        {
            JSRuntime.InvokeVoidAsync("JsFunctions.stopQuagga");
        }
    }
}
