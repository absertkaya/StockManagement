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

        [Inject] public IItemRepository Repository { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected Category _category = new Category();
        protected EditContext _editContext;

        protected FileUploadComponent _fileUpload; 

        protected bool _submitFail;

        protected override void OnInitialized()
        { 
            if (Id != null)
            {
                _category = (Category) Repository.GetById(typeof(Category), Id);
            }
            _editContext = new EditContext(_category);
        }


        protected async Task Submit()
        {
            if (_editContext.Validate())
            {
                try
                {
                    Repository.Save(_category);
                    await _fileUpload.Upload("category" + _category.Id);
                    NavigationManager.NavigateTo("/beheer");
                }
                catch (Exception ex)
                {
                    _submitFail = true;
                }
            }

        }

        protected void Back()
        {
            NavigationManager.NavigateTo("/beheer");
        }
    }
}
