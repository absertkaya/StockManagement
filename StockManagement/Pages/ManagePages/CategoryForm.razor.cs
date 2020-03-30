using Blazored.Toast.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Pages.ReuseableComponents;
using System;
using System.Threading.Tasks;

namespace StockManagement.Pages.ManagePages
{
    public class CategoryFormBase : ComponentBase
    {
        [Parameter]
        public int? Id { get; set; }


        [CascadingParameter]
        protected Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject]
        private IUserRepository UserRepository { get; set; }

        [Inject] 
        public IItemRepository Repository { get; set; }
        [Inject] 
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public TelemetryClient Telemetry { get; set; }


        protected Category _category = new Category();
        protected EditContext _editContext;

        protected override async Task OnInitializedAsync()
        {
            var auth = await AuthenticationStateTask;
            var stockUser = UserRepository.GetByEmail(auth.User.Identity.Name);

            if (stockUser == null || stockUser.StockRole != StockRole.ADMIN)
            {
                NavigationManager.NavigateTo("/accessdenied");
                return;
            }

            try
            {
                if (Id != null)
                {
                    _category = (Category)await Repository.GetByIdAsync(typeof(Category), Id);
                }
                _editContext = new EditContext(_category);
            } catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                ToastService.ShowWarning("Fout bij het inladen van de data, herlaad de pagina.");
            }

        }


        protected void Submit()
        {
            if (_editContext.Validate())
            {
                try
                {
                    Repository.Save(_category);
                    ToastService.ShowSuccess("Categorie " + _category.CategoryName + " toegevoegd.");
                    Telemetry.TrackEvent("CategorieAdd");
                    NavigationManager.NavigateTo("/beheer");
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    ToastService.ShowError("Kon categorie niet toevoegen.");
                }
            }

        }
    }
}
