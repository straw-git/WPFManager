
using HRDBModels;
using System.Data.Entity;

namespace HRPlugin 
{
    public class HRDBContext : DbContext
    {
        public HRDBContext() : base(HRDBModels.DBInfo.ConnectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<HRDBContext>());
        }

        public DbSet<StaffContract> StaffContract { get; set; }
        public DbSet<StaffInsurance> StaffInsurance { get; set; }
        public DbSet<StaffSalary> StaffSalary { get; set; }
        public DbSet<StaffSalaryOther> StaffSalaryOther { get; set; }
        public DbSet<StaffSalarySettlement> StaffSalarySettlement { get; set; }
        public DbSet<StaffSalarySettlementLog> StaffSalarySettlementLog { get; set; }
    }
}

