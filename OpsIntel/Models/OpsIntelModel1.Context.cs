﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OpsIntel1Entities1 : DbContext
    {
        public OpsIntel1Entities1()
            : base("name=OpsIntel1Entities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C_VMThreshold> C_VMThreshold { get; set; }
        public virtual DbSet<testtable> testtables { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<VMAction> VMActions { get; set; }
        public virtual DbSet<VMAutomationRule> VMAutomationRules { get; set; }
        public virtual DbSet<VMThreshold> VMThresholds { get; set; }
        public virtual DbSet<VMUtilization> VMUtilizations { get; set; }
        public virtual DbSet<VMValue> VMValues { get; set; }
    }
}
