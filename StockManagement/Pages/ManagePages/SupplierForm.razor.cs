using Microsoft.AspNetCore.Components;
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
        protected Supplier _supplier;
        protected EditContext _editContext;



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
                    NavigationManager.NavigateTo("/beheer");
                } catch (Exception ex)
                {
                    //Log
                }
            }
        }
    }
}
