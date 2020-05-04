using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
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
        [Parameter]
        public MobileSubscription MobileSubscription { get; set; }
        [Parameter]
        public MobileAccount Account { get; set; }

        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }

        protected IList<MobileAccount> _accounts = new List<MobileAccount>();

        protected bool _addMode;
        protected bool _accError;
        protected bool _hasSubs;

        protected int? _selectedAccount;

        protected EditContext _editContext;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _accounts = await UserRepository.GetAllMobileAccounts();
                if (Account != null)
                {
                    _selectedAccount = Account.Id;
                    CheckHasSubs();
                }
            }
            catch (Exception ex)
            {
                BlazoredModal.Close();
            }
        }

        protected void CheckHasSubs()
        {
            _hasSubs = Account != null && Account.HasSubscriptions();
        }

        protected override void OnInitialized()
        {
            if (MobileSubscription == null)
            {
                MobileSubscription = new MobileSubscription();
                Account = new MobileAccount();
                MobileSubscription.User = ADUser;
            } 

            _editContext = new EditContext(MobileSubscription);
        }

        protected void FireAccountChange(ChangeEventArgs e)
        {
            _selectedAccount = int.Parse(e.Value.ToString());
            Account = _accounts.FirstOrDefault(a => a.Id == (int)_selectedAccount);
            MobileSubscription.MobileAccount = Account;
            CheckHasSubs();
        }

        protected void DeleteAccount()
        {
            MobileAccount acc = _accounts.FirstOrDefault(a => a.Id == _selectedAccount);
            if (acc != null)
            {
                UserRepository.Delete(acc);
                _accounts.Remove(acc);
                Account = new MobileAccount();
                MobileSubscription.MobileAccount = null;
                _selectedAccount = null;
            }
        }

        protected void SwitchAddMode()
        {
            Account = new MobileAccount();
            _selectedAccount = null;
            _addMode = !_addMode;
        }

        protected void SubmitAccount()
        {
            _accError = false;
            if (!string.IsNullOrWhiteSpace(Account.AccountName) && !string.IsNullOrWhiteSpace(Account.AccountNumber))
            {
                MobileSubscription.MobileAccount = Account;
                UserRepository.Save(Account);
                if (!_accounts.Any(a => a.Id == Account.Id))
                {
                    _accounts.Add(Account);
                }
                _selectedAccount = Account.Id;
                MobileSubscription.MobileAccount = Account;
                _addMode = false;
                _hasSubs = false;
                _selectedAccount = Account.Id;
                Account = new MobileAccount();
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
                UserRepository.Save(MobileSubscription);
                ToastService.ShowSuccess("Abonnement toegevoegd.");
                BlazoredModal.Close(ModalResult.Ok<MobileSubscription>(MobileSubscription));
            }

        }
    }
}
