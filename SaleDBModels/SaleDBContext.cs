using CoreDBModels.Models;
using CustomerDBModels.Models;
using SaleDBModels.Models;
using System.Data.Entity;

namespace SaleDBModels
{
    public class SaleDBContext : DbContext
    {
        public SaleDBContext() : base("name=ZDBConnectionString")
        {

        }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<SaleOrder> SaleOrders { get; set; }
    }
}
