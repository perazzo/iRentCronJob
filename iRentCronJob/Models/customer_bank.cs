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
    
    public partial class customer_bank
    {
        public int CustomerBankID { get; set; }
        public int CustomerID { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public bool Verified { get; set; }
    }
}
