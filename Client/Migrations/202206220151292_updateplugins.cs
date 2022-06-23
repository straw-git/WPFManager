namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateplugins : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plugins", "DependentIds", c => c.String(maxLength: 500));
            AddColumn("dbo.Plugins", "IsEnable", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Plugins", "ConnectionName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Plugins", "ConnectionString", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Plugins", "ConnectionString", c => c.String());
            AlterColumn("dbo.Plugins", "ConnectionName", c => c.String());
            DropColumn("dbo.Plugins", "IsEnable");
            DropColumn("dbo.Plugins", "DependentIds");
        }
    }
}
