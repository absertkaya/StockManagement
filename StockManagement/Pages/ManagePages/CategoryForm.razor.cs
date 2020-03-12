using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
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

        [Inject] 
        public IItemRepository Repository { get; set; }
        [Inject] 
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }

        protected Category _category = new Category();
        protected EditContext _editContext;

        protected override void OnInitialized()
        { 
            if (Id != null)
            {
                _category = (Category) Repository.GetById(typeof(Category), Id);
            }
            _editContext = new EditContext(_category);
        }


        protected void Submit()
        {
            if (_editContext.Validate())
            {
                try
                {
                    Repository.Save(_category);
                    ToastService.ShowSuccess("Categorie " + _category.CategoryName + " toegevoegd.");
                    NavigationManager.NavigateTo("/beheer");
                }
                catch (Exception ex)
                {
                    ToastService.ShowError("Kon categorie niet toevoegen.");
                }
            }

        }
    }
}
