
using CoreDBModels.Models;
using ERPDBModels.Models;
using System;
using System.ComponentModel;
using System.Data.Entity;


public class ERPDBContext : DbContext
{
    public ERPDBContext() : base("name=ZDBConnectionString")
    {
        Database.SetInitializer(new CreateDatabaseIfNotExists<ERPDBContext>());
    }

    public DbSet<SysDic> SysDic { get; set; }
    public DbSet<User> User { get; set; }

    //供应商
    public DbSet<Supplier> Supplier { get; set; }
    //物品
    public DbSet<Goods> Goods { get; set; }
    //采购
    public DbSet<PurchasePlan> PurchasePlan { get; set; }
    public DbSet<PurchasePlanItem> PurchasePlanItem { get; set; }
    public DbSet<PurchasePlanLog> PurchasePlanLog { get; set; }
    //库存
    public DbSet<Stock> Stock { get; set; }
    public DbSet<StockLog> StockLog { get; set; }

}
