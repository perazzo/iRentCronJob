using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentCronJob.Models
{
    public class TenantModel
    {

        public int getTenantID(string description)
        {
            string search = "TenantID: ";
            try
            {
                return Int32.Parse(description.Substring(description.IndexOf(search) + search.Length));
            } catch(Exception any)
            {
                return 0;
            }
        }
    }
}