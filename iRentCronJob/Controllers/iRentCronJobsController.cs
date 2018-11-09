using iRentCronJob.Views.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iRentCronJob.Controllers
{
    public class iRentCronJobsController : Controller
    {
        // GET: iRentCronJobs
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ScheduleSendToCollection()
        {
            ScheduleSendToCollection sendToCollection = new ScheduleSendToCollection();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

    }
}