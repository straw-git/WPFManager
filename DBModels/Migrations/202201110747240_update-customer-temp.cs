namespace DBModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecustomertemp : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CustomerTemps");
            AddColumn("dbo.CustomerTemps", "Phone", c => c.String(maxLength: 50));
            AddColumn("dbo.CustomerTemps", "XCM", c => c.Boolean(nullable: false));
            AlterColumn("dbo.CustomerTemps", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.CustomerTemps", "Id");
            DropColumn("dbo.CustomerTemps", "HS");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerTemps", "HS", c => c.Boolean(nullable: false));
            DropPrimaryKey("dbo.CustomerTemps");
            AlterColumn("dbo.CustomerTemps", "Id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.CustomerTemps", "XCM");
            DropColumn("dbo.CustomerTemps", "Phone");
            AddPrimaryKey("dbo.CustomerTemps", "Id");
        }
    }
}
