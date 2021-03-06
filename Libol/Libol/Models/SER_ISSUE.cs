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
    
    public partial class SER_ISSUE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SER_ISSUE()
        {
            this.SER_ARTICLE = new HashSet<SER_ARTICLE>();
            this.SER_COPY = new HashSet<SER_COPY>();
        }
    
        public int ID { get; set; }
        public short ClaimCycle1 { get; set; }
        public short ClaimCycle2 { get; set; }
        public short ClaimCycle3 { get; set; }
        public Nullable<System.DateTime> ClaimDate1 { get; set; }
        public Nullable<System.DateTime> ClaimDate2 { get; set; }
        public Nullable<System.DateTime> ClaimDate3 { get; set; }
        public bool SpecialIssue { get; set; }
        public string PhysDetail { get; set; }
        public byte DeliveryTime { get; set; }
        public decimal Price { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public System.DateTime IssuedDate { get; set; }
        public string Series { get; set; }
        public string IssueNo { get; set; }
        public int ReceivedCopies { get; set; }
        public int SubscribedCopies { get; set; }
        public string OvIssueNo { get; set; }
        public int ItemID { get; set; }
        public string VolumeByPublisher { get; set; }
        public string SpecialTitle { get; set; }
        public string Summary { get; set; }
        public bool FirstIssueInYear { get; set; }
        public string VolumeByLibrary { get; set; }
    
        public virtual ITEM ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SER_ARTICLE> SER_ARTICLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SER_COPY> SER_COPY { get; set; }
    }
}
