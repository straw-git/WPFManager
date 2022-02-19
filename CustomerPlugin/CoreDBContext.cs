
using CoreDBModels;
using System.Data.Entity;

namespace CustomerPlugin 
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext() : base(CoreDBModels.DBInfo.ConnectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDBContext>());
        }

        public DbSet<User> User { get; set; }
    }
}
