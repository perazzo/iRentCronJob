using iRentCronJob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class PostRecurringBills
    {
        public MyiRentEntities db = new MyiRentEntities();

        public PostRecurringBills()
        {
            try
            {
                var GetRecurringBillsUpdate = (from rb in db.recurringbills
                                               join pm in db.postmethods on rb.PostMethodID equals pm.PostMethodID
                                               join f in db.frequencies on rb.FrequencyID equals f.FrequencyID
                                               join p in db.properties on rb.PropertyID equals p.PropertyID
                                               join et in db.expensetypes on rb.ExpenseTypeID equals et.ExpenseTypeID
                                               join v in db.vendors on rb.VendorID equals v.VendorID
                                               select rb).ToList();

                // If first pay date is in the past, update to today
                foreach (var rBills in GetRecurringBillsUpdate)
                {
                    if (rBills.FirstPayDate < DateTime.Now)
                    {
                        rBills.FirstPayDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }

                var GetRecurringBills = (from rb in db.recurringbills
                                         join pm in db.postmethods on rb.PostMethodID equals pm.PostMethodID
                                         join f in db.frequencies on rb.FrequencyID equals f.FrequencyID
                                         join p in db.properties on rb.PropertyID equals p.PropertyID
                                         join et in db.expensetypes on rb.ExpenseTypeID equals et.ExpenseTypeID
                                         join v in db.vendors on rb.VendorID equals v.VendorID
                                         where rb.FirstPayDate == DateTime.Now && rb.PostMethodID == 1
                                         select rb).ToList();
                foreach (var rBills in GetRecurringBills)
                {
                    // Add Bill
                    checkregister cr = new checkregister();
                    cr.PropertyID = rBills.PropertyID;
                    cr.VendorID = rBills.VendorID;
                    cr.Amount = rBills.Amount;
                    cr.Memo = rBills.Memo;
                    cr.ExpenseTypeID = rBills.ExpenseTypeID;
                    cr.CheckDate = DateTime.Now;
                    cr.TransactionType = 1;
                    cr.Paid = rBills.Paid;
                    cr.Reconciled = 0;
                    cr.Escrow = rBills.Escrow;
                    cr.InvoiceDate = DateTime.Now;
                    cr.PaidDate = DateTime.Now;
                    cr.InvoiceNumber = rBills.InvoiceNumber;
                    cr.SubmittedBy = 16; // Will User's id
                    db.checkregisters.Add(cr);
                    db.SaveChanges();

                    DateTime nextPayDate = DateTime.Now;
                    if (rBills.FrequencyID == 1)
                        nextPayDate.AddDays(7);
                    if (rBills.FrequencyID == 2)
                        nextPayDate.AddDays(14);
                    if (rBills.FrequencyID == 3)
                    {
                        int days = DateTime.DaysInMonth(nextPayDate.Year, nextPayDate.Month);
                        days = days / 2;
                        nextPayDate.AddDays(days);
                    }
                    if (rBills.FrequencyID == 4)
                        nextPayDate.AddMonths(1);
                    if (rBills.FrequencyID == 5)
                        nextPayDate.AddMonths(3);
                    if (rBills.FrequencyID == 6)
                        nextPayDate.AddMonths(6);
                    if (rBills.FrequencyID == 7)
                        nextPayDate.AddYears(1);

                    if (rBills.Unlimited != 1)
                    {
                        int numPayments = rBills.NumberofPayments - 1;
                        if (numPayments == 0)
                        {
                            db.recurringbills.Remove(rBills);
                        }
                        else
                        {
                            rBills.NumberofPayments = numPayments;
                            rBills.FirstPayDate = nextPayDate;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        rBills.FirstPayDate = nextPayDate;
                        db.SaveChanges();
                    }
                }

                string msg = "There were " + GetRecurringBills.Count.ToString() + " bills posted.";
                SendUsEmail message = new SendUsEmail();
                message.sendAlert(msg, "We just posted Recurring Bills.");
            } catch(Exception any)
            {
                SendUsEmail message = new SendUsEmail();
                message.sendAlert(any.ToString(), "Error - We just posted Recurring Bills.");
            }
        }
    }
}