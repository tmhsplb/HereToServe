namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GiftCard2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GiftCards", "GiftCardType", c => c.Int(nullable: false));
            DropColumn("dbo.GiftCards", "HolderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GiftCards", "HolderId", c => c.Int(nullable: false));
            DropColumn("dbo.GiftCards", "GiftCardType");
        }
    }
}
