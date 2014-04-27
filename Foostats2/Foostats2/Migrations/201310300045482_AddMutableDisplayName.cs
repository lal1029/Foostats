namespace Foostats2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMutableDisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "MutableDisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "MutableDisplayName");
        }
    }
}
