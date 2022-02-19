
using SaleDBModels;
using System.Data.Entity;

namespace SalePlugin
{
    public class SaleDBContext : DbContext
    {
        public SaleDBContext() : base(SaleDBModels.DBInfo.ConnectionString)
        {

        }
        public DbSet<SaleOrder> SaleOrders { get; set; }
    }
}
