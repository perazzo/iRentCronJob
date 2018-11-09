using iRentCronJob.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class ScheduleSendToCollection
    {
        public MyiRentEntities db = new MyiRentEntities();

        public ScheduleSendToCollection()
        {
            try
            {
                // Get all active companies that have signed to send to collection
                var getCompanies = db.companies.Where(c => c.Active == 1 && c.SendToCollection == 1).ToList();

                foreach (var company in getCompanies)
                {
                    var getProperties = db.properties.Where(p => p.CompanyID == company.CompanyID && p.Active == 0 && p.PropertyID != 9).ToList();
                    foreach (var property in getProperties)
                    {
                        DataTable dt = new DataTable();
                        dt = createCollectionTable();
                        Decimal TotalBalance = 0;
                        int count = 0;

                        // Get Former Tenants need to send to collection
                        var getFormerTenants = (from t in db.tenants
                                                join b in db.backgrounds on t.TenantID equals b.TenantID
                                                join c in db.collections on t.TenantID equals c.TenantID
                                                where t.Prospect == 3 && t.Collections == 1 && c.SentToCollections != 4
                                                && t.PropertyID == property.PropertyID
                                                select new { t, b }).ToList();

                        foreach (var formerTenant in getFormerTenants)
                        {
                            var getUnit = db.units.Where(u => u.UnitID == formerTenant.t.UnitID).FirstOrDefault();
                            var getForwarding = db.whitelists.Where(w => w.TenantID == formerTenant.t.TenantID).FirstOrDefault();
                            TenantBalance tBalance = new TenantBalance(formerTenant.t.TenantID);
                            if (tBalance.TotalTenantBalance > 0)
                            {
                                TotalBalance += tBalance.TotalTenantBalance;
                                count++;

                                // Tenant info into csv
                                dt.Rows.Add(
                                    formerTenant.t.TenantID,
                                    formerTenant.t.TenantFName + " " + formerTenant.t.TenantMName + " " + formerTenant.t.TenantLName,
                                    formerTenant.b.DOB,
                                    formerTenant.t.SSN,
                                    property.PropertyName,
                                    "Unit: " + getUnit.UnitID + " " + property.PropertyAddress1 + " " + property.PropertyCity + " " + property.PropertyState + " " + property.PropertyZip,
                                    "NA", "NA", "NA",
                                    getForwarding.FAddress + " " + getForwarding.FCity + " " + getForwarding.FState + " " + getForwarding.FZip,
                                    formerTenant.t.TenantPhone,
                                    formerTenant.t.TenantEmail,
                                    formerTenant.t.LeaseStartDate.Value.ToString("MM/dd/yyyy") + " to " + formerTenant.t.LeaseEndDate.Value.ToString("MM/dd/yyyy"),
                                    tBalance.TotalTenantBalance,
                                    "Balance Overdue as of: " + formerTenant.t.LeaseEndDate.Value.ToString("MM/dd/yyyy"),
                                    "Rental Fees"
                                );

                                var getCollection = db.collections.Where(c => c.TenantID == formerTenant.t.TenantID).ToList();
                                getCollection.ForEach(x => { x.SentToCollections = 4; });
                                db.SaveChanges();
                            }
                        }

                        if(count > 0)
                        {
                            // Convert the file to .CSV
                            StringBuilder sb = new StringBuilder();
                            foreach (DataRow row in dt.Rows)
                            {
                                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                                sb.AppendLine(string.Join(",", fields));
                            }


                            // Send the Property File (FTP)
                            Wakefield wake = new Wakefield();
                            var getLastCollection = db.collections.OrderByDescending(c => c.CollectionsID).Select(a => a.CollectionsID).First();
                            string ftpPath = wake.ftp + wake.ftpFolder + "Collection" + getLastCollection.ToString() + ".csv";
                            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                            request.Method = WebRequestMethods.Ftp.UploadFile;
                            byte[] fileBytes = Encoding.Default.GetBytes(sb.ToString());
                            //Enter FTP Server credentials
                            request.Credentials = new NetworkCredential(wake.UserName, wake.Password);
                            request.ContentLength = fileBytes.Length;
                            request.UsePassive = true;
                            request.UseBinary = true;
                            request.ServicePoint.ConnectionLimit = fileBytes.Length;
                            request.EnableSsl = true;
                            using (Stream requestStream = request.GetRequestStream())
                            {
                                requestStream.Write(fileBytes, 0, fileBytes.Length);
                                requestStream.Close();
                            }
                            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                            response.Close();

                            // Send an email to property manager/admin
                            string emailTo = "";
                            var getPMEmail = (from upm in db.users
                                              join propMap in db.userpropertymaps on upm.UserID equals propMap.UserID
                                              where upm.SecurityLevelID == 2 && upm.Active == 1 && propMap.PropertyID == property.PropertyID
                                              select upm).FirstOrDefault();
                            if (getPMEmail == null)
                            {
                                var getAdminEmail = (from upm in db.users
                                                     join propMap in db.userpropertymaps on upm.UserID equals propMap.UserID
                                                     where upm.SecurityLevelID == 1 && upm.Active == 1 && propMap.PropertyID == property.PropertyID
                                                     select upm).FirstOrDefault();
                                if (getAdminEmail != null)
                                {
                                    emailTo = getAdminEmail.UserEmail.ToString();
                                }
                            }
                            else
                            {
                                emailTo = getPMEmail.UserEmail.ToString();
                            }
                            if (emailTo != "")
                            {
                                MailMessage mailMessage = new MailMessage();
                                string sendTo = emailTo;
                                sendTo += ",gperazzo@myirent.com";
                                mailMessage.To.Add(sendTo);
                                if(company.LeadSourceCompanyID == 1)
                                {
                                    mailMessage.From = new MailAddress("support@cicreports.com");
                                } else
                                {
                                    mailMessage.From = new MailAddress("support@myirent.com");
                                }
                                string subject = "Attached is your Collections list which has " + count.ToString() + " individuals totaling " + TotalBalance.ToString("C", CultureInfo.CurrentCulture);
                                mailMessage.Subject = subject;
                                mailMessage.Body = "These are being sent to collections.";

                                byte[] contentAsBytes = Encoding.Default.GetBytes(sb.ToString());
                                MemoryStream memoryStream = new MemoryStream();
                                memoryStream.Write(contentAsBytes, 0, contentAsBytes.Length);
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                ContentType contentType = new ContentType("text/csv");
                                contentType.Name = "collection.csv";
                                Attachment attachment = new Attachment(memoryStream, contentType);
                                mailMessage.Attachments.Add(attachment);

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
                }

                // Alert us that the file run fine
                //SendUsEmail message = new SendUsEmail();
                //message.sendAlert("Just run send to collection", "Send to Collection Alert");
            }
            catch (Exception any)
            {
                SendUsEmail error = new SendUsEmail();
                error.sendError(any.ToString(), "Send to Collections Error");
            }
        }

        public DataTable createCollectionTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("tenantID", typeof(int));
            dt.Columns.Add("tenantName", typeof(string));
            dt.Columns.Add("dob", typeof(DateTime));
            dt.Columns.Add("tenantSSN", typeof(string));
            dt.Columns.Add("PropertyName", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("NA1", typeof(string));
            dt.Columns.Add("NA2", typeof(string));
            dt.Columns.Add("NA3", typeof(string));
            dt.Columns.Add("Forwarding", typeof(string));
            dt.Columns.Add("TenantPhone", typeof(string));
            dt.Columns.Add("TenantEmail", typeof(string));
            dt.Columns.Add("LeaseDates", typeof(string));
            dt.Columns.Add("TenantBalance", typeof(Decimal));
            dt.Columns.Add("BalanceOverdue", typeof(string));
            dt.Columns.Add("RentalFees", typeof(string));

            return dt;
        }
    }
}