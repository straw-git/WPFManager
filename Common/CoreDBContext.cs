
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

        public DbSet<Department> Department { get; set; }//部门
        public DbSet<DepartmentPosition> DepartmentPosition { get; set; }//职位

        public DbSet<User> User { get; set; }//用户
    }
}
