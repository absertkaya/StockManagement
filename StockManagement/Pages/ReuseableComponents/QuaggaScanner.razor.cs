using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using StockManagement.Domain.IComponents;

using System.Threading.Tasks;

namespace StockManagement.Pages.ReuseableComponents
{
    public class QuaggaScannerBase : ComponentBase, IScannerComponent
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected string _code;



        public string GetResult()
        {
            return _code;
        }
    }
}
