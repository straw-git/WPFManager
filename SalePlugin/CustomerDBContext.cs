
using CustomerDBModels;
using System.Data.Entity;

namespace SalePlugin
{
    public class CustomerDBContext : DbContext
    {
        public CustomerDBContext() : base(SaleDBModels.DBInfo.ConnectionString)
        {

        }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerTemp> CustomerTemp { get; set; }
    }
}
