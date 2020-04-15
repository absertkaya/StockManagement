using StockManagement.Graph;
using System.Collections.Generic;

namespace StockManagement.Domain
{
    public class ADUser
    {
        public virtual string Id { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string OfficeRole { get; set; }
        public virtual string Mail { get; set; }
        public virtual string MobilePhone { get; set; }
        public virtual string Office { get; set; }
        public virtual string LastName { get; set; }
        public virtual IList<Item> Items { get; set; }
        public virtual IList<ItemUser> ItemUsers { get; set; }

        public virtual IList<MobileSubscription> MobileSubscriptions { get; set; }

        public virtual StockRole? StockRole { get; set; }

        public virtual string NormalizedSearchInfo => FirstName.ToLower() + LastName.ToLower() + Mail.ToLower();
        

        public ADUser()
        {
            Items = new List<Item>();
            ItemUsers = new List<ItemUser>();
            MobileSubscriptions = new List<MobileSubscription>();
        }

        public ADUser(GraphUser user) : this()
        {
            Id = user.Id;
            DisplayName = user.DisplayName;
            FirstName = user.GivenName;
            LastName = user.Surname;
            OfficeRole = user.JobTitle == null ? "" : user.JobTitle;
            Mail = user.Mail;
            MobilePhone = user.MobilePhone;
            Office = user.OfficeLocation == null ? "" : user.OfficeLocation.ToString();
        }

        public virtual void AddSubscription(MobileSubscription sub)
        {
            if (!MobileSubscriptions.Contains(sub))
            {
                MobileSubscriptions.Add(sub);
            }
            
        }
    }
}
