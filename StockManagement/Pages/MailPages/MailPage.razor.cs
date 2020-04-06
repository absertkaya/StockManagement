using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.MailPages
{
    public class MailPageBase : ComponentBase
    {
        [Parameter]
        public string Id { get; set; }

        [Inject]
        public IItemRepository ItemRepository { get; set; }

        protected ADUser aduser;

        protected override async Task OnInitializedAsync()
        {
            aduser = (ADUser) await ItemRepository.GetByIdAsync(typeof(ADUser), Id);
        }
    }
}
