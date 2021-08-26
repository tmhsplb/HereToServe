namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GiftCardInventory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GiftCardInventories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InventoryDate = c.DateTime(nullable: false),
                        METROCards20 = c.Int(nullable: false),
                        METROCards30 = c.Int(nullable: false),
                        METROCards40 = c.Int(nullable: false),
                        METROCards50 = c.Int(nullable: false),
                        VisaCards20 = c.Int(nullable: false),
                        VisaCards30 = c.Int(nullable: false),
                        VisaCards40 = c.Int(nullable: false),
                        VisaCards50 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Agencies", "METROBudget", c => c.Int(nullable: false));
            AddColumn("dbo.Agencies", "VisaBudget", c => c.Int(nullable: false));
            AddColumn("dbo.GiftCards", "HolderId", c => c.Int(nullable: false));
            AddColumn("dbo.GiftCards", "BalanceDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.GiftCards", "CardBalance", c => c.Int(nullable: false));
            AddColumn("dbo.GiftCards", "Client_Id", c => c.Int());
            CreateIndex("dbo.GiftCards", "Client_Id");
            AddForeignKey("dbo.GiftCards", "Client_Id", "dbo.Clients", "Id");
            DropColumn("dbo.GiftCards", "Owner");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GiftCards", "Owner", c => c.String());
            DropForeignKey("dbo.GiftCards", "Client_Id", "dbo.Clients");
            DropIndex("dbo.GiftCards", new[] { "Client_Id" });
            DropColumn("dbo.GiftCards", "Client_Id");
            DropColumn("dbo.GiftCards", "CardBalance");
            DropColumn("dbo.GiftCards", "BalanceDate");
            DropColumn("dbo.GiftCards", "HolderId");
            DropColumn("dbo.Agencies", "VisaBudget");
            DropColumn("dbo.Agencies", "METROBudget");
            DropTable("dbo.GiftCardInventories");
        }
    }
}
