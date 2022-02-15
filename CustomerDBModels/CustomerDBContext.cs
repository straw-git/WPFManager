
using CoreDBModels.Models;
using CustomerDBModels.Models;
using System;
using System.ComponentModel;
using System.Data.Entity;


public class CustomerDBContext : DbContext
{
    public CustomerDBContext() : base("name=ZDBConnectionString")
    {
        Database.SetInitializer(new CreateDatabaseIfNotExists<CustomerDBContext>());
    }

    public DbSet<User> User { get; set; }
    public DbSet<Customer> Customer { get; set; }

    //顾客
    public DbSet<CustomerTemp> CustomerTemp { get; set; }
    //会员
    public DbSet<Member> Member { get; set; }
    public DbSet<MemberLevel> MemberLevel { get; set; }
    public DbSet<MemberRecharge> MemberRecharge { get; set; }
    //活动
    public DbSet<MJActivity> MJActivity { get; set; }//满减活动 含会员
}
