
using CoreDBModels;
using System.Data.Entity;

namespace LiveChartsTestPlugin
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext() : base(CoreDBModels.DBInfo.ConnectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDBContext>());
        }

        public DbSet<Staff> Staff { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<SysDic> SysDic { get; set; }
    }
}
