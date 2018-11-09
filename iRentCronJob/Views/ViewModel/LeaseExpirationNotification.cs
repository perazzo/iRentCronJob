using iRentCronJob.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class LeaseExpirationNotification
    {
        public MyiRentEntities db = new MyiRentEntities();

        public void LeaseExpiration(int pId)
        {

        }

    }
}