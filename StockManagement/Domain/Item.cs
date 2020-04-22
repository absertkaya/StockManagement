using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StockManagement.Domain
{
    public class Item
    {
        private DateTime? _deliveryDate;
        private string _serialNumber;

        public virtual int Id { get; set; }
        [Required(ErrorMessage = "Product is verplicht")]
        public virtual Product Product { get; set; }
        public virtual ADUser ADUser { get; set; }
        public virtual string Comment { get; set; }
        [Required(ErrorMessage = "Serienummer is verplicht")]
        public virtual string SerialNumber { 
            get {

                    return _serialNumber;

            } set { _serialNumber = value; }
        }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public virtual DateTime? DeliveryDate { 
            get { return _deliveryDate; } 
            set { 

                _deliveryDate = value; 
            } 
        }
        public virtual DateTime? InvoiceDate { get; set; }
        public virtual ItemStatus ItemStatus { get; set; }
        [Required(ErrorMessage = "Leverancier is verplicht")]
        public virtual Supplier Supplier { get; set; }
        public virtual IList<ItemUser> ItemUsers { get; set; }

        public virtual string Imei { get; set; }
        public virtual string Hostname { get; set; }
        public virtual string License { get; set; }
        public virtual string Carepack { get; set; }
        public virtual string VGDNumber { get; set; }

        public Item()
        {
            DeliveryDate = DateTime.Today;
            InvoiceDate = DateTime.Today;
            ItemStatus = ItemStatus.INSTOCK;
            ItemUsers = new List<ItemUser>();
        }

        public virtual bool HasSerialNumber()
        {
            return !SerialNumber.StartsWith("NOSERIALNR");
        } 

        public virtual bool IsNotFaulty()
        {
            return ItemStatus == ItemStatus.INSTOCK || ItemStatus == ItemStatus.OUTSTOCK;
        }

        public virtual ItemUser RemoveFromStock(ADUser user, ADUser assigner)
        {
            if (ItemStatus.INSTOCK != ItemStatus)
            {
                throw new Exception("Can't remove in this state");
            }

            ItemStatus = ItemStatus.OUTSTOCK;
            var iu = new ItemUser(this, user, assigner);
            ItemUsers.Add(iu);
            user?.ItemUsers.Add(iu);
            ADUser = user;
            return iu;
        }

        public virtual ItemUser ReturnToStock(ADUser returner)
        {
            if (ItemStatus != ItemStatus.OUTSTOCK)
            {
                throw new Exception("Can't be returned in this state");
            }
            
            ItemUser iu = ItemUsers.FirstOrDefault(x => x.ToDate == null);
            if (iu == null)
            {
                if (ADUser != null)
                {
                    iu = new ItemUser(this, ADUser, returner);
                    ItemUsers.Add(iu);
                    ADUser?.ItemUsers.Add(iu);
                }
            }
            if (iu != null)
            {
                iu.Close(returner);
            }
            if (ADUser != null)
            {
                ADUser.Items.Remove(this);
                ADUser = null;
            }
            ItemStatus = ItemStatus.INSTOCK;
            return iu;
        }
    }
}
