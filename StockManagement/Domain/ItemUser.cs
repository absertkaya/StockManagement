using System;

namespace StockManagement.Domain
{
    public class ItemUser
    {
        public virtual int Id { get; set; }
        private Item _item;

        public virtual Item Item
        {
            get { return _item; }
            set { 
                if (value == null)
                {
                    throw new ArgumentNullException("Item must not be null");
                }
                _item = value; }
        }

        private ADUser _user;

        public virtual ADUser User 
        {
            get { return _user; }
            set { 
                if (value == null)
                {
                    throw new ArgumentNullException("User must not be null");
                }
                _user = value; }
        }

        private  ADUser _assignedBy;

        public virtual ADUser AssignedBy
        {
            get { return _assignedBy; }
            set { _assignedBy = value; }
        }

        public virtual ADUser ReturnedBy { get; set; }
        public virtual DateTime FromDate { get; set; }
        public virtual DateTime? ToDate { get; set; }

        public ItemUser()
        {
            FromDate = DateTime.Now;
        }
        public ItemUser(Item item, ADUser user, ADUser assigner)
        {
            Item = item;
            User = user;
            AssignedBy = assigner;
            FromDate = DateTime.Now;
        }

        public virtual void Close(ADUser returner)
        {
            ReturnedBy = returner;
            ToDate = DateTime.Now;
        }
    }
}
