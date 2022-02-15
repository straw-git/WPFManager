
using CoreDBModels.Models;
using FixedAssetsDBModels.Models;
using System;
using System.ComponentModel;
using System.Data.Entity;


public class FixedAssetsDBContext : DbContext
{
    public FixedAssetsDBContext() : base("name=ZDBConnectionString")
    {
        Database.SetInitializer(new CreateDatabaseIfNotExists<FixedAssetsDBContext>());
    }

    public DbSet<Staff> Staff { get; set; }
    public DbSet<SysDic> SysDic { get; set; }

    public DbSet<FixedAssets> FixedAssets { get; set; }
    public DbSet<FixedAssetsCheck> FixedAssetsCheck { get; set; }
}
