namespace OPIDDaily.DataContexts.OPIDDailyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgencyId = c.Int(nullable: false),
                        AgencyName = c.String(),
                        ContactPerson = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AncientChecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecordID = c.Int(nullable: false),
                        sRecordID = c.String(),
                        InterviewRecordID = c.Int(nullable: false),
                        sInterviewRecordID = c.String(),
                        Name = c.String(),
                        DOB = c.DateTime(nullable: false),
                        sDOB = c.String(),
                        Num = c.Int(nullable: false),
                        sNum = c.String(),
                        Date = c.DateTime(),
                        sDate = c.String(),
                        Service = c.String(),
                        Amount = c.Int(nullable: false),
                        Disposition = c.String(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServiceDate = c.DateTime(nullable: false),
                        Expiry = c.DateTime(nullable: false),
                        AgencyId = c.Int(nullable: false),
                        AgencyName = c.String(),
                        ServiceTicket = c.String(),
                        WaitTime = c.Int(nullable: false),
                        Stage = c.String(),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        BirthName = c.String(),
                        DOB = c.DateTime(nullable: false),
                        Age = c.Int(nullable: false),
                        Conversation = c.Boolean(nullable: false),
                        HeadOfHousehold = c.Boolean(nullable: false),
                        HHId = c.Int(),
                        AKA = c.String(),
                        Email = c.String(),
                        BirthCity = c.String(),
                        BirthState = c.String(),
                        Phone = c.String(),
                        CurrentAddress = c.String(),
                        City = c.String(),
                        Staat = c.String(),
                        Zip = c.String(),
                        Msgs = c.String(),
                        Notes = c.String(),
                        Screened = c.DateTime(nullable: false),
                        CheckedIn = c.DateTime(nullable: false),
                        Interviewing = c.DateTime(nullable: false),
                        Interviewed = c.DateTime(nullable: false),
                        BackOffice = c.DateTime(nullable: false),
                        Done = c.DateTime(nullable: false),
                        BC = c.Boolean(nullable: false),
                        HCC = c.Boolean(nullable: false),
                        MBVD = c.Boolean(nullable: false),
                        State = c.String(),
                        NewTID = c.Boolean(nullable: false),
                        ReplacementTID = c.Boolean(nullable: false),
                        NewTDL = c.Boolean(nullable: false),
                        ReplacementTDL = c.Boolean(nullable: false),
                        Numident = c.Boolean(nullable: false),
                        RequestedDocument = c.String(),
                        SDBC = c.Boolean(nullable: false),
                        SDSSC = c.Boolean(nullable: false),
                        SDTID = c.Boolean(nullable: false),
                        SDTDL = c.Boolean(nullable: false),
                        SDTDCJ = c.Boolean(nullable: false),
                        SDVREG = c.Boolean(nullable: false),
                        SDML = c.Boolean(nullable: false),
                        SDDD = c.Boolean(nullable: false),
                        SDSL = c.Boolean(nullable: false),
                        SDDD214 = c.Boolean(nullable: false),
                        SDGC = c.Boolean(nullable: false),
                        SDEBT = c.Boolean(nullable: false),
                        SDHOTID = c.Boolean(nullable: false),
                        SDSchoolRecords = c.Boolean(nullable: false),
                        SDPassport = c.Boolean(nullable: false),
                        SDJobOffer = c.Boolean(nullable: false),
                        SDOther = c.Boolean(nullable: false),
                        SDOthersd = c.String(),
                        Incarceration = c.String(),
                        HousingStatus = c.String(),
                        USCitizen = c.String(),
                        Gender = c.String(),
                        Ethnicity = c.String(),
                        Race = c.String(),
                        MilitaryVeteran = c.String(),
                        DischargeStatus = c.String(),
                        Disabled = c.String(),
                        ACK = c.Boolean(nullable: false),
                        LCK = c.Boolean(nullable: false),
                        XID = c.Boolean(nullable: false),
                        XBC = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.HHId)
                .Index(t => t.HHId);
            
            CreateTable(
                "dbo.TextMsgs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InHouse = c.Boolean(nullable: false),
                        Date = c.DateTime(nullable: false),
                        From = c.String(),
                        To = c.String(),
                        Vid = c.Int(nullable: false),
                        Msg = c.String(),
                        Client_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.Client_Id)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Extended = c.DateTime(nullable: false),
                        Accepted = c.DateTime(nullable: false),
                        UserName = c.String(),
                        FullName = c.String(),
                        Email = c.String(),
                        Role = c.String(),
                        AgencyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MBVDs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MBVDId = c.Int(nullable: false),
                        MBVDName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PocketChecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        HeadOfHousehold = c.Boolean(nullable: false),
                        HH = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(),
                        DOB = c.DateTime(nullable: false),
                        Item = c.String(),
                        Num = c.Int(nullable: false),
                        Disposition = c.String(),
                        Notes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RChecks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecordID = c.Int(nullable: false),
                        sRecordID = c.String(),
                        InterviewRecordID = c.Int(nullable: false),
                        sInterviewRecordID = c.String(),
                        Name = c.String(),
                        DOB = c.DateTime(nullable: false),
                        sDOB = c.String(),
                        Num = c.Int(nullable: false),
                        sNum = c.String(),
                        Date = c.DateTime(),
                        sDate = c.String(),
                        Service = c.String(),
                        Amount = c.Int(nullable: false),
                        Disposition = c.String(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TextMsgs", "Client_Id", "dbo.Clients");
            DropForeignKey("dbo.Clients", "HHId", "dbo.Clients");
            DropIndex("dbo.TextMsgs", new[] { "Client_Id" });
            DropIndex("dbo.Clients", new[] { "HHId" });
            DropTable("dbo.RChecks");
            DropTable("dbo.PocketChecks");
            DropTable("dbo.MBVDs");
            DropTable("dbo.Invitations");
            DropTable("dbo.TextMsgs");
            DropTable("dbo.Clients");
            DropTable("dbo.AncientChecks");
            DropTable("dbo.Agencies");
        }
    }
}
