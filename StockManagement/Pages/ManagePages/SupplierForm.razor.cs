using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class SupplierFormBase : ComponentBase
    {
        [Parameter]
        public int? Id { get; set; }
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }
        protected Supplier _supplier;
        protected EditContext _editContext;

        protected override async Task OnInitializedAsync()
        {
            var auth = await AuthenticationStateTask;
            var stockUser = UserRepository.GetByEmail(auth.User.Identity.Name);

            if (stockUser == null || stockUser.StockRole != StockRole.ADMIN)
            {
                Telemetry.TrackEvent("AccessDenied");
                NavigationManager.NavigateTo("/accessdenied");
                return;
            }
        }

        protected override void OnInitialized()
        {
            if (Id != null)
            {
                _supplier = (Supplier)Repository.GetById(typeof(Supplier), Id);
            }
            
            if (_supplier == null)
            {
                _supplier = new Supplier();
            }

            _editContext = new EditContext(_supplier);
        }

        protected void Submit()
        {
            if (_editContext.Validate())
            {
                try
                {
                    Repository.Save(_supplier);
                    ToastService.ShowSuccess("Leverancier " + _supplier.SupplierName + " toegevoegd.");
                    Telemetry.TrackEvent("AddSupplier");
                    NavigationManager.NavigateTo("/beheer");
                } catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    ToastService.ShowError("Kon leverancier niet toevoegen.");
                }
            }
        }
    }
}
