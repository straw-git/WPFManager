using DBModels.Activities;
using DBModels.ERP;
using DBModels.Finance;
using DBModels.Member;
using DBModels.Staffs;
using DBModels.Sys;
using System;
using System.ComponentModel;
using System.Data.Entity;


public class DBContext : DbContext
{
    public DBContext() : base("name=ZDBConnectionString")
    {
        Database.SetInitializer(new CreateDatabaseIfNotExists<DBContext>());
    }

    //字典
    public DbSet<SysDic> SysDic { get; set; }

    //人员
    public DbSet<Staff> Staff { get; set; }
    public DbSet<StaffContract> StaffContract { get; set; }
    public DbSet<StaffInsurance> StaffInsurance { get; set; }
    public DbSet<StaffSalary> StaffSalary { get; set; }
    public DbSet<StaffSalaryOther> StaffSalaryOther { get; set; }
    public DbSet<StaffSalarySettlement> StaffSalarySettlement { get; set; }
    public DbSet<StaffSalarySettlementLog> StaffSalarySettlementLog { get; set; }

    //物品
    public DbSet<Goods> Goods { get; set; }

    //供应商
    public DbSet<Supplier> Supplier { get; set; }

    //系统账户
    public DbSet<User> User { get; set; }

    //库存
    public DbSet<Stock> Stock { get; set; }
    public DbSet<StockLog> StockLog { get; set; }

    //日志
    public DbSet<Log> Log { get; set; }

    //采购
    public DbSet<PurchasePlan> PurchasePlan { get; set; }
    public DbSet<PurchasePlanItem> PurchasePlanItem { get; set; }
    public DbSet<PurchasePlanLog> PurchasePlanLog { get; set; }

    //财务
    public DbSet<FinanceBill> FinanceBill { get; set; }
    public DbSet<FinanceType> FinanceType { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<PaymentLog> PaymentLog { get; set; }
    public DbSet<PayOrder> PayOrder { get; set; }

    //会员
    public DbSet<Member> Member { get; set; }
    public DbSet<MemberLevel> MemberLevel { get; set; }
    public DbSet<MemberRecharge> MemberRecharge { get; set; }

    //顾客
    public DbSet<Customer> Customer { get; set; }
    public DbSet<CustomerTemp> CustomerTemp { get; set; }

    //活动
    public DbSet<MJActivity> MJActivity { get; set; }//满减活动 含会员

    //附件
    public DbSet<Attachment> Attachments { get; set; }

}
