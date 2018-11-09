using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class iRentEmailConf
    {
        public string Host = "smtp.myirent.com";
        public int Port = 587;
        public string User = "support@myirent.com";
        public string Password = "iRent4Now!";
    }
}