using WarehouseDBModels;
using System.Data.Entity;

namespace Warehouse
{
    public class WarehouseDBContext : DbContext
    {
        public DbSet<Supplier> Suppliers { get; set; }
    }
}
