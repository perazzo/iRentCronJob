using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class Wakefield
    {
        public string ftp = "ftp://clients.wakeassoc.com/";
        //FTP Folder name. Leave blank if you want to upload to root folder.
        public string ftpFolder = "uploads/";
        public string UserName = "iRent.Corp";
        public string Password = "iR3nt2W@ke";
    }
}