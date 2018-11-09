using iRentCronJob.Models;
using iRentCronJob.Views.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace iRentCronJob.Controllers
{
    public class MySQLTimeoutController : Controller
    {
        public MyiRentEntities db = new MyiRentEntities();

        [HttpGet]
        public ActionResult GetConnection()
        {
            try
            {
                var getComp = db.companies.Where(c => c.CompanyID == 27).FirstOrDefault();
                getComp.LeadSourceCompanyID = 0;
                db.SaveChanges();                
            } catch (Exception any)
            {
                SendUsEmail error = new SendUsEmail();
                error.sendError(any.ToString(), "Error MySQL Timeout");
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}