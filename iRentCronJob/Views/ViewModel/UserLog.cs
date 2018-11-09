using iRentCronJob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class UserLog
    {
        public MyiRentEntities db = new MyiRentEntities();

        public UserLog()
        {
            var getUserLog = db.userlogs.Where(x => x.LastPropertyID == 0).ToList();
            foreach(var ul in getUserLog)
            {
                db.userlogs.Remove(ul);
                db.SaveChanges();
            }
        }
    }
}