//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace yasemin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CONCEPT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONCEPT()
        {
            this.CONCEPT_RESTAURANT = new HashSet<CONCEPT_RESTAURANT>();
        }
    
        public int ConceptId { get; set; }
        public string ConceptName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONCEPT_RESTAURANT> CONCEPT_RESTAURANT { get; set; }
    }
}