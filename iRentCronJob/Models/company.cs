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
    
    public partial class company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string ContactFName { get; set; }
        public string ContactLName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string CompanyAdd { get; set; }
        public string CompanyAdd2 { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZip { get; set; }
        public System.DateTime CompanyStartDate { get; set; }
        public int Active { get; set; }
        public int LeadSourceCompanyID { get; set; }
        public int MarketingSource { get; set; }
        public Nullable<int> ChargeByACH { get; set; }
        public Nullable<int> CompanyPayFee { get; set; }
        public Nullable<int> AllPropertiesTenantPortal { get; set; }
        public Nullable<int> SendToCollection { get; set; }
        public Nullable<int> AllowEvictionTenantPayOnline { get; set; }
        public Nullable<int> TurnOffOnlinePaymentsNotification { get; set; }
        public Nullable<int> ShowAllPropertiesTenants { get; set; }
        public Nullable<int> LateLicensePayment { get; set; }
        public Nullable<decimal> AmountDue { get; set; }
        public Nullable<int> TurnOffUpdatedTransactionNotification { get; set; }
        public Nullable<int> TurnOffSendToCollection { get; set; }
    }
}