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
    
    public partial class stripe_customer
    {
        public int id { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string StripeCustomer { get; set; }
    }
}
