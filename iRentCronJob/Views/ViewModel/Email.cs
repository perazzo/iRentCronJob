using iRentCronJob.Views.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace iRentCronJob.Models
{
    public class Email
    {
        public MyiRentEntities db = new MyiRentEntities();

        public Email()
        {}

        public string getPropertyManagerEmail(int propertyID)
        {
            List<string> emails = new List<string>();
            var getPMEmail = (from upm in db.users
                              join propMap in db.userpropertymaps on upm.UserID equals propMap.UserID
                              where upm.SecurityLevelID == 2 && upm.Active == 1 && propMap.PropertyID == propertyID
                              select upm).ToList();
            foreach(var pmEmail in getPMEmail)
            {
                emails.Add(pmEmail.UserEmail);
            }

            return string.Join(",", emails);
        }

        public string getAdminEmail(int propertyID)
        {
            List<string> emails = new List<string>();

            var getAdminEmail = (from upm in db.users
                                 join propMap in db.userpropertymaps on upm.UserID equals propMap.UserID
                                 where upm.SecurityLevelID == 1 && upm.Active == 1 && propMap.PropertyID == propertyID
                                 select upm).ToList();

            foreach (var adminEmail in getAdminEmail)
            {
                emails.Add(adminEmail.UserEmail);
            }

            return string.Join(",", emails);
        }

        public void sendEmail(string toEmail, string fromEmail, string subject, string body,
            StringBuilder att = null, string attName = "", string attType = "text/csv")
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(toEmail);
            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            // att?
            if(att != null)
            {
                byte[] contentAsBytes = Encoding.Default.GetBytes(att.ToString());
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Write(contentAsBytes, 0, contentAsBytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                ContentType contentType = new ContentType(attType);
                contentType.Name = attName;
                Attachment attachment = new Attachment(memoryStream, contentType);
                mailMessage.Attachments.Add(attachment);
            }

            SmtpClient smtp = new SmtpClient();
            iRentEmailConf emailConf = new iRentEmailConf();
            smtp.Host = emailConf.Host;
            smtp.Port = emailConf.Port;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential
            (emailConf.User, emailConf.Password);
            smtp.Send(mailMessage);
        }
    }
}