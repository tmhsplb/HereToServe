namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GiftCards3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GiftCards", "IsCurrent", c => c.Boolean(nullable: false));
            AddColumn("dbo.GiftCards", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GiftCards", "IsActive");
            DropColumn("dbo.GiftCards", "IsCurrent");
        }
    }
}
