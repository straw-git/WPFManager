namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class webapi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CoreSettings", "APIUrl", c => c.String(maxLength: 500));
            DropColumn("dbo.CoreSettings", "PluginsUpdateBaseUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CoreSettings", "PluginsUpdateBaseUrl", c => c.String(maxLength: 500));
            DropColumn("dbo.CoreSettings", "APIUrl");
        }
    }
}
