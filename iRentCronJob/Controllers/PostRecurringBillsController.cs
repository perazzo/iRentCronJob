using iRentCronJob.Views.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iRentCronJob.Controllers
{
    public class PostRecurringBillsController : Controller
    {
        [HttpGet]
        public ActionResult SchedulePostRecurringBills()
        {
            PostRecurringBills post = new PostRecurringBills();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}