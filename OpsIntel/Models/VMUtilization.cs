//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpsIntel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VMUtilization
    {
        public int Id { get; set; }
        public int VMId { get; set; }
        public System.DateTime Date { get; set; }
        public decimal CPUMinimum { get; set; }
        public decimal CPUMaximum { get; set; }
        public decimal DiskReadMinimum { get; set; }
        public decimal DiskReadMaximum { get; set; }
        public decimal DiskWriteMinimum { get; set; }
        public decimal DiskWriteMaximum { get; set; }
        public decimal NetworkInMinimum { get; set; }
        public decimal NetworkInMaximum { get; set; }
        public decimal NetworkOutMinimum { get; set; }
        public decimal NetworkOutMaximum { get; set; }
        public decimal MemoryMin { get; set; }
        public decimal C_MemoryMax { get; set; }
    }
}