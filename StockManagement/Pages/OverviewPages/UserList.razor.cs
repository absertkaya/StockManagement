using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class UserListBase : ComponentBase
    {
        [Inject]
        public IRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected IList<ADUser> _users;

        protected override async Task OnInitializedAsync()
        {
            _users = await Repository.GetAll<ADUser>();
        }

        protected void GetUser(string id)
        {
            NavigationManager.NavigateTo("/gebruiker/" + id);
        }
    }
}
