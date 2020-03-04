using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain.IComponents;
using System.Threading.Tasks;

namespace StockManagement.Pages.ReuseableComponents
{
    public class ScannerBase : ComponentBase, IScannerComponent
    {
        [Parameter]
        public string Kind { get; set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        protected string _code;
        protected ElementReference startButton;
        protected ElementReference video;
        protected ElementReference sourceSelectPanel;
        protected ElementReference sourceSelect;
        protected ElementReference result;

        protected override async Task OnAfterRenderAsync(bool firstrender)
        {
            if (firstrender)
                await JSRuntime.InvokeVoidAsync("JsFunctions.scanner");
        }


        public string GetResult()
        {
            return _code;
        }
    }
}
