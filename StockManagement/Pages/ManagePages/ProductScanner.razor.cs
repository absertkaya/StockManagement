using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain.IRepositories;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class ProductScannerBase : ComponentBase
    {
        [Inject] public IItemRepository Repository { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected string _productnr;
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
            NavigationManager.NavigateTo("/productform/" + _productnr);
        }
    }
}
