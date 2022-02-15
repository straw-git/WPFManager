
using CoreDBModels.Models;
using HRDBModels.Models;
using System;
using System.ComponentModel;
using System.Data.Entity;


public class HRDBContext : DbContext
{
    public HRDBContext() : base("name=ZDBConnectionString")
    {
        Database.SetInitializer(new CreateDatabaseIfNotExists<HRDBContext>());
    }

    public DbSet<Staff> Staff { get; set; }
    public DbSet<SysDic> SysDic { get; set; }

    public DbSet<StaffContract> StaffContract { get; set; }
    public DbSet<StaffInsurance> StaffInsurance { get; set; }
    public DbSet<StaffSalary> StaffSalary { get; set; }
    public DbSet<StaffSalaryOther> StaffSalaryOther { get; set; }
    public DbSet<StaffSalarySettlement> StaffSalarySettlement { get; set; }
    public DbSet<StaffSalarySettlementLog> StaffSalarySettlementLog { get; set; }
}
