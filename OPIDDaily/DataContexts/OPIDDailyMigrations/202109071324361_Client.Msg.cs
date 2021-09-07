namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientMsg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Msg", c => c.String());
            DropColumn("dbo.Clients", "Msgs");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "Msgs", c => c.String());
            DropColumn("dbo.Clients", "Msg");
        }
    }
}
