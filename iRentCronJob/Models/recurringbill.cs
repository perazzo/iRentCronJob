//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iRentCronJob.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class recurringbill
    {
        public int RecurringBillsID { get; set; }
        public int PropertyID { get; set; }
        public decimal Amount { get; set; }
        public int ExpenseTypeID { get; set; }
        public string Memo { get; set; }
        public System.DateTime FirstPayDate { get; set; }
        public int TransactionType { get; set; }
        public int VendorID { get; set; }
        public int Escrow { get; set; }
        public System.DateTime SubmitDate { get; set; }
        public string InvoiceNumber { get; set; }
        public int SubmittedBy { get; set; }
        public int FrequencyID { get; set; }
        public int PostMethodID { get; set; }
        public int NumberofPayments { get; set; }
        public int Unlimited { get; set; }
        public int Paid { get; set; }
    }
}