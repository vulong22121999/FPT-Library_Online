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
    using System.Collections.Generic;
    
    public partial class HOLDING_INVENTORY
    {
        public int ID { get; set; }
        public int HoldingID { get; set; }
        public bool OnLoan { get; set; }
        public string Shelf { get; set; }
        public string CorrectShelf { get; set; }
        public bool Liquidated { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<int> CorrectLocationID { get; set; }
        public bool Lock { get; set; }
        public int Type { get; set; }
        public int InventoryID { get; set; }
        public string CopyNumber { get; set; }
        public int ItemID { get; set; }
        public int LibID { get; set; }
        public Nullable<int> CorrectLibID { get; set; }
        public string Reason { get; set; }
        public int InventoryTime { get; set; }
        public string CallNumber { get; set; }
        public string Title { get; set; }
        public Nullable<int> ReasonID { get; set; }
    
        public virtual HOLDING_LIBRARY HOLDING_LIBRARY { get; set; }
        public virtual HOLDING_LOCATION HOLDING_LOCATION { get; set; }
        public virtual HOLDING_REMOVE_REASON HOLDING_REMOVE_REASON { get; set; }
        public virtual INVENTORY INVENTORY { get; set; }
        public virtual ITEM ITEM { get; set; }
    }
}
