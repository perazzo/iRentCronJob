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
    
    public partial class vendor
    {
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress1 { get; set; }
        public string VendorAddress2 { get; set; }
        public string VendorCity { get; set; }
        public string VendorState { get; set; }
        public string VendorZip { get; set; }
        public string VendorPhone { get; set; }
        public string VendorEmail { get; set; }
        public System.DateTime VendorStartDate { get; set; }
        public int CompanyID { get; set; }
        public string RoutingNumber { get; set; }
        public string AcountNumber { get; set; }
        public int Active { get; set; }
        public string VendorEIN { get; set; }
        public Nullable<int> A1099 { get; set; }
        public string Memo { get; set; }
    }
}
