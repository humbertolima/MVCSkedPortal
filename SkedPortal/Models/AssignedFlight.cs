//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SkedPortal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class AssignedFlight
    {
        public int id { get; set; }
        [Required]
        public int flight_number { get; set; }
        [Required]
        public System.DateTime flight_date { get; set; }
        public Nullable<int> captain { get; set; }
        public Nullable<int> first_officer { get; set; }
        public Nullable<int> fal { get; set; }
        public Nullable<int> fa1 { get; set; }
        public Nullable<int> fa2 { get; set; }
        public Nullable<int> fa3 { get; set; }
        public Nullable<int> fa4 { get; set; }
        public Nullable<int> fa5 { get; set; }
    }
}
