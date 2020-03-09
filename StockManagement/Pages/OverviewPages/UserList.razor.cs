using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System.Collections.Generic;
using System.Linq;
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
        protected ADUser _selectedUser;

        protected override async Task OnInitializedAsync()
        {
            _users = await Repository.GetAll<ADUser>();
        }

        protected async Task<IEnumerable<ADUser>> SearchUser(string searchString)
        {
            return await Task.FromResult(_users.Where(u => u.NormalizedSearchInfo.Contains(searchString.Trim().ToLower())));
        }

        protected void GetSelectedUser()
        {
            if (_selectedUser != null)
            {
                NavigationManager.NavigateTo("/gebruiker/" + _selectedUser.Id);
            }
        }

        protected void GetUser(string id)
        {
            NavigationManager.NavigateTo("/gebruiker/" + id);
        }
    }
}
