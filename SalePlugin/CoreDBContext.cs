
using CoreDBModels;
using System.Data.Entity;

namespace SalePlugin
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext() : base(CoreDBModels.DBInfo.ConnectionString)
        {

        }
        public DbSet<User> User { get; set; }
    }
}
