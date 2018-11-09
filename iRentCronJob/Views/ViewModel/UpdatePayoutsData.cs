using iRentCronJob.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class UpdatePayoutsData
    {
        public MyiRentEntities db = new MyiRentEntities();

        public UpdatePayoutsData()
        {
            try
            {
                
                // Get active Stripe keys
                var getStripeKeys = db.stripes.GroupBy(s => s.SecretKey).Select(x => x.FirstOrDefault()).ToList();

                foreach (var stripe in getStripeKeys)
                {
                    StripeConfiguration.SetApiKey(stripe.SecretKey);
                    // Get Payouts
                    var payoutService = new StripePayoutService();
                    StripeList<StripePayout> payoutItems = payoutService.List(
                      new StripePayoutListOptions()
                      {
                          Limit = 100
                      }
                    );
                    foreach (var payout in payoutItems)
                    {
                        if (payout.Status == "paid")
                        {
                            var checkPayout = db.stripepayouts.Where(p => p.StripePayoutID == payout.Id).ToList();
                            if (checkPayout.Count == 0)
                            {
                                // Insert Payout
                                stripepayout insertPayout = new stripepayout();
                                insertPayout.StripePayoutID = payout.Id;
                                insertPayout.PaidDate = payout.ArrivalDate;
                                insertPayout.PropertyID = stripe.PropertyID;
                                db.stripepayouts.Add(insertPayout);
                                db.SaveChanges();

                                // Get Payouts Payments
                                var balanceService = new StripeBalanceService();
                                StripeList<StripeBalanceTransaction> balanceTransactions = balanceService.List(
                                  new StripeBalanceTransactionListOptions()
                                  {
                                      Limit = 100,
                                      PayoutId = payout.Id
                                  }
                                );
                                foreach (var transaction in balanceTransactions)
                                {
                                    if (transaction.Description != "STRIPE PAYOUT")
                                    {
                                        stripepayoutdetail payoutDetails = new stripepayoutdetail();
                                        payoutDetails.AmountGross = (transaction.Amount / 100.0M);
                                        payoutDetails.Fee = (transaction.Fee / 100.0M);
                                        payoutDetails.AmountNet = (transaction.Net / 100.0M);
                                        payoutDetails.Description = transaction.Description;
                                        payoutDetails.StripePayoutID = insertPayout.ID;
                                        db.stripepayoutdetails.Add(payoutDetails);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                } 
            }
            catch (Exception e)
            {
                SendUsEmail error = new SendUsEmail();
                error.sendError(e.ToString(), "Error getting iRent Payouts");
            }            
        }
    }
}