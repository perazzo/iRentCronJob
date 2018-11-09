using iRentCronJob.Views.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iRentCronJob.Controllers
{
    public class UpComingLeaseExpirationController : Controller
    {
        [HttpGet]
        public ActionResult getLeaseExpirations()
        {
            UpComingLeaseExpiration post = new UpComingLeaseExpiration();
            return new HttpStatusCodeResult(HttpStatusCode.OK); 
        }
    }
}