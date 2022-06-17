namespace GenerateCouponCapstone2022.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createemailentity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        EmailID = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        CouponID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmailID)
                .ForeignKey("dbo.Coupons", t => t.CouponID, cascadeDelete: true)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID)
                .Index(t => t.CouponID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Emails", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Emails", "CouponID", "dbo.Coupons");
            DropIndex("dbo.Emails", new[] { "CouponID" });
            DropIndex("dbo.Emails", new[] { "CustomerID" });
            DropTable("dbo.Emails");
        }
    }
}
