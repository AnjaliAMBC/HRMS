//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class JobReferral
    {
        public int Sno { get; set; }
        public Nullable<int> JobID { get; set; }
        public string CandidateName { get; set; }
        public string ResumePath { get; set; }
        public string ReferredBy { get; set; }
        public string Condidatemblnumber { get; set; }
        public string ReferredById { get; set; }
        public string ReferredByEmail { get; set; }
        public Nullable<System.DateTime> ReferredDate { get; set; }
        public string CandidateStatus { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
