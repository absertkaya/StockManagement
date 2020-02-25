using System.Collections.Generic;

namespace StockManagement.Domain
{
    public class ADUser
    {
        public virtual int Id { get; set; }
        public virtual IList<Item> Items { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual string Office { get; set; }
        public virtual string OfficeRole { get; set; }

        public virtual IList<ItemUser> ItemUsers { get; set; }

        public ADUser()
        {
            UserRole = UserRole.BOER;
        }


    }
}
