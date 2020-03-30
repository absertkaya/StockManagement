using Blazor.Extensions.Storage.Interfaces;
using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Domain.IServices;
using StockManagement.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagement.Pages.OverviewPages
{
    public class OverviewBase : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        protected IList<Category> _categories;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _categories = await Repository.GetAllAsync<Category>();
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }
             
        }
    }
}
