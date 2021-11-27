namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customer_black : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "IsBlack", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "IsBlack");
        }
    }
}
