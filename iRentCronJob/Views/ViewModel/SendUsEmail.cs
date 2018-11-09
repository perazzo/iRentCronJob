using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class SendUsEmail
    {
        iRentEmailConf emailConf = new iRentEmailConf();

        public void sendError(string MSG, string subjectError)
        {
            MailMessage mailMessage = new MailMessage();
            string sendTo = "wkelty@myirent.com, gperazzo@myirent.com";
            mailMessage.To.Add(sendTo);
            mailMessage.From = new MailAddress("support@myirent.com");
            mailMessage.Subject = subjectError;
            mailMessage.Body = MSG;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = emailConf.Host;
            smtp.Port = emailConf.Port;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential
            (emailConf.User, emailConf.Password);
            smtp.Send(mailMessage);
        }

        public void sendAlert(string MSG, string subjectError)
        {
            MailMessage mailMessage = new MailMessage();
            string sendTo = "wkelty@myirent.com, gperazzo@myirent.com";
            mailMessage.To.Add(sendTo);
            mailMessage.From = new MailAddress("support@myirent.com");
            mailMessage.Subject = subjectError;
            mailMessage.Body = MSG;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = emailConf.Host;
            smtp.Port = emailConf.Port;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential
            (emailConf.User, emailConf.Password);
            smtp.Send(mailMessage);
        }
    }
}