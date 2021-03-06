//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Libol.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ILL_OUTGOING_REQUESTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ILL_OUTGOING_REQUESTS()
        {
            this.ILL_OUTGOING_REQUESTS_LOG = new HashSet<ILL_OUTGOING_REQUESTS_LOG>();
        }
    
        public int ID { get; set; }
        public string RequestID { get; set; }
        public Nullable<int> ResponderID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<System.DateTime> NeedBeforeDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<System.DateTime> RespondDate { get; set; }
        public Nullable<System.DateTime> CancelledDate { get; set; }
        public Nullable<System.DateTime> ShippedDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> ReturnedDate { get; set; }
        public Nullable<System.DateTime> CheckedInDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> LocalDueDate { get; set; }
        public Nullable<System.DateTime> LocalCheckedInDate { get; set; }
        public Nullable<System.DateTime> LocalCheckedOutDate { get; set; }
        public Nullable<int> Renewals { get; set; }
        public Nullable<bool> Renewable { get; set; }
        public Nullable<int> LoanTypeID { get; set; }
        public string Barcode { get; set; }
        public Nullable<int> PatronID { get; set; }
        public Nullable<bool> NoticePatron { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public string CurrencyCode1 { get; set; }
        public Nullable<decimal> InsuredForCost { get; set; }
        public string CurrencyCode2 { get; set; }
        public Nullable<int> ReturnInsuranceCost { get; set; }
        public string CurrencyCode3 { get; set; }
        public Nullable<byte> ChargeableUnits { get; set; }
        public Nullable<decimal> MaxCost { get; set; }
        public string CurrencyCode { get; set; }
        public Nullable<int> Status { get; set; }
        public string Note { get; set; }
        public Nullable<byte> DeliveryLocID { get; set; }
        public Nullable<byte> BillingLocID { get; set; }
        public Nullable<bool> ReciprocalAgreement { get; set; }
        public Nullable<bool> WillPayFee { get; set; }
        public Nullable<bool> PaymentProvided { get; set; }
        public Nullable<byte> ServiceType { get; set; }
        public Nullable<byte> CopyrightCompliance { get; set; }
        public Nullable<byte> Priority { get; set; }
        public Nullable<byte> PaymentType { get; set; }
        public Nullable<int> ItemType { get; set; }
        public Nullable<byte> Medium { get; set; }
        public Nullable<byte> DelivMode { get; set; }
        public Nullable<int> EDelivModeID { get; set; }
        public Nullable<bool> Alert { get; set; }
        public string Title { get; set; }
        public string PatronName { get; set; }
        public string PatronCode { get; set; }
        public Nullable<int> PatronGroupID { get; set; }
    
        public virtual CIR_PATRON CIR_PATRON { get; set; }
        public virtual ILL_ITEM_TYPES ILL_ITEM_TYPES { get; set; }
        public virtual ILL_LIBRARIES ILL_LIBRARIES { get; set; }
        public virtual ILL_REQUEST_STATUS ILL_REQUEST_STATUS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ILL_OUTGOING_REQUESTS_LOG> ILL_OUTGOING_REQUESTS_LOG { get; set; }
    }
}
