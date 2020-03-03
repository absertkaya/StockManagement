using StockManagement.Domain.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StockManagement.Data.Services
{
    public class GmailService : MailService
    {
        private SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
        private MailMessage mail = new MailMessage();

        public GmailService()
        {
            client.Credentials = new NetworkCredential("stockmanagervgd@gmail.com", "stockmanager123");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            client.Port = 587;
            client.EnableSsl = true;
            mail.From = new MailAddress("stockmanagervgd@gmail.com", "Manager");
        }

        public void SendMail(string subj, string msg, string to)
        {
            mail.Subject = subj;
            mail.Body = msg;
            mail.To.Add(to);
            client.Send(mail);
        }
    }
}
