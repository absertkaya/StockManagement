using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StockManagement.Domain
{
    public class Item
    {
        private DateTime? _deliveryDate;

        public virtual int Id { get; set; }
        [Required(ErrorMessage = "Product is verplicht")]
        public virtual Product Product { get; set; }
        public virtual ADUser ADUser { get; set; }
        public virtual string Comment { get; set; }
        [Required(ErrorMessage = "Serienummer is verplicht")]
        public virtual string SerialNumber { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public virtual DateTime? DeliveryDate { 
            get { return _deliveryDate; } 
            set { 

                _deliveryDate = value; 
            } 
        }
        [Required]
        public virtual DateTime? InvoiceDate { get; set; }
        public virtual ItemStatus ItemStatus { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual IList<ItemUser> ItemUsers { get; set; }

        public Item()
        {
            DeliveryDate = DateTime.Today;
            InvoiceDate = DateTime.Today;
            ItemStatus = ItemStatus.INSTOCK;
            ItemUsers = new List<ItemUser>();
        }

        public virtual void RemoveFromStock(ADUser user, ADUser assigner)
        {
            if (ItemStatus.INSTOCK != ItemStatus)
            {
                throw new Exception("Can't remove in this state");
            }

            if (ADUser != null)
            {
                throw new Exception("Item has a user");
            }

            ItemStatus = ItemStatus.OUTSTOCK;
            ItemUser use = new ItemUser(this, user, assigner);
            ADUser = user;
        }

        public virtual void ReturnToStock(ADUser returner)
        {
            if (ItemStatus != ItemStatus.OUTSTOCK)
            {
                throw new Exception("Can't be returned in this state");
            }
            ItemStatus = ItemStatus.INSTOCK;
            ItemUser use = ItemUsers.FirstOrDefault(x => x.ToDate == null);
            if (use != null)
            {
                use.Close(returner);
            }
            if (ADUser != null)
            {
                ADUser.Items.Remove(this);
                ADUser = null;
            }
        }
    }
}
