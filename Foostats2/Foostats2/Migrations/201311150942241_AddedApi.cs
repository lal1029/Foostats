namespace Foostats2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedApi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Players", "Trueskill_Id", c => c.Int());
            AddColumn("dbo.Players", "WinLoss_Id", c => c.Int());
            AddForeignKey("dbo.Players", "Trueskill_Id", "dbo.Trueskills", "Id");
            AddForeignKey("dbo.Players", "WinLoss_Id", "dbo.WinLosses", "Id");
            CreateIndex("dbo.Players", "Trueskill_Id");
            CreateIndex("dbo.Players", "WinLoss_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Players", new[] { "WinLoss_Id" });
            DropIndex("dbo.Players", new[] { "Trueskill_Id" });
            DropForeignKey("dbo.Players", "WinLoss_Id", "dbo.WinLosses");
            DropForeignKey("dbo.Players", "Trueskill_Id", "dbo.Trueskills");
            DropColumn("dbo.Players", "WinLoss_Id");
            DropColumn("dbo.Players", "Trueskill_Id");
            DropColumn("dbo.Players", "Discriminator");
        }
    }
}
