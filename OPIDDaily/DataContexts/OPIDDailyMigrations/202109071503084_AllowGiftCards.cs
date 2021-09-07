namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowGiftCards : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Agencies", "AllowGiftCards", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Agencies", "AllowGiftCards");
        }
    }
}
