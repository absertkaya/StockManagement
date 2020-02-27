using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain.IRepositories;
using System.Threading.Tasks;

namespace StockManagement.Pages.ReuseableComponents
{
    public class ScannerBase : ComponentBase
    {
        [Inject] private IItemRepository Repository { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        protected string _productnr;
        protected ElementReference startButton;
        protected ElementReference resetButton;
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
            return _productnr;
        }
    }
}
