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
    public class ChargeTenantsController : Controller
    {
        public MyiRentEntities db = new MyiRentEntities();

        public ActionResult chargeTenants()
        {
            try
            {
                DateTime dt = DateTime.Now;
                int d = dt.Day;

                // get properties
                var getProperties = (from p in db.properties
                                     join c in db.companies on p.CompanyID equals c.CompanyID
                                     join pe in db.properties_exclude on p.PropertyID equals pe.PropertyID
                                     where p.Active == 0 && c.Active == 1
                                     && p.PropertyID != 9 && p.PropertyID != 1
                                     && pe.AutoBill != '2'
                                     select p).ToList();
                foreach(var prop in getProperties)
                {
                    // Check if first day of month to apply rental charges
                    if(d == 1)
                    {
                        int tenantCount = 0;
                        decimal amountCount = 0;
                        int emailCount = 0;
                        int messageCount = 0;

                        // get tenants
                        var getProspects = (from t in db.tenants
                                            join p in db.properties on t.PropertyID equals p.PropertyID
                                            join tt in db.tenanttransactions on t.TenantID equals tt.TenantID into ps
                                            from tt in ps.DefaultIfEmpty()
                                            select new { t = t, p = p, tt = tt })
                                           .Where(x => (x.t.PropertyID == prop.PropertyID) && (x.t.Prospect == 2))
                                           .GroupBy(x => x.t.TenantID)
                                           .Select(x => new
                                           {
                                               tenantID = x.Key,
                                               tenantFirstName = x.FirstOrDefault().t.TenantFName,
                                               tenantLastName = x.FirstOrDefault().t.TenantLName,
                                               rentalAmount = x.FirstOrDefault().t.RentalAmount,
                                               petRent = x.FirstOrDefault().t.PetRent,
                                               houseingAmount = x.FirstOrDefault().t.HousingAmount,
                                               utilityCharge = x.FirstOrDefault().t.UtilityCharge,
                                               tvCharge = x.FirstOrDefault().t.TVCharge,
                                               sercurityDeposit = x.FirstOrDefault().t.SecurityDeposit,
                                               hoaFee = x.FirstOrDefault().t.HousingAmount,
                                               parkingCharge = x.FirstOrDefault().t.ParkingCharge,
                                               storageCharge = x.FirstOrDefault().t.StorageCharge,
                                               concessionAmount = x.FirstOrDefault().t.ConcessionAmount,
                                               concessionReason = x.FirstOrDefault().t.ConcessionReason,
                                               tenantEmail = x.FirstOrDefault().t.TenantEmail,
                                               tenantPhone = x.FirstOrDefault().t.TenantPhone,
                                               tenantCellPhoneProvider = x.FirstOrDefault().t.CellPhoneProviderID,
                                               tenantMoveInDate = x.FirstOrDefault().t.MoveInDate,
                                               propertyID = x.FirstOrDefault().p.PropertyID,
                                               propertyName = x.FirstOrDefault().p.PropertyName,
                                               totalDebit = x.Where(y => (y.tt.TransactionTypeID == 1) && (y.tt.ChargeTypeID != 6) && (y.tt.TenantTransactionDate <= DateTime.Now)).Sum(y => y.tt.TransactionAmount),
                                               housingDebit = x.Where(y => (y.tt.TransactionTypeID == 1) && (y.tt.ChargeTypeID == 6) && (y.tt.TenantTransactionDate <= DateTime.Now)).Sum(y => y.tt.TransactionAmount),
                                               totalCredit = x.Where(y => (y.tt.TransactionTypeID == 2) && (y.tt.ChargeTypeID != 6) && (y.tt.TenantTransactionDate <= DateTime.Now)).Sum(y => y.tt.TransactionAmount),
                                               housingCredit = x.Where(y => (y.tt.TransactionTypeID == 2) && (y.tt.ChargeTypeID == 6) && (y.tt.TenantTransactionDate <= DateTime.Now)).Sum(y => y.tt.TransactionAmount),
                                           }).ToList();
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            } catch(Exception any)
            {
                SendUsEmail email = new SendUsEmail();
                email.sendError(any.ToString(), "Error Processing Charge Tenants CRON Job");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult test()
        {
            try
            {
                // get tenants
                var getProspects = (from t in db.tenants
                                    join p in db.properties on t.PropertyID equals p.PropertyID
                                    where t.Prospect == 2
                                    && p.PropertyID == 9
                                    select t).ToList();
                foreach(var prospect in getProspects)
                {
                    var TotalDebit = (from tt in db.tenanttransactions
                                      where tt.TenantID == prospect.TenantID && tt.TransactionTypeID == 1 
                                      && tt.ChargeTypeID != 6 && tt.TenantTransactionDate <= DateTime.Now
                                      select tt.TransactionAmount).DefaultIfEmpty(0).Sum();
                    var HousingDebit = (from tt in db.tenanttransactions
                                        where tt.TenantID == prospect.TenantID && tt.TransactionTypeID == 1
                                        && tt.ChargeTypeID == 6 && tt.TenantTransactionDate <= DateTime.Now
                                        select tt.TransactionAmount).DefaultIfEmpty(0).Sum();
                    var TotalCredit = (from tt in db.tenanttransactions
                                        where tt.TenantID == prospect.TenantID && tt.TransactionTypeID == 2
                                        && tt.ChargeTypeID != 6 && tt.TenantTransactionDate <= DateTime.Now
                                        select tt.TransactionAmount).DefaultIfEmpty(0).Sum();
                    var HousingCredit = (from tt in db.tenanttransactions
                                       where tt.TenantID == prospect.TenantID && tt.TransactionTypeID == 2
                                       && tt.ChargeTypeID == 6 && tt.TenantTransactionDate <= DateTime.Now
                                       select tt.TransactionAmount).DefaultIfEmpty(0).Sum();
                    int i = 0;
                }
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            } catch(Exception any)
            {
                Console.Write(any.ToString());
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: ChargeTenants
        public ActionResult Index()
        {
            return View();
        }
    }
}