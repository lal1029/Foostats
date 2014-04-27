namespace Foostats2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Matches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Score1 = c.Int(nullable: false),
                        Score2 = c.Int(nullable: false),
                        Team1Validated = c.DateTime(),
                        Team2Validated = c.DateTime(),
                        Team1_Id = c.Int(),
                        Team2_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.Team1_Id)
                .ForeignKey("dbo.Teams", t => t.Team2_Id)
                .Index(t => t.Team1_Id)
                .Index(t => t.Team2_Id);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(),
                        Player1_Id = c.Int(),
                        Player2_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Player1_Id)
                .ForeignKey("dbo.Players", t => t.Player2_Id)
                .Index(t => t.Player1_Id)
                .Index(t => t.Player2_Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(),
                        Password = c.String(),
                        Salt = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Trueskills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StandardDeviation = c.Double(nullable: false),
                        Mean = c.Double(nullable: false),
                        ConservativeRating = c.Double(nullable: false),
                        Player_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Player_Id)
                .Index(t => t.Player_Id);
            
            CreateTable(
                "dbo.RegistryEntries",
                c => new
                    {
                        ParentKey = c.String(nullable: false, maxLength: 128),
                        ChildKey = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => new { t.ParentKey, t.ChildKey });
            
            CreateTable(
                "dbo.WinLosses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Win = c.Int(nullable: false),
                        Loss = c.Int(nullable: false),
                        Player_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Player_Id)
                .Index(t => t.Player_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.WinLosses", new[] { "Player_Id" });
            DropIndex("dbo.Trueskills", new[] { "Player_Id" });
            DropIndex("dbo.Teams", new[] { "Player2_Id" });
            DropIndex("dbo.Teams", new[] { "Player1_Id" });
            DropIndex("dbo.Matches", new[] { "Team2_Id" });
            DropIndex("dbo.Matches", new[] { "Team1_Id" });
            DropForeignKey("dbo.WinLosses", "Player_Id", "dbo.Players");
            DropForeignKey("dbo.Trueskills", "Player_Id", "dbo.Players");
            DropForeignKey("dbo.Teams", "Player2_Id", "dbo.Players");
            DropForeignKey("dbo.Teams", "Player1_Id", "dbo.Players");
            DropForeignKey("dbo.Matches", "Team2_Id", "dbo.Teams");
            DropForeignKey("dbo.Matches", "Team1_Id", "dbo.Teams");
            DropTable("dbo.WinLosses");
            DropTable("dbo.RegistryEntries");
            DropTable("dbo.Trueskills");
            DropTable("dbo.Players");
            DropTable("dbo.Teams");
            DropTable("dbo.Matches");
        }
    }
}
