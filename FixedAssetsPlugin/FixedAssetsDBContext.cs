using DBModels.Staffs;
using DBModels.Sys;
using FixedAssetsPlugin.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixedAssetsPlugin
{
    public class FixedAssetsDBContext : DbContext
    {
        public FixedAssetsDBContext() : base("name=ZDBConnectionString")
        {
        }

        public DbSet<SysDic> SysDic { get; set; }
        public DbSet<Staff> Staff { get; set; }

        public DbSet<FixedAssets> FixedAssets { get; set; }
        public DbSet<FixedAssetsCheck> FixedAssetsCheck { get; set; }
    }
}
