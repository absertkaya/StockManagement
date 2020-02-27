using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using System;

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

        protected bool _submitFail;

        protected override void OnInitialized()
        {
            _editContext = new EditContext(_category);
        }

        protected void Submit()
        {
            if (_editContext.Validate())
            {
                try
                {
                    Repository.Save(_category);
                    NavigationManager.NavigateTo("/beheer", true);
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
