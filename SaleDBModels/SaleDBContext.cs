using System.Data.Entity;

namespace SaleDBModels
{
    public class SaleDBContext : DbContext
    {
        public SaleDBContext() : base("name=ZDBConnectionString")
        {

        }

        public DbSet<SaleOrder> SaleOrders { get; set; }
    }
}
