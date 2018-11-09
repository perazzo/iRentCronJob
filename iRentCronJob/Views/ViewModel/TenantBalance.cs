using iRentCronJob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentCronJob.Views.ViewModel
{
    public class TenantBalance
    {
        public MyiRentEntities db = new MyiRentEntities();

        public Decimal TotalCredits { get; set; }
        public Decimal TotalDebits { get; set; }
        public Decimal TotalTenantBalance { get; set; }

        public TenantBalance(int tenantID)
        {
            decimal totalCredits = 0;
            decimal totalDebits = 0;
            var getTransactionDetails = (from tt in db.tenanttransactions
                                         join ttype in db.transactiontypes on tt.TransactionTypeID equals ttype.TransactionTypeID
                                         join ct in db.chargetypes on tt.ChargeTypeID equals ct.ChargeTypeID
                                         where tt.TenantID == tenantID && ct.ChargeTypeID != 6
                                         select tt).ToList();

            foreach (var tenantTransaction in getTransactionDetails)
            {
                // debit
                if(tenantTransaction.TransactionTypeID == 1)
                {
                    totalDebits += tenantTransaction.TransactionAmount;
                }
                // credit
                else if(tenantTransaction.TransactionTypeID == 2)
                {
                    totalCredits += tenantTransaction.TransactionAmount;
                }
            }

            TotalDebits = totalDebits;
            TotalCredits = totalCredits;
            TotalTenantBalance = totalDebits - totalCredits;
        }
    }
}