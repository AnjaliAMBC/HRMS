﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HRMS_EntityFramework : DbContext
    {
        public HRMS_EntityFramework()
            : base("name=HRMS_EntityFramework")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<emplogin> emplogins { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<emp_info> emp_info { get; set; }
        public virtual DbSet<LeaveRM> LeaveRMs { get; set; }
        public virtual DbSet<tbld_ambclogininformation> tbld_ambclogininformation { get; set; }
        public virtual DbSet<tblambcholiday> tblambcholidays { get; set; }
        public virtual DbSet<CheckInView> CheckInViews { get; set; }
        public virtual DbSet<LeaveBalance> LeaveBalances { get; set; }
        public virtual DbSet<Compoff> Compoffs { get; set; }
        public virtual DbSet<con_leaveupdate> con_leaveupdate { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<IT_Ticket> IT_Ticket { get; set; }
        public virtual DbSet<ReminderLog> ReminderLogs { get; set; }
        public virtual DbSet<VendorList> VendorLists { get; set; }
        public virtual DbSet<VendorType> VendorTypes { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<AssetTransfer_> AssetTransfer_ { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<SubscriptionHistory> SubscriptionHistories { get; set; }
        public virtual DbSet<IT_Maintenance> IT_Maintenance { get; set; }
        public virtual DbSet<JobReferral> JobReferrals { get; set; }
        public virtual DbSet<JobDetail> JobDetails { get; set; }
    }
}
