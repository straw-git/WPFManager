
using FinanceDBModels;
using System.Data.Entity;

namespace FinancePlugin
{
    public class FinanceDBContext : DbContext
    {
        public FinanceDBContext() : base(FinanceDBModels.DBInfo.ConnectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<FinanceDBContext>());
        }
        //财务
        public DbSet<FinanceBill> FinanceBill { get; set; }
        public DbSet<FinanceType> FinanceType { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<PaymentLog> PaymentLog { get; set; }
        public DbSet<PayOrder> PayOrder { get; set; }
    }
}
