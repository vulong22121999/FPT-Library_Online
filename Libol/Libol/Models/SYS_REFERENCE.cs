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
    
    public partial class SYS_REFERENCE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SYS_REFERENCE()
        {
            this.SYS_USER = new HashSet<SYS_USER>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public string Name { get; set; }
        public string WorkURL { get; set; }
        public string SentURL { get; set; }
        public string Icon { get; set; }
    
        public virtual SYS_MODULE SYS_MODULE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SYS_USER> SYS_USER { get; set; }
    }
}
