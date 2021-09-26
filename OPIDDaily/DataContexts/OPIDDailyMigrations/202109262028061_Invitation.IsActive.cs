namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvitationIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invitations", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invitations", "IsActive");
        }
    }
}
