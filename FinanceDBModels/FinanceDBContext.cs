
using CoreDBModels.Models;
using FinanceDBModels.Models;
using System;
using System.ComponentModel;
using System.Data.Entity;


public class FinanceDBContext : DbContext
{
    public FinanceDBContext() : base("name=ZDBConnectionString")
    {
        Database.SetInitializer(new CreateDatabaseIfNotExists<FinanceDBContext>());
    }

    public DbSet<Staff> Staff { get; set; }
    public DbSet<User> User { get; set; }

    //财务
    public DbSet<FinanceBill> FinanceBill { get; set; }
    public DbSet<FinanceType> FinanceType { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<PaymentLog> PaymentLog { get; set; }
    public DbSet<PayOrder> PayOrder { get; set; }
}
