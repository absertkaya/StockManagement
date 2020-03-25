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

        private IEnumerable<ADUser> _users;
        protected IEnumerable<ADUser> _filteredUsers;
        protected ADUser _selectedUser;
        protected string _filterString = "";

        protected override async Task OnInitializedAsync()
        {
            _users = await Repository.GetAllAsync<ADUser>();
            _filteredUsers = new List<ADUser>(_users);
        }

        protected void Filter()
        {
            _filteredUsers = _users.Where(u => u.NormalizedSearchInfo.Contains(_filterString.Trim().ToLower()));
            StateHasChanged();
        }

        protected void NavigateToUserDetail(ADUser user)
        {
            NavigationManager.NavigateTo("/gebruiker/" + user.Id);
        }
    }
}
