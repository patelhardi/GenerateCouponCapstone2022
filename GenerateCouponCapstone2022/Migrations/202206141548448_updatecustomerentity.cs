namespace GenerateCouponCapstone2022.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecustomerentity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "UserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Customers", "UserID");
            AddForeignKey("dbo.Customers", "UserID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Customers", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.Customers", new[] { "UserID" });
            DropColumn("dbo.Customers", "UserID");
        }
    }
}
