using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ModalComponents
{
    public class AddSubscriptionBase: ComponentBase
    {
        [CascadingParameter]
        private BlazoredModalInstance BlazoredModal { get; set; }

        [Parameter]
        public ADUser ADUser { get; set; }

        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected IList<MobileAccount> _accounts = new List<MobileAccount>();

        protected bool _addMode;
        protected bool _accError;

        protected int? _selectedAccount;

        protected MobileSubscription _sub;
        protected MobileAccount _acc = new MobileAccount();
        protected EditContext _editContext;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _accounts = await UserRepository.GetAllMobileAccounts();

            }
            catch (Exception ex)
            {
                BlazoredModal.Close();
            }
        }

        protected override void OnInitialized()
        {
            _sub = new MobileSubscription();
            _editContext = new EditContext(_sub);
            _sub.User = ADUser;
        }

        protected void FireAccountChange(ChangeEventArgs e)
        {
            _selectedAccount = int.Parse(e.Value.ToString());
            _sub.MobileAccount = _accounts.FirstOrDefault(a => a.Id == (int)_selectedAccount);
        }

        protected void DeleteAccount()
        {
            MobileAccount acc = _accounts.FirstOrDefault(a => a.Id == _selectedAccount);
            UserRepository.Delete(acc);
            _accounts.Remove(acc);
            _selectedAccount = null;
        }

        protected void SwitchAddMode()
        {
            _addMode = !_addMode;
        }

        protected void SubmitAccount()
        {
            _accError = false;
            if (!string.IsNullOrWhiteSpace(_acc.AccountName) && !string.IsNullOrWhiteSpace(_acc.AccountNumber))
            {
                UserRepository.Save(_acc);
                _accounts.Add(_acc);
                _selectedAccount = _acc.Id;
                _sub.MobileAccount = _acc;
                _addMode = false;
                _selectedAccount = _acc.Id;
                _acc = new MobileAccount();
            }
            else
            {
                _accError = true;
            }
        }

        protected void Submit()
        {
            if (_editContext.Validate())
            {
                UserRepository.Save(_sub);
                BlazoredModal.Close(ModalResult.Ok<MobileSubscription>(_sub));
            }

        }
    }
}
