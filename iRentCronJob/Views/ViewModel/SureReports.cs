using iRentCronJob.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class SureReports
    {
        public MyiRentEntities db = new MyiRentEntities();

        public void getReport()
        {
            try
            {
                DateTime now = DateTime.Now;
                string repotroday = "report_" + now.Year.ToString() + "_" + now.Month.ToString() + "_" + now.Day.ToString() + ".csv";

                WebClient client = new WebClient();
                string url = "ftp://vps.myirent.com/" + repotroday;
                client.Credentials = new NetworkCredential("SURE", "iRent@123");
                string contents = client.DownloadString(url);

                // Loop over the rows
                foreach (string row in contents.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        string[] columns = row.Split(',');
                        if (columns[2].ToString() != "Unique Customer ID")
                        {
                            int tenantID = Int32.Parse(columns[2].ToString());
                            string cancellation = columns[7].ToString();
                            if (String.IsNullOrEmpty(cancellation))
                            {
                                // Set insurance true
                                var getTenant = db.tenants.Find(tenantID);
                                
                            }
                        }
                    }
                }

                int i = 0;
            } 
            catch (Exception any)
            {
                Console.WriteLine(any.ToString());
                if(any.Message != "The remote server returned an error: (550) File unavailable (e.g., file not found, no access).")
                {

                }
            }
        }
    }
}