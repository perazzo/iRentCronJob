using iRentCronJob.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class UpComingLeaseExpiration
    {
        public MyiRentEntities db = new MyiRentEntities();

        public UpComingLeaseExpiration()
        {
            try
            {
                // get active properties with flag on
                var getProperties = db.properties.Where(p => p.Active == 0 & p.AlertUpcomingLeaseExpiration == 1).ToList();

                foreach(var property in getProperties)
                {
                    bool sendEmail = false;
                    DataTable dt = new DataTable();
                    dt = createReportTable();

                    // Check for next week
                    DateTime today = DateTime.Now;
                    DateTime oneWeek = DateTime.Now.AddDays(7);
                    var getLeaseExpiration = (from t in db.tenants
                                              join p in db.properties on t.PropertyID equals p.PropertyID
                                              where p.PropertyID == property.PropertyID
                                              && t.Prospect == 2
                                              && (t.LeaseEndDate >= today && t.LeaseEndDate <= oneWeek)
                                              select t).ToList();
                    
                    foreach(var leaseExpiration in getLeaseExpiration)
                    {
                        sendEmail = true;
                        string unitName = "";
                        var getUnit = db.units.Where(u => u.UnitID == leaseExpiration.UnitID).FirstOrDefault();
                        if (getUnit != null)
                            unitName = getUnit.UnitName;

                        // Add data to Report
                        dt.Rows.Add(
                            leaseExpiration.TenantID,
                            leaseExpiration.TenantFName + " " + leaseExpiration.TenantMName + " " + leaseExpiration.TenantLName,
                            leaseExpiration.TenantPhone,
                            leaseExpiration.TenantEmail,
                            unitName,
                            leaseExpiration.LeaseEndDate.Value.ToString("MM/dd/yyyy")
                        );
                    }

                    if (sendEmail)
                    {
                        // Convert the file to .CSV
                        StringBuilder sb = new StringBuilder();
                        IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                  Select(column => column.ColumnName);
                        sb.AppendLine(string.Join(",", columnNames));
                        foreach (DataRow row in dt.Rows)
                        {
                            IEnumerable<string> fields = row.ItemArray.Select(field =>
                                string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                            sb.AppendLine(string.Join(",", fields));
                        }

                        // Get PM or Admin email
                        string emailTo = "";
                        Email getEmail = new Email();
                        emailTo = getEmail.getPropertyManagerEmail(property.PropertyID);
                        if (emailTo == "")
                            emailTo = getEmail.getAdminEmail(property.PropertyID);

                        if (!string.IsNullOrEmpty(emailTo))
                        {
                            getEmail.sendEmail(
                                emailTo, 
                                "support@myirent.com",
                                "List of upcoming Lease Expiration from " + DateTime.Today.ToString("MM/dd/yyyy") + " to " + DateTime.Today.AddDays(7).ToString("MM/dd/yyyy") + " for Property: " + property.PropertyName,
                                "",
                                sb,
                                "LeaseExpiration.csv"
                            );
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        public DataTable createReportTable()
        {
            DataTable dt = new DataTable(); 
            dt.Columns.Add("tenantID", typeof(int));
            dt.Columns.Add("tenantName", typeof(string));
            dt.Columns.Add("TenantPhone", typeof(string));
            dt.Columns.Add("TenantEmail", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Lease Expire Date", typeof(string));

            return dt;
        }
    }
}