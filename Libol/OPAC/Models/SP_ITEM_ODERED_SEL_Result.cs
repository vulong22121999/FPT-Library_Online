//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OPAC.Models
{
    using System;
    
    public partial class SP_ITEM_ODERED_SEL_Result
    {
        public string RegularityCode { get; set; }
        public int ID { get; set; }
        public int TypeID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string Currency { get; set; }
        public bool Accepted { get; set; }
        public string Note { get; set; }
        public bool Renewed { get; set; }
        public Nullable<decimal> IssuePrice { get; set; }
        public string ISBN { get; set; }
        public string ISSN { get; set; }
        public string Edition { get; set; }
        public Nullable<int> Urgency { get; set; }
        public string SerialCode { get; set; }
        public string PubYear { get; set; }
        public Nullable<System.DateTime> ValidSubscribedDate { get; set; }
        public Nullable<System.DateTime> ExpiredSubscribedDate { get; set; }
        public Nullable<int> LanguageID { get; set; }
        public string Requester { get; set; }
        public bool Acquired { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string Publisher { get; set; }
        public Nullable<int> Issues { get; set; }
        public Nullable<int> AcceptedCopies { get; set; }
        public Nullable<int> ReceivedCopies { get; set; }
        public Nullable<int> RequestedCopies { get; set; }
        public string Author { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> ItemTypeID { get; set; }
        public string Title { get; set; }
        public Nullable<int> MediumID { get; set; }
        public Nullable<int> POID { get; set; }
        public Nullable<int> ClassID { get; set; }
        public string TypeCode { get; set; }
        public string ReceiptNo { get; set; }
    }
}
