
using CoreDBModels;
using System.Data.Entity;

namespace Common
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext() : base(DBConnections.Get("CoreConnectionStr"))
        {
        }

        public DbSet<Log> Logs { get; set; }//日志
    }
}
