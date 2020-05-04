using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Domain
{
    public class MailTemplate
    {
        public virtual int Id { get; set; }
        public virtual string MailTemplateName { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }

        public virtual string[] BuildMail(string user = null, string appUser = null, string userRole = null, IList<Item> items = null)
        {
            string itemsString = "";
            if (items != null)
            {  
                foreach (Item item in items)
                {
                    itemsString += $"    - {item.Product.Description} {(!item.HasSerialNumber() ? "" : $"(S/N: {item.SerialNumber})")}\n";
                }            }
            string body = Body.Replace("**items**", itemsString).Replace("**user**", user).Replace("**me**", appUser).Replace("**role**", userRole);
            string subject = Subject.Replace("**items**", itemsString).Replace("**user**", user).Replace("**me**", appUser).Replace("**role**", userRole);
            return new string[] { subject, body };
        }
    }
} 
