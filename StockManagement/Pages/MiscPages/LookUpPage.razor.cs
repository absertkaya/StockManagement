using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Pages.MiscPages
{
    public class LookUpPageBase : ComponentBase
    {
        [Inject]
        public IItemRepository ItemRepository { get; set; }

        [Parameter]
        public string Param { get; set; }

        protected IEnumerable<Item> _foundItems;
        protected string _searchString = "";

        protected bool searching;

        protected override async Task OnInitializedAsync()
        {
            if (Param != null)
            {
                _searchString = Param;
                await Search();
            }
        }

        protected async Task Search()
        {
            searching = true;
            _foundItems = await ItemRepository.GetBySerialNrAsync(_searchString);
            searching = false;
        }

        protected async Task KeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Search();
            }
        }
    }
}
