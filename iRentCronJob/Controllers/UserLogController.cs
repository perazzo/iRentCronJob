using iRentCronJob.Views.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iRentCronJob.Controllers
{
    public class UserLogController : Controller
    {
        [HttpGet]
        public ActionResult CheckUserLog()
        {
            UserLog ul = new UserLog();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}