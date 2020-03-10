using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using StockManagement.Domain.IRepositories;
using StockManagement.Pages.DialogComponents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;

namespace StockManagement.Pages.OverviewPages
{
    public class ItemListBase : ComponentBase
    {
        [Inject]
        public IItemRepository Repository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IModalService Modal { get; set; }
        [Parameter]
        public int Id { get; set; }

        protected Product _product;

        protected Item _itemToDelete;

        protected bool _deleteFail;

        protected override void OnInitialized()
        {
            _product = (Product) Repository.GetById(typeof(Product), Id);
        }

        protected void GetHistory(int id)
        {
            NavigationManager.NavigateTo("/itemhistoriek/" + id);
        }

        protected void EditItem(int id)
        {
            NavigationManager.NavigateTo("/itemform/in/id/" + id);
        }

        protected void DeleteItem(Item item)
        {
            _itemToDelete = item;
            Repository.Delete(_itemToDelete);
            _product.Items.Remove(_itemToDelete);
            
        }
        private void ShowModal()
        {
            var options = new ModalOptions()
            {
                Position = "blazored-modal-center"
            };
            Modal.OnClose += ModalClosed;
            Modal.Show<Confirmation>("confirmation", options);
        }

        private void ModalClosed(ModalResult result)
        {
            if (!result.Cancelled)
            {

                    Repository.Delete(_itemToDelete);
                    _product.Items.Remove(_itemToDelete);

            }
        }
    }
}
