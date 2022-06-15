namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WayBackMachine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "WBM", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "WBM");
        }
    }
}
