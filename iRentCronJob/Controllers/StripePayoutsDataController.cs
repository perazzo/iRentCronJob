using iRentCronJob.Views.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iRentCronJob.Controllers
{
    public class StripePayoutsDataController : Controller
    {
        [HttpGet]
        public ActionResult getStripePayOuts()
        {
            UpdatePayoutsData data = new UpdatePayoutsData(); 
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}