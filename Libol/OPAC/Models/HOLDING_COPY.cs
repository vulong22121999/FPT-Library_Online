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
    
    public partial class HOLDING_COPY
    {
        public int ItemID { get; set; }
        public Nullable<int> TotalCopies { get; set; }
        public Nullable<int> FreeCopies { get; set; }
        public Nullable<int> LoanTypeID { get; set; }
    
        public virtual CIR_LOAN_TYPE CIR_LOAN_TYPE { get; set; }
        public virtual ITEM ITEM { get; set; }
    }
}
