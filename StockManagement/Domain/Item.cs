using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagement.Domain
{
    public class Item
    {
        private DateTime _deliveryDate;

        public virtual int Id { get; set; }
        [Required(ErrorMessage = "Product is verplicht")]
        public virtual Product Product { get; set; }
        public virtual ADUser ADUser { get; set; }
        public virtual string Comment { get; set; }
        public virtual bool IsDefective { get; set; }
        [Required(ErrorMessage = "Serienummer is verplicht")]
        public virtual string SerialNumber { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public virtual DateTime DeliveryDate { 
            get { return _deliveryDate; } 
            set { 
                if (value > DateTime.Today)
                {
                    throw new ArgumentException("Delivery Date can't be in the future.");
                }
                _deliveryDate = value; 
            } 
        }
        [Required]
        public virtual DateTime InvoiceDate { get; set; }
        public virtual bool InStock { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual IList<ItemUser> ItemUsers { get; set; }

        public Item()
        {
            DeliveryDate = DateTime.Today;
            InvoiceDate = DateTime.Today;
            InStock = true;
        }
    }
}
