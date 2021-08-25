namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GiftCard : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GiftCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Owner = c.String(),
                        RegistrationID = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GiftCards");
        }
    }
}
