namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GiftCard1 : DbMigration
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
