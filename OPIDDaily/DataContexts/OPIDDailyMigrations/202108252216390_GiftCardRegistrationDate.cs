namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GiftCardRegistrationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GiftCards", "RegistrationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GiftCards", "RegistrationDate");
        }
    }
}
