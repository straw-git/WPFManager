
using FixedAssetsDBModels;
using System;
using System.ComponentModel;
using System.Data.Entity;

namespace FixedAssetsPlugin 
{
    public class FixedAssetsDBContext : DbContext
    {
        public FixedAssetsDBContext() : base(FixedAssetsDBModels.DBInfo.ConnectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<FixedAssetsDBContext>());
        }

        public DbSet<FixedAssets> FixedAssets { get; set; }
        public DbSet<FixedAssetsCheck> FixedAssetsCheck { get; set; }
    }
}
