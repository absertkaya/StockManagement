using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        private bool sortFirstNameDesc;
        private bool sortLastNameDesc;

        protected override async Task OnInitializedAsync()
        {
            _users = await Repository.GetAllAsync<ADUser>();
            _filteredUsers = new List<ADUser>(_users).OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
        }

        protected void SortByFirstName()
        {
            if (!sortFirstNameDesc)
            {
                _filteredUsers = _filteredUsers.OrderBy(u => u.FirstName);
            } else
            {
                _filteredUsers = _filteredUsers.OrderByDescending(u => u.FirstName);
            }

            sortFirstNameDesc = !sortFirstNameDesc;
        }

        protected void SortByLastName()
        {
            if (!sortLastNameDesc)
            {
                _filteredUsers = _filteredUsers.OrderBy(u => u.LastName);
            }
            else
            {
                _filteredUsers = _filteredUsers.OrderByDescending(u => u.LastName);
            }

            sortLastNameDesc = !sortLastNameDesc;
        }



        protected void Filter()
        {
            _filteredUsers = _users.Where(u => u.NormalizedSearchInfo.Contains(Regex.Replace(_filterString.ToLower(), " ", "")));
            StateHasChanged();
        }

        protected void NavigateToUserDetail(ADUser user)
        {
            NavigationManager.NavigateTo("/gebruiker/" + user.Id);
        }
    }
}
