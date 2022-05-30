namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Emails", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Emails", "Title");
        }
    }
}
