//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JobFinder.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class biografije
    {
        public int idbiografije { get; set; }
        public int idposloprimac { get; set; }
        public string zanimanje { get; set; }
        public string radno_iskustvo { get; set; }
        public string idealan_posao { get; set; }
        public string kompetencije { get; set; }
        public Nullable<System.DateTime> datum_biografije { get; set; }
        public Nullable<int> idkategorije { get; set; }
    
        public virtual kategorije kategorije { get; set; }
        public virtual posloprimci posloprimci { get; set; }
    }
}
