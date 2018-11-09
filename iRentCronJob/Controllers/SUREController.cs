using iRentCronJob.Views.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iRentCronJob.Controllers
{
    public class SUREController : Controller
    {
        // GET: SURE
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult checkReports()
        {
            SureReports sr = new SureReports();
            sr.getReport();
                 
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}