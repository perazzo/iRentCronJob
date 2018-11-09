using iRentCronJob.Models;
using iRentCronJob.Views.ViewModel;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace iRentCronJob.Controllers
{
    public class StripeController : Controller
    {
        public MyiRentEntities db = new MyiRentEntities();

        // GET: Stripe
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult verifyPayments()
        {
            try
            {
                var getStripes = (from s in db.stripes
                                  join p in db.properties on s.PropertyID equals p.PropertyID
                                  join c in db.companies on p.CompanyID equals c.CompanyID
                                  where c.Active == 1
                                  select s).ToList();

                foreach(var stripe in getStripes)
                {
                    // Villa Nueva for test
                    StripeConfiguration.SetApiKey(stripe.SecretKey);

                    // Get Payments for a month date range
                    DateTime today = DateTime.Now;
                    var chargeService = new StripeChargeService();
                    StripeList<StripeCharge> chargeItems = chargeService.List(
                      new StripeChargeListOptions()
                      {
                          Limit = 1000,
                          Created = new StripeDateFilter
                          {
                              GreaterThanOrEqual = today.AddMonths(-1),
                              LessThanOrEqual = today
                          }
                      }
                    );

                    foreach (var item in chargeItems)
                    {
                        if (item.Refunded)
                        {
                            // Remove it from tenant ledger if there
                            int tenantID = 0;
                            if (!string.IsNullOrEmpty(item.CustomerId))
                            {
                                var getStripeCustomer = db.stripe_customer.Where(x => x.StripeCustomer == item.CustomerId).FirstOrDefault();
                                if (getStripeCustomer != null)
                                    tenantID = (int)getStripeCustomer.TenantID;
                            }
                            else
                            {
                                TenantModel tm = new TenantModel();
                                tenantID = tm.getTenantID(item.Description);
                            }
                            double amount = (double)item.Amount / 100;
                            SourceType source = item.Source.Type;
                            if (source.ToString() == "Card")
                            {
                                amount = (amount - 0.3) * 0.971;
                            }
                            else
                            {
                                double achAmount = amount * 0.008;
                                if (achAmount > 5)
                                {
                                    amount -= 5;
                                }
                                else
                                {
                                    amount -= achAmount;
                                }
                            }
                            amount = Math.Round(amount, 2);
                            Decimal paidAmount = (decimal)amount;
                            var getTenantTransaction = (from tt in db.tenanttransactions
                                                        where tt.TenantID == tenantID
                                                        && tt.TenantTransactionDate == item.Created.Date
                                                        && tt.TransactionAmount == paidAmount
                                                        && (tt.Comment == "Tenant Payment - Online ACH Payment" || tt.Comment == "Tenant Payment - Online Tenant Credit Card Payment" || tt.Comment == "Tenant Payment via ACH" || tt.Comment == "Tenant Payment via Credit Card")
                                                        select tt).FirstOrDefault();

                            if (getTenantTransaction != null)
                            {
                                Decimal transactionAmount = getTenantTransaction.TransactionAmount;

                                // Send PM Email
                                var getTenant = db.tenants.Where(x => x.TenantID == tenantID).FirstOrDefault();
                                var getUnit = db.units.Find(getTenant.UnitID);
                                Email email = new Email();
                                string ToEmail = "";
                                ToEmail = email.getPropertyManagerEmail((int)getTenant.PropertyID);
                                if (string.IsNullOrEmpty(ToEmail))
                                {
                                    ToEmail = email.getAdminEmail((int)getTenant.PropertyID);
                                }

                                string emailBody = "The payment of " + string.Format("{0:C}", transactionAmount) + " made on " + item.Created.Date.ToString("MM/dd/yyyy") + " was deleted from ";
                                emailBody += "tenant: " + getTenant.TenantFName + " " + getTenant.TenantLName + ". Unit: " + getUnit.UnitName + ".\n\n";
                                emailBody += "Reason: Refunded";
                                string subject = "Tenant Payment Refunded";

                                int checkRegisterID = getTenantTransaction.CheckRegisterID;
                                var getCR = db.checkregisters.Find(checkRegisterID);
                                if(getCR != null)
                                {
                                    db.checkregisters.Remove(getCR);
                                }

                                db.tenanttransactions.Remove(getTenantTransaction);
                                db.SaveChanges();

                                email.sendEmail(ToEmail, "support@myirent.com", subject, emailBody);
                            }
                        }
                    }
                }

                SendUsEmail emailOK = new SendUsEmail();
                emailOK.sendAlert("Just run Verify payments", "Verify Payments");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            } catch(Exception any)
            {
                SendUsEmail email = new SendUsEmail();
                email.sendError(any.ToString(), "Error Verify Refunded Payments");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}