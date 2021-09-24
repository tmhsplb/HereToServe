namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferringAgent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "ReferringAgentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "ReferringAgentId");
        }
    }
}
